using UnityEngine;
using TMPro;

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

    public int currentRound = 1;
    private float elapsedTime = 0f;
    private bool isPaused = false;

    void Start()
    {
        if (mainPauseMenu != null) mainPauseMenu.SetActive(false);
        if (settingPanel != null) settingPanel.SetActive(false);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused) CloseAllMenus();
            else OpenMainMenu();
        }

        if (!isPaused)
        {
            elapsedTime += Time.deltaTime;
            int minutes = Mathf.FloorToInt(elapsedTime / 60f);
            int seconds = Mathf.FloorToInt(elapsedTime % 60f);
            timeText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
        }
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

    // --- 조작키 탭 버튼에 연결할 함수 ---
    public void OpenKeyTab()
    {
        if (soundContent != null) soundContent.SetActive(false);
        if (keyContent != null) keyContent.SetActive(true);
        Debug.Log("조작키 탭으로 변경!");
    }

    // --- 소리 설정 탭 버튼에 연결할 함수 --- 
    public void OpenSoundTab()
    {
        if (soundContent != null) soundContent.SetActive(true);
        if (keyContent != null) keyContent.SetActive(false);
        Debug.Log("소리 설정 탭으로 변경!");
    }

    public void CloseAllMenus()
    {
        if (mainPauseMenu != null) mainPauseMenu.SetActive(false);
        if (settingPanel != null) settingPanel.SetActive(false);
        
        isPaused = false;
        Time.timeScale = 1f;
    }
}