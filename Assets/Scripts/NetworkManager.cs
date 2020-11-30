using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
public class NetworkManager : MonoBehaviourPunCallbacks
{
    // Let's have this Script as a singleton, so that we can refernece it from anywhere 
    // in the project
    public static NetworkManager instance;

    private void Awake()
    {   

        // Make sure there is only one instance of this NetworkManager existing
        if (instance != null & instance != this)
            gameObject.SetActive(false);
        else
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        //Step 1: Connect to the master server of the game, using default PhotonPun's Server Settings
        PhotonNetwork.ConnectUsingSettings();
    }

    //Function to be called when user acts to create a room, 
    //arguement will be the room name set by user
    public void CreateRoom(string roomName)
    {
        //Pass the expected roomName to Photon's master server and have it create a room with
        //that name
        PhotonNetwork.CreateRoom(roomName);
    }

    //Function to be called when user already has a room in mind,
    //argument will be the exact roomname user wishes to join.
    public void JoinRoom(string roomName)
    {
        //Pass the expected roomName to Photon's master server and have it take the user to
        //that room
        PhotonNetwork.JoinRoom(roomName);
    }

    //Function to Change the Scene in the project, used specifically to take player 
    //from lobby <-> room <-> game
    public void ChangeScene(string sceneName)
    {
        //Let photon change the scene, instead of unity, so that 
        //It can make sure, nothing happens on the server while scene is loading
        PhotonNetwork.LoadLevel(sceneName);
    }

    /// <summary>
    /// Called when the client is connected to the Master Server and ready for matchmaking and other tasks.
    /// Displays a Debug.Log message, if success.
    /// </summary>    
    public override void OnConnectedToMaster()
    {
        Debug.Log("Client Connected to Master Server!");
        CreateRoom("testroom");
    }

    /// <summary>
    /// Called when the client successfully creates a room and joins.
    /// Displays a Debug.Log message with room name, if success.
    /// </summary> 
    public override void OnCreatedRoom()
    {
        Debug.Log("Room Created: " + PhotonNetwork.CurrentRoom.Name);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
