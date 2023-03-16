using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum ActivePanel
{
    Sen = 0,
    Res = 1
}
public class BackImageColor : MonoBehaviour
{
    public Button senButton;
    public Button resButton;

    [Space]
    public GameObject[] panels;

    [Space]
    public GameObject settingsPanel;
    [Space]
    [SerializeField] List<Resolution> resolutions = new List<Resolution>();
    [Space]
    public Dropdown resolutionDropdown;
    int resolutionNum;
    FullScreenMode screenMode;
    public Toggle fullscreenBtn;
    [Space] 
    public Text SenCount;
    public Slider senSlider;

    private void Start()
    {
        ChangePanel(ActivePanel.Sen);
        InitUI();
    }

    private void Update()
    {
        OnChangeSenC();
    }

    public void OnClickSettingPanel()
    {
        settingsPanel.SetActive(true);
    }
    public void OnClickClose()
    {
        settingsPanel.SetActive(false);
    }
    
    public void OnClickSen()
    {
        ChangePanel(ActivePanel.Sen);
    }

    public void OnClickRes()
    {
        ChangePanel(ActivePanel.Res);
    }

    private void ChangePanel(ActivePanel panel)
    {
        foreach (GameObject _panel in panels)
        {
            _panel.SetActive(false);
        }
        panels[(int)panel].SetActive(true);
    }
    
    public void DropboxOptionChange(int x)
    {
        resolutionNum = x;
    }

    void InitUI()
    {
        for (int i = 0; i < Screen.resolutions.Length; i++)
        {
            if (Screen.resolutions[i].height <= 1080)
                resolutions.Add(Screen.resolutions[i]);
        }
        resolutionDropdown.options.Clear(); // options > List함수라서 초기 자료들 리셋

        int optionNum = 0;

        foreach (Resolution item in resolutions)
        {
            Dropdown.OptionData option = new Dropdown.OptionData();
            option.text = item.width + "x" + item.height;
            resolutionDropdown.options.Add(option);

            if (item.width == Screen.width && item.height == Screen.height)
                resolutionDropdown.value = optionNum;
            optionNum++;
        }
        resolutionDropdown.RefreshShownValue();

        fullscreenBtn.isOn = Screen.fullScreenMode.Equals(FullScreenMode.FullScreenWindow) ? true : false;
    }
    
    public void FullScreenBtn(bool isFull)
    {
        screenMode = isFull ? FullScreenMode.FullScreenWindow : FullScreenMode.Windowed;
    }
    public void OkBtnClick()
    {
        Screen.SetResolution(resolutions[resolutionNum].width,
            resolutions[resolutionNum].height,
            fullscreenBtn.isOn);
    }

    public void OnChangeSenC()
    {
        float temp;
        temp = senSlider.value;

        if (SenCount.gameObject.activeSelf == false)
        {
            SenCount.text = temp.ToString();
        }
        else
        {
            SenCount.text = senSlider.value.ToString();
        }
    }
}
