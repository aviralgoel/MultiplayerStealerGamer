using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;


public class PlayerController : MonoBehaviourPunCallbacks, IPunObservable
{
    [HideInInspector]
    //player ID to identify the player in the multiplayer game
    public int id;


    [Header("Player Properties")]
    public float moveSpeed;
    public float jumpForce;
    public GameObject hatObject;
    
    [HideInInspector]
    public float currentHatTime;

    [Header("Components")]
    public Rigidbody rb;
    public Player photonPlayer;

    //ray shot by player towards the groud, to check isGrounded?
    private float rayLength = 0.7f;

    [PunRPC]
    public void Initialize(Player player)
    {
        photonPlayer = player;
        id = player.ActorNumber;

        GameManager.instance.players[id - 1] = this; 

        // give the first player the hat
        if(id == 1)
        {
            GameManager.instance.GiveHat(id, true);
        }

        if (!photonView.IsMine)
            rb.isKinematic = true;
    }
    private void Update()
    {   
        //if the instance of this script is running on the master client
        //then we must keep track of hat times for the game
        if(PhotonNetwork.IsMasterClient)
        {
            if(currentHatTime >= GameManager.instance.timeToWin && !GameManager.instance.hasGameEnded)
            {
                GameManager.instance.hasGameEnded = true;
                GameManager.instance.photonView.RPC("WinTheGame", RpcTarget.All, id);
            }
        }

        // if the instance of this script is running on my pc
        // then it is my player, and I can move it
        if(photonView.IsMine)
        {
            Move();
            if (Input.GetKeyDown(KeyCode.Space))
                TryJump();

            if(hatObject.activeInHierarchy)
            {
                currentHatTime += Time.deltaTime;
            }
            
        }
       
    }

    void Move()
    {
        float h = Input.GetAxis("Horizontal") * moveSpeed;
        float v = Input.GetAxis("Vertical") * moveSpeed;

        rb.velocity = new Vector3(h, rb.velocity.y, v);


    }

    void TryJump()
    {
        Ray ray = new Ray(transform.position, Vector3.down);
        
        //if player on ground
        if(Physics.Raycast(ray, rayLength))
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
    }

    public void SetHat(bool hasHat)
    {
        hatObject.SetActive(hasHat);
    }
    private void OnCollisionEnter(Collision collision)
    {
        if(!photonView.IsMine)
        {
            return;
        }

        //Did we hit another player?
        if(collision.gameObject.CompareTag("Player"))
        {   
            //Do they have the hat?
            if(GameManager.instance.GetPlayer(collision.gameObject).id==GameManager.instance.playerWithHatID)
            {
                //Are we allowed to take that hat?
                if(GameManager.instance.CanGetHat())
                {
                    //Give us the hat
                    GameManager.instance.photonView.RPC("SetHat", RpcTarget.All, id, false);    
                }
            }
        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if(stream.IsWriting)
        {
            stream.SendNext(currentHatTime);
        }
        if(stream.IsReading)
        {
            currentHatTime = (float)stream.ReceiveNext();
        }

    }













}
