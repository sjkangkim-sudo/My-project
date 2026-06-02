using UnityEngine;
using TMPro;
using System.Collections;

public class BattleUI : MonoBehaviour
{
    [Header("텍스트 UI")]
    public TextMeshProUGUI roundText;
    public TextMeshProUGUI timeText;

    [Header("패널 설정")]
    public GameObject mainPauseMenu; 
    public GameObject settingPanel;  

    [Header("세팅 내부 탭 설정")]
    public GameObject soundContent; 
    public GameObject keyContent;   

    [Header("★ 배틀 시작 대사 사운드 등록")]
    public AudioClip heroStartVoice; 
    public AudioClip bossStartVoice; 

    public int currentRound = 1;
    private float elapsedTime = 0f;
    private bool isPaused = false;

    void Start()
    {
        Time.timeScale = 1f; 

        // [★ 핵심] 다시하기로 넘어왔을 때 Missing 난 선들을 하이어라키에서 이름으로 자동 검색해서 연결합니다!
        if (roundText == null) roundText = GameObject.Find("RoundText")?.GetComponent<TextMeshProUGUI>();
        if (timeText == null) timeText = GameObject.Find("TimeText")?.GetComponent<TextMeshProUGUI>();
        
        if (mainPauseMenu == null) mainPauseMenu = GameObject.Find("MainPauseMenu");
        if (settingPanel == null) settingPanel = GameObject.Find("SettingPanel");
        if (soundContent == null) soundContent = GameObject.Find("SoundContent");
        if (keyContent == null) keyContent = GameObject.Find("KeyContent");

        if (BattleDialogueManager.instance != null)
        {
            BattleDialogueManager.instance.ForceCloseDialogue();
        }

        if (mainPauseMenu != null) mainPauseMenu.SetActive(false);
        if (settingPanel != null) settingPanel.SetActive(false);
        isPaused = false;

        StartCoroutine(StartBattleDialogueRoutine());
    }

    IEnumerator StartBattleDialogueRoutine()
    {
        yield return new WaitForSecondsRealtime(0.5f);
        
        if (BattleDialogueManager.instance != null)
        {
            BattleDialogueManager.instance.PlayDialogue("용사", "널 쓰러뜨리고 제국을 되찾겠다!", heroStartVoice);
            BattleDialogueManager.instance.PlayDialogue("마왕", "한 번 해볼테면 해봐.", bossStartVoice);
        }
        else
        {
            Debug.LogWarning("⚠️ 씬에 BattleDialogueManager가 없습니다! 대사 연출을 스킵합니다.");
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused) 
            {
                CloseAllMenus();
            }
            else 
            {
                OpenMainMenu();
            }
        }

        if (!isPaused)
        {
            elapsedTime += Time.deltaTime;
            int minutes = Mathf.FloorToInt(elapsedTime / 60f);
            int seconds = Mathf.FloorToInt(elapsedTime % 60f);
            
            if (timeText != null)
            {
                timeText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
            }
        }
    }

    public void ClickReturnToBattleButton()
    {
        CloseAllMenus();
    }

    public void NextRound()
    {
        currentRound++;
        if (roundText != null)
            roundText.text = "Round " + currentRound;
    }

    public void OpenMainMenu()
    {
        if (mainPauseMenu != null)
        {
            mainPauseMenu.SetActive(true);
            if (settingPanel != null) settingPanel.SetActive(false);
            
            isPaused = true;
            Time.timeScale = 0f; 
        }
    }

    public void OpenSetting()
    {
        if (settingPanel != null)
        {
            settingPanel.SetActive(true);
            if (mainPauseMenu != null) mainPauseMenu.SetActive(false);

            OpenSoundTab(); 
        }
    }

    public void OpenKeyTab()
    {
        if (soundContent != null) soundContent.SetActive(false);
        if (keyContent != null) keyContent.SetActive(true);
    }

    public void OpenSoundTab()
    {
        if (soundContent != null) soundContent.SetActive(true);
        if (keyContent != null) keyContent.SetActive(false);
    }

    public void CloseAllMenus()
    {
        if (mainPauseMenu != null) mainPauseMenu.SetActive(false);
        if (settingPanel != null) settingPanel.SetActive(false);
        
        isPaused = false;
        Time.timeScale = 1f; 
    }
}