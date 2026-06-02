using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager instance;

    [Header("오디오 소스")]
    private AudioSource sfxSource;

    [Header("플레이어 효과음 등록")]
    public AudioClip buttonClickClip;
    public AudioClip playerAttackClip;
    public AudioClip playerSkillCClip; 
    public AudioClip playerSkillXClip; 
    public AudioClip playerDashClip;   

    [Header("마왕(AI) 효과음 등록")]
    public AudioClip bossAttackClip;   
    public AudioClip bossSkillCClip;   
    public AudioClip bossSkillXClip;   
    public AudioClip bossDashClip;     

    private float sfxVolume = 1f;
    private float masterVolume = 1f;

    void Awake()
    {
        if (instance == null)
        {
            // 1. 내가 첫 씬에서 태어난 진짜 원본일 때
            instance = this;
            DontDestroyOnLoad(gameObject);

            sfxSource = GetComponent<AudioSource>();
            if (sfxSource == null) sfxSource = gameObject.AddComponent<AudioSource>();
            
            sfxSource.playOnAwake = false;
            sfxSource.loop = false;

           
            if (!PlayerPrefs.HasKey("SFX_Volume") || PlayerPrefs.GetFloat("SFX_Volume") == 0)
            {
                PlayerPrefs.SetFloat("Master_Volume", 1f);
                PlayerPrefs.SetFloat("SFX_Volume", 1f);
                PlayerPrefs.Save();
            }

            LoadVolumeSettings();
        }
        else
        {

            AudioSource currentAudio = GetComponent<AudioSource>();
            if (currentAudio != null)
            {
                currentAudio.enabled = true;
                instance.sfxSource = currentAudio;
            }


            instance.LoadVolumeSettings();

          
            Destroy(gameObject);
            return;
        }
    }

    //
    public void LoadVolumeSettings()
    {
        sfxVolume = PlayerPrefs.GetFloat("SFX_Volume", 1f);
        masterVolume = PlayerPrefs.GetFloat("Master_Volume", 1f);
    }

    public void SetSFXVolume(float volume)
    {
        sfxVolume = volume;
        PlayerPrefs.SetFloat("SFX_Volume", sfxVolume);
        PlayerPrefs.Save();
    }

    public void SetMasterVolume(float volume)
    {
        masterVolume = volume;
        PlayerPrefs.SetFloat("Master_Volume", masterVolume);
        PlayerPrefs.Save();
    }

    public float GetSFXVolume() { return sfxVolume; }

    // --- 효과음 재생 함수 모음 ---
    public void PlayButtonClick() { PlaySFX(buttonClickClip); }
    public void PlayPlayerAttack() { PlaySFX(playerAttackClip); }
    public void PlayPlayerSkillC() { PlaySFX(playerSkillCClip); }
    public void PlayPlayerSkillX() { PlaySFX(playerSkillXClip); }
    public void PlayPlayerDash() { PlaySFX(playerDashClip); }
    public void PlayBossAttack() { PlaySFX(bossAttackClip); }
    public void PlayBossSkillC() { PlaySFX(bossSkillCClip); }
    public void PlayBossSkillX() { PlaySFX(bossSkillXClip); }
    public void PlayBossDash() { PlaySFX(bossDashClip); }

    // 공통 핵심 재생 로직
    private void PlaySFX(AudioClip clip)
    {

        if (instance != null && instance.sfxSource != null && clip != null)
        {

            instance.LoadVolumeSettings();


            if (!instance.sfxSource.enabled) instance.sfxSource.enabled = true;


            instance.sfxSource.PlayOneShot(clip, instance.sfxVolume * instance.masterVolume);
        }
    }
}