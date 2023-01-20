using System;
using System.Collections;
using System.Collections.Generic;
using ExitGames.Client.Photon;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Button = UnityEngine.UIElements.Button;
using Random = UnityEngine.Random;

public class PhotonStartInit : MonoBehaviourPunCallbacks
{
    public enum ActivePanel
    {
        LOGIN = 0,
        ROOMS = 1,
        LOBBY = 2
    }
    
    public ActivePanel activePanel = ActivePanel.LOGIN;

    private string _gameVersion = "1.0";
    public string userId = "11";
    public PunLogLevel logLevel = PunLogLevel.Informational;
    public byte maxPlayer = 4;

    public TMP_InputField txtUserId;
    public TMP_InputField txtRoomName;

    public GameObject[] panels;

    public GameObject WarningWord;

    public GameObject RoomList;
    
    private GameObject RoomButton;
    [SerializeField] private Text RoomName;

    //private readonly byte CreateRoomButtonEvent = 0;

    private void Awake()
    {
        PhotonNetwork.LogLevel = logLevel;
        PhotonNetwork.AutomaticallySyncScene = true;
        OnJoinedLobby();
    }
    private void Start()
    {
        txtUserId.text = PlayerPrefs.GetString("USER_ID", "USER_" + Random.Range(1, 999));
        txtRoomName.text = PlayerPrefs.GetString("ROOM_NAME", "ROOM_" + Random.Range(1, 999));
        RoomButton = Resources.Load<GameObject>("Room");
        RoomName = Resources.Load<GameObject>("Room").GetComponentInChildren<Text>();
    }

    [ContextMenu("Join")]
    public void OnLogin()
    {
        PhotonNetwork.NickName = txtUserId.text;
        PlayerPrefs.SetString("USER_ID", PhotonNetwork.NickName);

        if (PhotonNetwork.NickName.Equals(""))
        {
            WarningWord.SetActive(true);
            StartCoroutine("DisappearWarning");
        }
        else
        {
            PhotonNetwork.GameVersion = this._gameVersion;
            PhotonNetwork.NickName = txtUserId.text;

            PhotonNetwork.ConnectUsingSettings();

            PlayerPrefs.SetString("USER_ID", PhotonNetwork.NickName);
            ChangePanel(ActivePanel.ROOMS);
        }
    }

    [ContextMenu("MainPage")]
    public void MainPage()
    {
        PhotonNetwork.GameVersion = this._gameVersion;

        PlayerPrefs.SetString("USER_ID", null);
        txtUserId.text = PlayerPrefs.GetString("USER_ID", PhotonNetwork.NickName);
        ChangePanel(ActivePanel.LOGIN);
    }

    public void OnCreateRoomCLick()
    {
        PhotonNetwork.Instantiate("RoomButton", RoomButton.transform.position, Quaternion.identity);

        PhotonNetwork.CreateRoom(txtRoomName.text, new RoomOptions { MaxPlayers = this.maxPlayer,
                                                                     IsVisible = true,
                                                                     IsOpen = true});
        RoomName.text = txtRoomName.text;
        
        string rName = txtRoomName.text;
        //SendRaiseEvent(EVENTCODE.INSTANCIATE_BUTTON, new object[1], SEND_OPTION.OTHER);
        //photonView.RPC("MakeRoom", RpcTarget.OthersBuffered, rName);
        //MakeRoom(RoomName.text);
    }

    public void OnJoinRandomRoomClick()
    {
        PhotonNetwork.JoinRandomRoom();
    }


    private void ChangePanel(ActivePanel panel)
    {
        foreach (GameObject _panel in panels)
        {
            //Debug.Log(panels);
            _panel.SetActive(false);
        }
        panels[(int)panel].SetActive(true);
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("Connect To Master");
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.Log("Failed join room");
        PhotonNetwork.CreateRoom(null, new RoomOptions { MaxPlayers = this.maxPlayer, 
                                                         IsOpen = true,
                                                         IsVisible = true });
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("Joined room");
        PhotonNetwork.IsMessageQueueRunning = false;
        SceneManager.LoadScene("RoomScene");
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        Debug.Log("Disconnected");
    }

    public override void OnJoinedLobby()
    {
        Debug.Log("Joined Lobby");
    }
    
    private IEnumerator DisappearWarning()
    {
        yield return new WaitForSeconds(1.5f);
        WarningWord.SetActive((false));
    }

    //public enum EVENTCODE
    //{
    //    INSTANCIATE_BUTTON = 0,
    //    ADD_PLAYER = 1,
    //    SUB_PLAYER = 2
    //}
    //
    //public enum SEND_OPTION
    //{
    //    OTHER,
    //    ALL,
    //    MASTER
    //}
    //
    //public void SendRaiseEvent(EVENTCODE eventCode, object[] datas, SEND_OPTION sendOption = SEND_OPTION.OTHER)
    //{
    //    string DebugStr = string.Empty;
    //    DebugStr = "[SEND__" + eventCode.ToString() + "]";
    //    for (int i = 0; i < datas.Length; ++i)
    //    {
    //        DebugStr += "_" + datas[i];
    //    }
    //    Debug.LogError(DebugStr);
    //
    //    RaiseEventOptions raiseEventOptions = new RaiseEventOptions
    //    {
    //        Receivers = (ReceiverGroup)sendOption,
    //    };
    //    PhotonNetwork.RaiseEvent((byte)eventCode, datas, raiseEventOptions, SendOptions.SendReliable);
    //}
    //
    //private void EventReceived(EventData photonEvent)
    //{
    //    int code = photonEvent.Code;
    //
    //    if (code == (int)EVENTCODE.INSTANCIATE_BUTTON)
    //    {
    //        object[] datas = (object[])photonEvent.CustomData;
    //        Debug.LogError("RECV__EVENTCODE>INSTANTIATE_BUTTON__" + datas[0] + "_" + datas[1]);
    //        //this.InstantiateButton((string)datas[0], (float)datas[1]);
    //    }
    //    else if (code == (int)EVENTCODE.ADD_PLAYER)
    //    {
    //        
    //    }
    //    else if (code == (int)EVENTCODE.SUB_PLAYER)
    //    {
    //        
    //    }
    //}

    [PunRPC]
    public void MakeRoom(string rName)
    {
        RoomName.text = rName;
        Instantiate(RoomButton).transform.SetParent(RoomList.transform);
    }
}
