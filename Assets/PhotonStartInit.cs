using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine.SceneManagement;

public class PhotonStartInit : MonoBehaviourPunCallbacks
{
    public enum ActivePanel
    {
        LOGIN = 0,
        ROOMS = 1
    }
    public ActivePanel activePanel = ActivePanel.LOGIN;

    private string gameVersion = "1.0";
    public string userId = "11";
    public byte maxPlayer = 20;

    public TMP_InputField txtUserId;
    public TMP_InputField txtRoomName;

    public GameObject[] panels;

    public GameObject WarningWord;

    private void Awake()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
    }
    private void Start()
    {
        txtUserId.text = PlayerPrefs.GetString("USER_ID", "USER_" + Random.Range(1, 999));
        txtRoomName.text = PlayerPrefs.GetString("ROOM_NAME", "ROOM_" + Random.Range(1, 999));
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
            PhotonNetwork.GameVersion = this.gameVersion;
            PhotonNetwork.NickName = txtUserId.text;

            PhotonNetwork.ConnectUsingSettings();

            PlayerPrefs.SetString("USER_ID", PhotonNetwork.NickName);
            ChangePanel(ActivePanel.ROOMS);
        }
    }

    [ContextMenu("MainPage")]
    public void MainPage()
    {
        PhotonNetwork.GameVersion = this.gameVersion;

        PlayerPrefs.SetString("USER_ID", null);
        txtUserId.text = PlayerPrefs.GetString("USER_ID", PhotonNetwork.NickName);
        ChangePanel(ActivePanel.LOGIN);
    }

    public void OnCreateRoomCLick()
    {
        PhotonNetwork.CreateRoom(txtRoomName.text, new RoomOptions { MaxPlayers = this.maxPlayer });
    }

    public void OnJoinRandomRoomClick()
    {
        PhotonNetwork.JoinRandomRoom();
    }


    private void ChangePanel(ActivePanel panel)
    {
        foreach (GameObject _panel in panels)
        {
            Debug.Log(panels);
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
        PhotonNetwork.CreateRoom(null, new RoomOptions { MaxPlayers = this.maxPlayer });
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("Joined room");
        PhotonNetwork.IsMessageQueueRunning = false;
        SceneManager.LoadScene("RoomScene");
    }
    
    private IEnumerator DisappearWarning()
    {
        yield return new WaitForSeconds(1.5f);
        WarningWord.SetActive((false));
    }
}
