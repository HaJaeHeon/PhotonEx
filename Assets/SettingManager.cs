using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SettingManager : MonoBehaviour
{
    static public SettingManager instance;

    [SerializeField]
    List<Resolution> resolutions = new List<Resolution>();
    public GameObject ResPanel;
    public Dropdown resolutionDropdown;
    int resolutionNum;
    FullScreenMode screenMode;
    public Toggle fullscreenBtn;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(instance);
        }

        else
            Destroy(gameObject);

    }

    private void Start()
    {
        InitUI();
    }

    public void FullScreenBtn(bool isFull)
    {
        screenMode = isFull ? FullScreenMode.FullScreenWindow : FullScreenMode.Windowed;
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


        /*
        foreach(Resolution item in resolutions)
        {
            Debug.Log(item.width + "x" + item.height +" " + item.refreshRate);//refreshRate >> 화면재생빈도
        }
        */

        int optionNum = 0;

        foreach (Resolution item in resolutions)
        {
            Dropdown.OptionData option = new Dropdown.OptionData();
            option.text = item.width + "x" + item.height + " " /*+ item.refreshRate + "hz"*/;
            resolutionDropdown.options.Add(option);

            if (item.width == Screen.width && item.height == Screen.height)
                resolutionDropdown.value = optionNum;
            optionNum++;
        }
        resolutionDropdown.RefreshShownValue();

        fullscreenBtn.isOn = Screen.fullScreenMode.Equals(FullScreenMode.FullScreenWindow) ? true : false;
    }
    public void OkBtnClick()
    {
        Screen.SetResolution(resolutions[resolutionNum].width,
            resolutions[resolutionNum].height,
            fullscreenBtn.isOn);
    }

    public void ResButtonClk()
    {
        ResPanel.SetActive(true);
    }

    public void ExitButtonClk()
    {
        ResPanel.SetActive(false);
    }
}
