//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using Photon.Pun;
//using Photon.Realtime;
//using UnityEngine.UI;

//public class GameMgr : MonoBehaviourPunCallbacks
//{
//    public Text msgList;
//    public InputField ifSendMsg;

//    private void Start()
//    {
//        CreatePlayer();
//        PhotonNetwork.IsMessageQueueRunning = true;        
//    }

//    void CreatePlayer()
//    {
//        Transform[] points = GameObject.Find("SpawnPoint").GetComponentsInChildren<Transform>();

//        int idx = Random.Range(1, points.Length);
//        PhotonNetwork.Instantiate("Player", points[idx].position, Quaternion.identity);
//    }

//    public void OnSendChatMsg()
//    {
//        string msg = string.Format("[{0}] {1}"
//                                  , PhotonNetwork.LocalPlayer.NickName
//                                  , ifSendMsg.text);
//        photonView.RPC("ReceiveMsg", RpcTarget.OthersBuffered, msg);
//        ReceiveMsg(msg);

//        //    ifSendMsg.text = "";
//        //    ifSendMsg.ActivateInputField();
//    }

//    [PunRPC]
//    void ReceiveMsg(string msg)
//    {
//        msgList.text += "\n" + msg;
//    }


//}

using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using Photon.Realtime;
using Photon.Pun;

public class GameMgr : MonoBehaviourPunCallbacks
{
    public List<string> chatList = new List<string>();
    //public Button sendBtn;
    public Text chatLog;
    public Text chattingList;
    public InputField input;
    ScrollRect scroll_rect = null;
    //string chatters;
    // Start is called before the first frame update



    void Start()
    {
        PhotonNetwork.IsMessageQueueRunning = true;
        scroll_rect = GameObject.FindObjectOfType<ScrollRect>();
        CreatePlayer();
    }

    public void SendButtonOnClicked()
    {
        if (input.text.Equals("")) { Debug.Log("Empty"); return; }
        string msg = string.Format("[{0}] {1}", PhotonNetwork.LocalPlayer.NickName, input.text);
        photonView.RPC("ReceiveMsg", RpcTarget.OthersBuffered, msg);
        ReceiveMsg(msg);
        input.ActivateInputField(); // 반대는 input.select(); (반대로 토글)
        input.text = "";
    }

    void Update()
    {
        //chatterUpdate();
        if (Input.GetKeyDown(KeyCode.Return) && !input.isFocused) SendButtonOnClicked();
    }

    void CreatePlayer()
    {
        Transform[] points = GameObject.Find("SpawnPoint").GetComponentsInChildren<Transform>();

        int idx = Random.Range(1, points.Length);
        PhotonNetwork.Instantiate("Player", points[idx].position, Quaternion.identity);
    }

    //void chatterUpdate()
    //{
    //    chatters = "Player List\n";
    //    foreach (Player p in PhotonNetwork.PlayerList)
    //    {
    //        chatters += p.NickName + "\n";
    //    }
    //    chattingList.text = chatters;
    //}

    [PunRPC]
    public void ReceiveMsg(string msg)
    {
        chatLog.text += "\n" + msg;
        scroll_rect.verticalNormalizedPosition = 0.0f;
    }
}
