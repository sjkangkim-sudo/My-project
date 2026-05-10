using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SettingManager : MonoBehaviour
{
    [Header("화면 설정")]
    public TMP_Dropdown screenModeDropdown;

    [Header("음량 설정")]
    public Slider masterSlider;
    public Slider sfxSlider;
    public Slider bgmSlider;

    private AudioSource bgmSource;

    void Start()
    {
        if (screenModeDropdown != null)
        {
            screenModeDropdown.value = Screen.fullScreen ? 0 : 1;
        }

        BGMManager bgmManager = FindObjectOfType<BGMManager>();
        if (bgmManager != null)
        {
            bgmSource = bgmManager.GetComponent<AudioSource>();
            
            if (bgmSlider != null && bgmSource != null)
            {
                bgmSlider.value = bgmSource.volume;
            }
        }

        if (masterSlider != null) masterSlider.value = AudioListener.volume;
    }

    // --- 화면 설정 ---
    public void SetScreenMode(int index)
    {
        if (index == 0) Screen.fullScreenMode = FullScreenMode.FullScreenWindow;
        else if (index == 1) Screen.fullScreenMode = FullScreenMode.Windowed;
    }

    // --- 전체 음량 ---
    public void SetMasterVolume(float volume) 
    { 
        AudioListener.volume = volume; 
    }

    // --- BGM 조절 ---
    public void SetBGMVolume(float volume) 
    { 
        if (bgmSource != null)
        {
            bgmSource.volume = volume;
        }
        Debug.Log("BGM 볼륨 조절 중: " + volume);
    }

    // --- 효과음 조절 ---
    public void SetSFXVolume(float volume) 
    { 
        PlayerPrefs.SetFloat("SFXVolume", volume);
        Debug.Log("효과음 볼륨 조절 중: " + volume);
    }
}