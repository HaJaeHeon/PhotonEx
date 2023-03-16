using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EscCtrl : MonoBehaviour
{
    public GameObject escPanel;
    public bool panelCheck = true;
    
    private void Start()
    {
        
    }

    private void Update()
    {
        EscPanelOnOff();
    }

    public void EscPanelOnOff()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && panelCheck)
        {
            if (escPanel.activeSelf == true)
            {
                panelCheck = false;
                escPanel.SetActive(false);
                StartCoroutine(InputEsc());
            }
            else
            {
                panelCheck = false;
                escPanel.SetActive(true);
                StartCoroutine(InputEsc());
            }
        }
    }

    IEnumerator InputEsc()
    {
        yield return new WaitForSeconds(0.25f);
        panelCheck = true;
    }
}
