using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;
using Photon.Realtime;

public class Menu : MonoBehaviourPunCallbacks
{
    [Header("Screens")]
    public GameObject mainScreen;
    public GameObject lobbyScreen;

    [Header("MainScreen")]
    public Button createRoomButton;
    public Button joinRoomButton;

    [Header("LobbyScreen")]
    public TextMeshProUGUI playerListTest;
    public Button startGameButton;
    // Start is called before the first frame update
    void Start()
    {
        // disable buttons to create a room or join a room, if we are not connected to the 
        // master server 
        joinRoomButton.interactable = false;
        createRoomButton.interactable = false;
    }

    public void SetScreen(GameObject screen)
    {
        //deactivate all the screen
        mainScreen.SetActive(false);
        lobbyScreen.SetActive(false);

        //enable requested screen
        screen.SetActive(true);
    }
    public override void OnConnectedToMaster()
    {
        //Once we have a confirmation, that we are connected to the master server
        //we enable to button (functionality) to create the room or join the room. 
        joinRoomButton.interactable = true;
        createRoomButton.interactable = true;
    }
    
    // when we have confirmation that the player has join a room
    public override void OnJoinedRoom()
    {   
        //change the screen to lobby screen
        SetScreen(lobbyScreen);
        photonView.RPC("UpdateLobbyUI", RpcTarget.All);
    }

    // when we have confirmation that the player has left the room
    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        UpdateLobbyUI();
        // here we are not calling UpdateLobbyUI as an RPC call because OnPlayerLeftRoom itself is called on every player in the lobby
        //when any one player leaves a room
    }
    

    //when a player is in a room and some event happens (new player joins/leaves)
    //we call this function on every player in the room
    [PunRPC]
    public void UpdateLobbyUI()
    {   
        //intital list of players is empty
        playerListTest.text = " ";

        //add nicknames of all the players in the list
        foreach(Player player in PhotonNetwork.PlayerList)
        {
            playerListTest.text += player.NickName + "\n";
        }

        //check if the current player is the master client, if he is make Start Game Button Enable, else not.
        if (PhotonNetwork.IsMasterClient)
            startGameButton.interactable = true;
        else
            startGameButton.interactable = false;
    }


    //Functions to call when buttons are pressed.
    public void CreateRoomButtonClicked(TMP_InputField roomName)
    {
        //command to create a room on the server via network manager
        NetworkManager.instance.CreateRoom(roomName.text);
    }
    public void JoinRoomButtonClicked(TMP_InputField roomName)
    {
        //command to join a room on the server via network manager
        NetworkManager.instance.JoinRoom(roomName.text);
    }

    public void LeaveRoomButtonClicked()
    {
        PhotonNetwork.LeaveRoom();
        SetScreen(mainScreen);
    }
    public void StartButtonClicked()
    {   
        //Change the scene to Game Scene by calling Change Scene function, however call it as RPC target All.
        NetworkManager.instance.photonView.RPC("ChangeScene", RpcTarget.All, "Game");
    }

    public void PlayerNameInputFieldUpdated(TMP_InputField playerName)
    {
        PhotonNetwork.NickName = playerName.text;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
