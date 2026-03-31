using UnityEngine;
using UnityEngine.UI;

public class SkillCooldownUI : MonoBehaviour
{
    [Header("연결할 플레이어 캐릭터")]
    public Player player;

    [Header("쿨타임 이미지들 (Image Type: Filled 필수)")]
    public Image dashFilter;
    public Image skillCFilter;
    public Image skillXFilter;

    // 실시간 타이머 변수 //
    private float dashTimer;
    private float skillCTimer;
    private float skillXTimer;

    void Update()
    {
        if (player == null) return;

        UpdateUI(ref dashTimer, player.canDash, player.dashDuration + player.dashDelay, dashFilter);


        UpdateUI(ref skillCTimer, player.canSkillC, player.skillDuration + player.skillCDelay, skillCFilter);

     
        UpdateUI(ref skillXTimer, player.canSkillX, 0.3f + player.skill1Delay, skillXFilter);
    }

    void UpdateUI(ref float timer, bool canUse, float maxCD, Image filter)
    {
        if (filter == null) return;

        if (!canUse) 
        {
            timer += Time.deltaTime;
            filter.fillAmount = 1f - (timer / maxCD); 

            if (timer >= maxCD)
            {
                timer = 0;
                filter.fillAmount = 0;
            }
        }
        else
        {
            timer = 0;
            filter.fillAmount = 0;
        }
    }
}