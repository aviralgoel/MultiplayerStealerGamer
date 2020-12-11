using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System.Linq;


public class GameManager : MonoBehaviourPunCallbacks
{
    [Header("Game Stats")]
    public bool hasGameEnded = false;
    public float timeToWin;
    public float invincibilityDuration;
    public float hatPickupTime;

    [Header("Player")]
    public string playerPrefabLocation;
    public Transform[] spawnPoints;
    public PlayerController[] players;
    public int playerWithHatID;
    private int playersInGame;


    public static GameManager instance;
    //instance
    private void Awake()
    {
        // Make sure there is only one instance of this GameManager existing
        if (instance != null & instance != this)
            gameObject.SetActive(false);
        else
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    private void Start()
    {
        //Set the length of PlayerController array to the number of players determined by PhotonNetwork
        players = new PlayerController[PhotonNetwork.PlayerList.Length];
        photonView.RPC("ImInGame", RpcTarget.AllBuffered);

    }


    // Each player tells the GameManager that they've joined the game and check if we are ready to start the game
    // it is PUNRPC because we need to notify everyother player on the network about number of players who are ready
    [PunRPC]
    private void ImInGame()
    {
        playersInGame++;

        // if everyone joined the game scene, then spawn all.
        if(playersInGame == PhotonNetwork.PlayerList.Length)
        {
            SpawnThePlayer();
        }
    }

    private void SpawnThePlayer()
    {
        GameObject playerObj = PhotonNetwork.Instantiate(playerPrefabLocation, 
                                   spawnPoints[Random.Range(0, spawnPoints.Length)].position, Quaternion.identity);

        PlayerController playerScript = playerObj.GetComponent<PlayerController>();
        
        // initialize the player
        playerScript.photonView.RPC("Initialize", RpcTarget.All, PhotonNetwork.LocalPlayer);

    }


    /// <summary>
    /// Accessor functions, that will return the PlayerController script of the player whose player ID or whose GameObject 
    /// is passed.
    /// </summary>
    public PlayerController GetPlayer(int playerId)
    {
        return players.First(x => x.id == playerId);
    }

    public PlayerController GetPlayer(GameObject playerObject)
    {
        return players.First(x => x.gameObject == playerObject);
    }


    // called when a player hits the hatted player
    [PunRPC]
    public void GiveHat(int playerID, bool initialGive)
    {
        //Remove the hat from currently hatted player
        if(!initialGive)
        {
            GetPlayer(playerWithHatID).SetHat(false);
        }

        //Give Hat to the new player, also update the ID of the currentHattedPlayer
        playerWithHatID = playerID;
        GetPlayer(playerID).SetHat(true);
        hatPickupTime = Time.time;
    }

    public bool CanGetHat()
    {
        if (Time.time > hatPickupTime + invincibilityDuration)
        {
            return true;
        }
        else
            return false;
    }
    [PunRPC]
    public void WinTheGame(int _playerid)
    {
        hasGameEnded = true;
        PlayerController player = GetPlayer(_playerid);
        // set ui who won
        UIManager.instance.SetWinText(player.photonPlayer.NickName);
        //go back to menu
        Invoke("GoToMenu", 3.0f);
    }

    public void GoToMenu()
    {
        PhotonNetwork.LeaveRoom();
        NetworkManager.instance.ChangeScene("Menu");
    }




}
