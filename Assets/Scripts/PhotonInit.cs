using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class PhotonInit : MonoBehaviourPunCallbacks
{
    [SerializeField]
    private string gameVersion = "1.0";
    [SerializeField]
    public string userId = "11";
    public byte maxPlayer = 20;



    private void Awake()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
    }

    private void Start()
    {
        PhotonNetwork.GameVersion = this.gameVersion;
        userId = PhotonNetwork.NickName;

        PhotonNetwork.ConnectUsingSettings();

    }



    public override void OnConnectedToMaster()
    {
        Debug.Log("Connect To Master");
        PhotonNetwork.JoinRandomRoom();
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.Log("Failed Join Room");
        PhotonNetwork.CreateRoom(null, new RoomOptions { MaxPlayers = this.maxPlayer });
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("Joined Room");
        //CreatPlayer();
    }

    //void CreatPlayer()
    //{
    //    Transform[] sp = GameObject.Find("SpawnPoint").GetComponentsInChildren<Transform>();

    //    int idx = Random.Range(1, sp.Length);

    //    PhotonNetwork.Instantiate("Player", sp[idx].position, Quaternion.identity);
    //}
}
