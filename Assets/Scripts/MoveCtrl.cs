using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityStandardAssets.Utility;
using TMPro;
using UnityEngine.UI;
using System;

public class MoveCtrl : MonoBehaviourPunCallbacks, IPunObservable
{
    [SerializeField]
    private Rigidbody rb;
    [SerializeField]
    private Transform tr;
    [SerializeField]
    private float h, v, mouseX;

    public float mouseSen =1f;

    public float speed = 10f;
    public float rotSpeed = 100f;
    public TextMeshPro nickName;

    public Slider senSl;

    private void Start()
    {
        if(photonView.IsMine)
        {
            Camera.main.GetComponent<SmoothFollow>().target = gameObject.transform.GetChild(0).transform.GetChild(0).transform;
        }
        else
        {
            GetComponent<Rigidbody>().isKinematic = true;
        }
        rb = gameObject.GetComponent<Rigidbody>();
        tr = gameObject.GetComponent<Transform>();

        nickName.text = photonView.Owner.NickName;

        senSl = GameObject.FindWithTag("Slider")
            .GetComponent<Slider>(); //GameObject.FindGameObjectsWithTag("Slider")GetComponent<Slider>();
    }

    private void Update()
    {
        if(photonView.IsMine)
        {
            v = Input.GetAxis("Vertical");
            h = Input.GetAxis("Horizontal");
            mouseX = Input.GetAxis("Mouse X");

            tr.Translate(Vector3.forward * v * speed * Time.deltaTime);
            tr.Translate(Vector3.left * -h * speed * Time.deltaTime);
            tr.Rotate(Vector3.up * mouseX * mouseSen* rotSpeed * Time.deltaTime);
            
            OnChangeSen();
        }
        else
        {
            if((tr.position - currPos).sqrMagnitude >= 10f * 10f)
            {
                tr.position = currPos;
                tr.rotation = currRot;
            }
            else
            {
                tr.position = Vector3.Lerp(tr.position, currPos, Time.deltaTime * 10f);
                tr.rotation = Quaternion.Slerp(tr.rotation, currRot, Time.deltaTime * 10f);
            }
        }
    }

    string GetNickNameByActorNumber(int actorNumber)
    {
        foreach(Player player in PhotonNetwork.PlayerListOthers)
        {
            if(player.ActorNumber == actorNumber)
            {
                return player.NickName;
            }
        }
        return "AI";
    }

    private Vector3 currPos;
    private Quaternion currRot;
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if(stream.IsWriting)
        {
            stream.SendNext(tr.position);
            stream.SendNext(tr.rotation);
        }
        else
        {
            currPos = (Vector3)stream.ReceiveNext();
            currRot = (Quaternion)stream.ReceiveNext();
        }
    }

    public void OnChangeSen()
    {
        senSl.maxValue = 5f;
        senSl.minValue = 0.5f;
        float temp = senSl.value;

        if (senSl.gameObject.activeSelf == false)
        {
            mouseSen = temp;
        }
        else
        {
            mouseSen = senSl.value;

        }
    }
}
