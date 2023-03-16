using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UI;
using UnityEngine;
using Photon.Realtime;
using Photon.Pun;
using UnityEngine.EventSystems;
using Random = UnityEngine.Random;

public class GameMgr : MonoBehaviourPunCallbacks, IScrollHandler
{    
    public InputField input;
    [Space]
    public GameObject chatTexts;
    public GameObject chatLog;
    public RectTransform contentRect;
    public ScrollRect scroll_rect = null;
    [Space]
    public Scrollbar scroll_Bar;
    [Space]
    public int chatCount;
    [Space]
    public Queue<GameObject> chatTextQueue;
    [Space]
    public GameObject dummyQueueObject;
    //string chatters;

    private void Awake()
    {
        chatTextQueue = new Queue<GameObject>();
        chatTextQueue.Enqueue(dummyQueueObject);
    }

    void Start()
    {
        PhotonNetwork.IsMessageQueueRunning = true;
        scroll_rect = GameObject.FindObjectOfType<ScrollRect>();
        scroll_Bar = GameObject.FindObjectOfType<Scrollbar>();
        CreatePlayer();
        scroll_rect.movementType = ScrollRect.MovementType.Clamped;
        scroll_rect.scrollSensitivity = 20f;
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
        ChangeScrollValue();
        ChangeSizeContent();
        ControllTextCount();
    }

    void CreatePlayer()
    {
        Transform[] points = GameObject.Find("SpawnPoint").GetComponentsInChildren<Transform>();

        int idx = Random.Range(1, points.Length);
        PhotonNetwork.Instantiate("Player", points[idx].position, Quaternion.identity);
    }

    public void ChangeSizeContent()
    {
        contentRect.sizeDelta = new Vector2(280, chatLog.GetComponent<RectTransform>().sizeDelta.y +35f);
    }

    public void ChangeScrollValue()
    {
        chatCount = chatLog.transform.childCount;
        float comp = 320 / contentRect.sizeDelta.y;
        scroll_Bar.size = comp >= 1 ? 1 : comp;
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
    
    public void ControllTextCount()
    {
        if (chatTextQueue.Count == 51)
        {
            chatTextQueue.Dequeue();
            Destroy(chatTextQueue.First());
        }
        else
        {
            return;
        }
    }

    public void OnClickLeaveRoom()
    {
        PhotonNetwork.LeaveRoom();
    }

    public override void OnLeftRoom()
    {
        PhotonNetwork.LoadLevel("StartScene");
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        string msg = string.Format(">{0}< {1}", newPlayer.NickName, "Player Entered Room");
        photonView.RPC("ReceiveMsg", RpcTarget.OthersBuffered, msg);
        ReceiveMsg(msg);
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        string msg = string.Format(">{0}< {1}", otherPlayer.NickName, "Player Left Room");
        photonView.RPC("ReceiveMsg", RpcTarget.OthersBuffered, msg);
        ReceiveMsg(msg);
    }

    [PunRPC]
    public void ReceiveMsg(string msg)
    {
        GameObject cT =Instantiate(chatTexts, this.transform.position, Quaternion.identity);
        cT.transform.SetParent(chatLog.transform);
        cT.GetComponent<Text>().text = msg;
        chatTextQueue.Enqueue(cT);
        //chatLog.text += "\n" + msg;
        scroll_rect.verticalNormalizedPosition = 0.0f;
    }

    #region oldCode
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
    

    #endregion

    public void OnScroll(PointerEventData eventData)
    {
        scroll_rect.OnScroll(eventData);
        
    }
}