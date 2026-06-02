using UnityEngine;
using UnityEngine.UI; // 버튼(Button) 컴포넌트를 제어하기 위해 반드시 필요합니다!

public class ButtonSoundAssigner : MonoBehaviour
{
    void Start()
    {
        
        Button[] allButtons = FindObjectsOfType<Button>(true);


        foreach (Button btn in allButtons)
        {

            btn.onClick.AddListener(() => {
                if (SoundManager.instance != null)
                {
                    SoundManager.instance.PlayButtonClick();
                }
            });
        }
    }
}