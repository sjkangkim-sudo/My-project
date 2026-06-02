using UnityEngine;

public class MainSoundManager : MonoBehaviour
{
        public static MainSoundManager Instance { get; private set; }

    [Header("Audio Sources")]
    public AudioSource bgmSource; // 메인 화면 배경음악 소스
    public AudioSource sfxSource; // 메인 화면 효과음 소스 (필요시 추가)

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        // 씬이 켜질 때 기존 저장된 볼륨을 오디오 소스에 즉시 적용
        if (bgmSource != null) bgmSource.volume = PlayerPrefs.GetFloat("BGM_Volume", 1f);
        if (sfxSource != null) sfxSource.volume = PlayerPrefs.GetFloat("SFX_Volume", 1f);
    }

    // BGM 오디오 소스 볼륨 변경
    public void SetBGMVolume(float volume)
    {
        if (bgmSource != null)
        {
            bgmSource.volume = volume;
        }
    }

    // SFX 오디오 소스 볼륨 변경
    public void SetSFXVolume(float volume)
    {
        if (sfxSource != null)
        {
            sfxSource.volume = volume;
        }
    }
}