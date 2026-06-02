using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainSettingsUI : MonoBehaviour
{
    [Header("Audio Sliders")]
    public Slider masterSlider; 
    public Slider bgmSlider;
    public Slider sfxSlider;

    // 패널이 켜질 때마다 무조건 슬라이더 바 위치를 동기화합니다.
    void OnEnable()
    {
        SyncSliders();
    }

    void Start()
    {
        SyncSliders();
    }

    public void SyncSliders()
    {
        // [★ 수정] 저장 데이터가 아예 없는 최초 실행 시, 0이 아닌 1f(100%) 볼륨으로 슬라이더 바 위치를 채웁니다.
        float currentMaster = PlayerPrefs.GetFloat("Master_Volume", 1f);
        float currentBGM = PlayerPrefs.GetFloat("BGM_Volume", 1f);
        float currentSFX = PlayerPrefs.GetFloat("SFX_Volume", 1f);

        if (masterSlider != null) masterSlider.value = currentMaster;
        if (bgmSlider != null) bgmSlider.value = currentBGM;
        if (sfxSlider != null) sfxSlider.value = currentSFX;

        // 영원히 살아남은 진짜 원본 사운드 매니저들의 장부를 최신화시킵니다.
        if (BGMManager.instance != null) BGMManager.instance.LoadVolumeSettings();
        if (SoundManager.instance != null) SoundManager.instance.LoadVolumeSettings();
    }

    // [★ 핵심 수정] 슬라이더 바를 조작하는 "그 실시간"에 가짜가 아닌 진짜 원본 매니저를 다이렉트로 관통 제어합니다.
    public void OnMasterVolumeChanged(float value)
    {
        if (BGMManager.instance != null) BGMManager.instance.SetMasterVolume(value);
        if (SoundManager.instance != null) SoundManager.instance.SetMasterVolume(value);
    }

    public void OnBGMVolumeChanged(float value)
    {
        if (BGMManager.instance != null) BGMManager.instance.SetBGMVolume(value);
    }

    public void OnSFXVolumeChanged(float value)
    {
        if (SoundManager.instance != null) SoundManager.instance.SetSFXVolume(value);
    }

    public void OnBackButtonClicked()
    {
        SceneManager.LoadScene("mainmenu");
    }
}