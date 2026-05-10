using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    public void PlayGame()
    {

        SceneManager.LoadScene("SubMenu"); 
    }

    public void SettingGame()
    {

        SceneManager.LoadScene("Setting"); 
    }

    public void QuitGame()
    {
        Application.Quit();
        Debug.Log("게임 종료!");
    }
}