using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;

public class GameOverFade : MonoBehaviour
{
    [Header("게임오버 이미지 연결")]
    public Image gameOverImage;

    [Header("이미지 페이드인 시간 (초)")]
    public float fadeDuration = 2.0f; 

    [Header("페이드인 종료 후 나타날 버튼 그룹의 CanvasGroup 연결")]
    public CanvasGroup buttonCanvasGroup;

    [Header("버튼 페이드인 시간 (초)")]
    public float buttonFadeDuration = 1.0f;

    [Header("이동할 메인메뉴와 배틀 씬의 정확한 이름")]
    public string mainMenuSceneName = "MainMenu";
    public string battleSceneName = "BattleScene"; 

    void Start()
    {
        if (gameOverImage == null)
        {
            gameOverImage = GetComponent<Image>();
        }

        if (buttonCanvasGroup != null)
        {
            buttonCanvasGroup.alpha = 0f;
            buttonCanvasGroup.interactable = false;
            buttonCanvasGroup.blocksRaycasts = false;
        }

        if (gameOverImage != null)
        {
            Color c = gameOverImage.color;
            c.a = 0f;
            gameOverImage.color = c;

            StartCoroutine(GameOverSequenceRoutine());
        }
    }

    IEnumerator GameOverSequenceRoutine()
    {
        float timer = 0f;
        Color originalColor = gameOverImage.color;

        while (timer < fadeDuration)
        {
            timer += Time.unscaledDeltaTime;
            originalColor.a = timer / fadeDuration;
            gameOverImage.color = originalColor;
            yield return null;
        }
        originalColor.a = 1f;
        gameOverImage.color = originalColor;
        
        if (buttonCanvasGroup != null)
        {
            timer = 0f;
            while (timer < buttonFadeDuration)
            {
                timer += Time.unscaledDeltaTime;
                buttonCanvasGroup.alpha = timer / buttonFadeDuration;
                yield return null;
            }
            buttonCanvasGroup.alpha = 1f;
            
            buttonCanvasGroup.interactable = true;
            buttonCanvasGroup.blocksRaycasts = true;
        }
    }

    public void ClickRetry()
    {
        Time.timeScale = 1f; 
        
        if (BattleDialogueManager.instance != null)
        {
            BattleDialogueManager.instance.ForceCloseDialogue();
        }

        SceneManager.LoadScene(battleSceneName);
    }

    public void ClickMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(mainMenuSceneName);
    }
}