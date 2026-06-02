using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro; 
using System.Collections;

public class CutsceneManager : MonoBehaviour
{
    [System.Serializable]
    public class CutsceneData
    {
        public Sprite cutsceneImage;  // 스토리 일러스트
        [TextArea(3, 5)]
        public string dialogueText;   // 대사 내용
        public AudioClip voiceClip;   // 이 대사가 나올 때 재생할 목소리 사운드 파일
    }

    [Header("컷씬 데이터 설정")]
    public CutsceneData[] cutscenes;  
    public string nextSceneName = "Battle"; 

    [Header("UI 컴포넌트 연결")]
    public Image displayImage;        
    public TMP_Text dialogueTextDisplay; 
    public GameObject cutscenePanel;  

    // 목소리를 직접 내뿜어줄 스피커 컴포넌트 변수
    private AudioSource voiceAudioSource;

    private int currentIndex = 0;
    private bool isTyping = false;
    private string currentFullText = "";

    void Start()
    {
        // 내 몸에 내장된 오디오 소스를 가져오거나 없으면 새로 달아줍니다.
        voiceAudioSource = GetComponent<AudioSource>();
        if (voiceAudioSource == null) voiceAudioSource = gameObject.AddComponent<AudioSource>();
        
        // 대사 전용 스피커이므로 자동 재생 및 반복 재생은 확실하게 꺼둡니다.
        voiceAudioSource.playOnAwake = false;
        voiceAudioSource.loop = false;

        if (cutscenes.Length > 0)
        {
            cutscenePanel.SetActive(true);
            ShowCutscene(0);
        }
        else
        {
            StartGame();
        }
    }

    void Update()
    {
        // 마우스 클릭, 스페이스바, Z키로 넘기기
        if (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Z))
        {
            if (isTyping)
            {
                // 기존에 글자 타이핑만 멈추던 걸 넘어, 목소리 사운드도 즉시 멈춰서 씽크를 맞춥니다!
                if (voiceAudioSource != null) voiceAudioSource.Stop();

                StopAllCoroutines();
                dialogueTextDisplay.text = currentFullText;
                isTyping = false;
            }
            else
            {
                NextCutscene();
            }
        }
    }

    void ShowCutscene(int index)
    {
        currentIndex = index;
        currentFullText = cutscenes[index].dialogueText;

        if (cutscenes[index].cutsceneImage != null)
        {
            displayImage.gameObject.SetActive(true);
            displayImage.sprite = cutscenes[index].cutsceneImage;
        }
        else
        {
            displayImage.gameObject.SetActive(false);
        }

        // 다음 대사 소리를 틀기 전에, 현재 스피커에서 혹시 흘러나오고 있는 소리가 있다면 깔끔하게 커트합니다.
        if (voiceAudioSource != null)
        {
            voiceAudioSource.Stop();
            
            // 만약 이번 대사에 등록해 놓은 목소리 사운드 파일이 있다면 장착하고 즉시 플레이!
            if (cutscenes[index].voiceClip != null)
            {
                voiceAudioSource.clip = cutscenes[index].voiceClip;
                voiceAudioSource.Play();
                Debug.Log($"🗣[] 대사 사운드 출력 시작: {cutscenes[index].voiceClip.name}");
            }
        }

        StartCoroutine(TypeTextRoutine(currentFullText));

        if (SoundManager.instance != null) SoundManager.instance.PlayButtonClick();
    }

    IEnumerator TypeTextRoutine(string text)
    {
        isTyping = true;
        dialogueTextDisplay.text = "";

        foreach (char letter in text.ToCharArray())
        {
            dialogueTextDisplay.text += letter;
            yield return new WaitForSeconds(0.04f);
        }

       
        if (voiceAudioSource != null)
        {
            voiceAudioSource.Stop();
            Debug.Log("📝 텍스트 타이핑 완료 -> 대사 사운드 강제 정지");
        }

        isTyping = false;
    }

    void NextCutscene()
    {
        int nextIndex = currentIndex + 1;

        if (nextIndex < cutscenes.Length)
        {
            ShowCutscene(nextIndex);
        }
        else
        {
            StartGame();
        }
    }

    void StartGame()
    {
    
        if (voiceAudioSource != null) voiceAudioSource.Stop();

        if (cutscenePanel != null) cutscenePanel.SetActive(false);
        SceneManager.LoadScene(nextSceneName);
    }
}