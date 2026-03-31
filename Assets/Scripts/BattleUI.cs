using UnityEngine;
using TMPro;

public class BattleUI : MonoBehaviour
{
    public TextMeshProUGUI roundText;
    public TextMeshProUGUI timeText;

    private int currentRound = 1;
    private float elapsedTime = 0f;

    void Update()
    {
        elapsedTime += Time.deltaTime;
        int minutes = Mathf.FloorToInt(elapsedTime / 60f);
        int seconds = Mathf.FloorToInt(elapsedTime % 60f);
        timeText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    public void NextRound()
    {
        currentRound++;
        roundText.text = "Round " + currentRound;
    }
}