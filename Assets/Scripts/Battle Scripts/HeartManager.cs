using UnityEngine;
using UnityEngine.UI;

public class HeartManager : MonoBehaviour
{
    [Header("하트 이미지 3개를 드래그해서 넣으세요")]
    public Image[] hearts; 

    [Header("하트 스프라이트 설정")]
    public Sprite fullHeart;
    public Sprite emptyHeart;

    public void UpdateHearts(int currentHealth)
    {
        if (hearts == null || hearts.Length == 0) return;

        for (int i = 0; i < hearts.Length; i++)
        {
            if (i < currentHealth)
            {

                hearts[i].sprite = fullHeart;
                hearts[i].color = Color.white;
            }
            else
            {
   
                hearts[i].sprite = emptyHeart;
                hearts[i].color = Color.white;
            }
        }
    }
}