using UnityEngine;
using UnityEngine.SceneManagement;

public class SettingMenutoBack : MonoBehaviour
{
    public void BacktoMain()
    {

        SceneManager.LoadScene("MainMenu"); 
    }
}