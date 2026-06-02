using UnityEngine;
using UnityEngine.SceneManagement;

public class BGMManager : MonoBehaviour
{
    public static BGMManager instance; 
    private AudioSource audioSource;

    [Header("음악 설정")]
    public AudioClip menuMusic;
    public AudioClip battleMusic;
    public AudioClip StoryMusic;

    private float masterVolume = 1f;
    private float bgmVolume = 1f;

    void Awake()
    {
        if (instance == null)
        {

            instance = this;
            DontDestroyOnLoad(gameObject);
            
            audioSource = GetComponent<AudioSource>();
            if (audioSource == null) audioSource = gameObject.AddComponent<AudioSource>();
            

            if (!PlayerPrefs.HasKey("BGM_Volume") || PlayerPrefs.GetFloat("BGM_Volume") == 0)
            {
                PlayerPrefs.SetFloat("Master_Volume", 1f);
                PlayerPrefs.SetFloat("BGM_Volume", 1f);
                PlayerPrefs.Save();
            }

            LoadVolumeSettings();
        }
        else
        {
            string currentSceneName = SceneManager.GetActiveScene().name.ToLower();

            // 설정 씬 등 브금이 끊기지 않아야 하는 곳에서는 그냥 조용히 자기 자신만 파괴
            if (currentSceneName.Contains("setting"))
            {
                Destroy(gameObject);
                return;
            }


            if (instance.audioSource == null)
            {
                instance.audioSource = instance.GetComponent<AudioSource>();
                if (instance.audioSource == null) instance.audioSource = instance.gameObject.AddComponent<AudioSource>();
            }


            instance.SyncBGMOnSceneChange(SceneManager.GetActiveScene().name, false);


            Destroy(gameObject);
            return;
        }
    }

    void OnEnable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (instance == this)
        {
            // 씬이 바뀔 때는 컴퓨터 장부를 다시 긁어오도록 true 처리
            SyncBGMOnSceneChange(scene.name, true);
        }
    }

    public void SyncBGMOnSceneChange(string sceneName, bool loadVolume)
    {
        if (loadVolume)
        {
            LoadVolumeSettings();
        }

        string lowerSceneName = sceneName.ToLower();
        Debug.Log("현재 로드된 씬 감지 (최종 소문자화): " + lowerSceneName);

        if (lowerSceneName.Contains("battle"))
        {
            ChangeBGM(battleMusic);
        }
        else if (lowerSceneName.Contains("storycutscene1"))
        {
            ChangeBGM(StoryMusic);
        }
        else if (lowerSceneName.Contains("mainmenu") || lowerSceneName.Contains("setting")) 
        {
            ChangeBGM(menuMusic);
        }
    }

    public void LoadVolumeSettings()
    {
        masterVolume = PlayerPrefs.GetFloat("Master_Volume", 1f);
        bgmVolume = PlayerPrefs.GetFloat("BGM_Volume", 1f);
        ApplyVolume();
    }

    void ChangeBGM(AudioClip newClip)
    {

        if (audioSource == null) audioSource = GetComponent<AudioSource>();
        if (audioSource == null || newClip == null) return;

        if (!audioSource.enabled) audioSource.enabled = true;
        

        if (audioSource.clip == newClip && audioSource.isPlaying)
        {
            ApplyVolume();
            return;
        }

        audioSource.Stop();
        audioSource.clip = newClip;
        audioSource.Play();
        ApplyVolume();
        Debug.Log("BGM 교체 및 재생 성공: " + newClip.name);
    }

    public void SetBGMVolume(float volume)
    {
        bgmVolume = volume;
        PlayerPrefs.SetFloat("BGM_Volume", bgmVolume);
        PlayerPrefs.Save();
        ApplyVolume();
    }

    public void SetMasterVolume(float volume)
    {
        masterVolume = volume;
        PlayerPrefs.SetFloat("Master_Volume", masterVolume);
        PlayerPrefs.Save();
        ApplyVolume();
    }

    public void ApplyVolume()
    {
        if (audioSource != null)
        {
            audioSource.volume = bgmVolume * masterVolume;
        }
    }
}