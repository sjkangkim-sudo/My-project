using UnityEngine;
using UnityEngine.SceneManagement;

public class SubMenuManager : MonoBehaviour
{
    public void PlayGame()
    {

        SceneManager.LoadScene("StoryCutscene1"); 
    }

    public void QuitGame()
    {
        Application.Quit();
        Debug.Log("게임 종료!");
    }
}