using System;
using System.Collections;
using System.Collections.Generic;
using ExitGames.Client.Photon;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class PhotonStartInit : MonoBehaviourPunCallbacks
{
    public enum ActivePanel
    {
        LOGIN = 0,
        ROOMS = 1,
        MATCH = 2
    }
    
    
    private string _gameVersion = "1.0";
    public string userId = "11";
    public PunLogLevel logLevel = PunLogLevel.Informational;
    public byte maxPlayer = 4;
    
    [Space]
    public ActivePanel activePanel = ActivePanel.LOGIN;
    public GameObject[] panels;
    
    [Space]
    public TMP_InputField txtUserId;

    [Space]
    public GameObject WarningWord;
    
    [Space]
    public Dropdown dropdown_RoomMaxPlayers;
    public Dropdown dropdown_RoomMaxTime;
    
    [Space]
    public Text myPropTxt;
    public Text currentMatchTxt;

    private void Awake()
    {
        PhotonNetwork.LogLevel = logLevel;
        OnJoinedLobby();
        PhotonNetwork.AutomaticallySyncScene = true;
    }
    private void Start()
    {
        txtUserId.text = PlayerPrefs.GetString("USER_ID", "USER_" + Random.Range(1, 999));
        PhotonNetwork.GameVersion = this._gameVersion;
        if (!PhotonNetwork.IsConnected)
            PhotonNetwork.ConnectUsingSettings();
        else
            return;
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

            //PhotonNetwork.ConnectUsingSettings();

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

    private void OnConnectedToServer()
    {
        Debug.Log("Connected To Server");
    }

    public override void OnConnectedToMaster()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
    }

    public void Connect()
    {
        PhotonNetwork.ConnectUsingSettings();
    }
    
    public void OnJoinRandomRoomClick()
    {
        PhotonNetwork.JoinOrCreateRoom("Room",
            new RoomOptions { MaxPlayers = this.maxPlayer, IsOpen = true, IsVisible = true }, null);
    }

    
    public override void OnCreatedRoom()
    {
        byte maxPlayers = byte.Parse(dropdown_RoomMaxPlayers.options[dropdown_RoomMaxPlayers.value].text);
        byte maxTime = byte.Parse(dropdown_RoomMaxTime.options[dropdown_RoomMaxTime.value].text);

        RoomOptions roomOptions = new RoomOptions();
        roomOptions.MaxPlayers = maxPlayers;
        roomOptions.CustomRoomProperties = new ExitGames.Client.Photon.Hashtable() { { "maxTime", maxTime } };
        roomOptions.CustomRoomPropertiesForLobby = new string[] { "maxTime" };

        PhotonNetwork.CreateRoom(null, roomOptions);
    }

    public void JoinRandomOrCreateRoom()
    {
        byte maxPlayers = byte.Parse(dropdown_RoomMaxPlayers.options[dropdown_RoomMaxPlayers.value].text);
        byte maxTime = byte.Parse(dropdown_RoomMaxTime.options[dropdown_RoomMaxTime.value].text);

        RoomOptions roomOptions = new RoomOptions();
        roomOptions.MaxPlayers = maxPlayers;
        roomOptions.CustomRoomProperties = new ExitGames.Client.Photon.Hashtable() { { "maxTime", maxTime } };
        roomOptions.CustomRoomPropertiesForLobby = new string[] { "maxTIme" };

        PhotonNetwork.JoinRandomOrCreateRoom(
            expectedCustomRoomProperties: new ExitGames.Client.Photon.Hashtable() { { "maxTime", maxTime } },
            expectedMaxPlayers: maxPlayers,
            roomOptions: roomOptions);
    }

    public void ClickMatchingButton()
    {
        byte maxPlayers = byte.Parse(dropdown_RoomMaxPlayers.options[dropdown_RoomMaxPlayers.value].text);
        byte maxTime = byte.Parse(dropdown_RoomMaxTime.options[dropdown_RoomMaxTime.value].text);

        RoomOptions roomOptions = new RoomOptions();
        roomOptions.MaxPlayers = maxPlayers;
        roomOptions.CustomRoomProperties = new ExitGames.Client.Photon.Hashtable() { { "maxTime", maxTime } };
        roomOptions.CustomRoomPropertiesForLobby = new string[] { "maxTime" };

        PhotonNetwork.JoinRandomOrCreateRoom(
            expectedCustomRoomProperties: new ExitGames.Client.Photon.Hashtable() { { "maxTime", maxTime } },
            expectedMaxPlayers: maxPlayers,
            roomOptions: roomOptions
        );
    }

    public void ClickCancelMatch()
    {     
        ChangePanel(ActivePanel.ROOMS);
        PhotonNetwork.LeaveRoom();
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

    

    private void UpdatePlayerCount()
    {
        currentMatchTxt.text = $"{PhotonNetwork.CurrentRoom.PlayerCount} / {PhotonNetwork.CurrentRoom.MaxPlayers}";
    }
    
    public override void OnJoinedRoom()
    {
        //PhotonNetwork.IsMessageQueueRunning = false;
        
        ChangePanel(ActivePanel.MATCH);
        
        byte maxPlayers = byte.Parse(dropdown_RoomMaxPlayers.options[dropdown_RoomMaxPlayers.value].text);
        byte maxTime = byte.Parse(dropdown_RoomMaxTime.options[dropdown_RoomMaxTime.value].text);

        myPropTxt.text = "MaxPlayer : " + maxPlayers.ToString() + " \n " + "PlayTime : " + maxTime.ToString();
        //currentMatchTxt.text = $"{PhotonNetwork.CurrentRoom.PlayerCount} / {PhotonNetwork.CurrentRoom.MaxPlayers}";
        UpdatePlayerCount();
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        UpdatePlayerCount();
        
        if (PhotonNetwork.IsMasterClient)
        {
            if(PhotonNetwork.CurrentRoom.PlayerCount == PhotonNetwork.CurrentRoom.MaxPlayers)
                PhotonNetwork.LoadLevel("RoomScene");
        }
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        UpdatePlayerCount();
    }

   
    public override void OnDisconnected(DisconnectCause cause)
    {
        Debug.Log("Disconnected");
    }
    
    private IEnumerator DisappearWarning()
    {
        yield return new WaitForSeconds(1.5f);
        WarningWord.SetActive((false));
    }

    #region not use method
    

    //public void JoinOrCreateRoom()
    //{
    //    PhotonNetwork.LocalPlayer.NickName = txtUserId.text;
    //
    //    RoomOptions roomOptions = new RoomOptions();
    //    roomOptions.MaxPlayers = maxPlayer;
    //    roomOptions.IsOpen = true;
    //    roomOptions.IsVisible = true;
    //    
    //   
    //}
    
    //public void OnCreateRoomCLick()
    //{
    //    //PhotonNetwork.Instantiate("RoomButton", RoomButton.transform.position, Quaternion.identity);
    //
    //    PhotonNetwork.CreateRoom(txtRoomName.text, new RoomOptions { MaxPlayers = this.maxPlayer,
    //                                                                 IsVisible = true,
    //                                                                 IsOpen = true});
    //    RoomName.text = txtRoomName.text;
    //    
    //    string rName = txtRoomName.text;
    //    //SendRaiseEvent(EVENTCODE.INSTANCIATE_BUTTON, new object[1], SEND_OPTION.OTHER);
    //    //photonView.RPC("MakeRoom", RpcTarget.OthersBuffered, rName);
    //    //MakeRoom(RoomName.text);
    //}

    

    #endregion
    
    #region FailRoom
    //public override void OnJoinRoomFailed(short returnCode, string message)
    //{
    //    PhotonNetwork.CreateRoom(null, new RoomOptions
    //    {
    //        MaxPlayers = this.maxPlayer,
    //        IsOpen = true,
    //        IsVisible = true
    //    });
    //}
    //public override void OnJoinRandomFailed(short returnCode, string message)
    //{
    //    Debug.Log("Failed join room");
    //    PhotonNetwork.CreateRoom(null, new RoomOptions { MaxPlayers = this.maxPlayer, 
    //        IsOpen = true,
    //        IsVisible = true });
    //}
    #endregion

    #region NotUseSendRaiseEvent

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

    #endregion

    #region notUseRPC

    //[PunRPC]
    //public void MakeRoom(string rName)
    //{
    //    RoomName.text = rName;
    //    Instantiate(RoomButton).transform.SetParent(RoomList.transform);
    //}

    #endregion
    
}
