using UnityEngine;
using TMPro;

public class KeySettingsUI : MonoBehaviour
{
    [Header("각 키의 글자를 보여줄 UI TextMeshPro 컴포넌트들")]
    public TextMeshProUGUI attackText;
    public TextMeshProUGUI skillXText;
    public TextMeshProUGUI skillCText;
    public TextMeshProUGUI dashText;
    public TextMeshProUGUI jumpText;
    public TextMeshProUGUI leftText;
    public TextMeshProUGUI rightText;

    private string currentChangingAction = ""; 
    private bool isWaitingForInput = false;    

    void OnEnable()
    {
        ForceSyncKeyManager();
        UpdateKeyUI();
    }

    void Start()
    {
        ForceSyncKeyManager();
        UpdateKeyUI();
    }

    
    void Update()
    {
        if (!isWaitingForInput) return;

        // 현재 프레임에 어떤 키보드 자판이 눌렸는지 실시간 전수 조사
        if (Input.anyKeyDown)
        {
            foreach (KeyCode kcode in System.Enum.GetValues(typeof(KeyCode)))
            {
                // 마우스 클릭은 제외하고 순수 키보드 자판만 인식합니다.
                if (Input.GetKeyDown(kcode) && kcode != KeyCode.None && !kcode.ToString().Contains("Mouse"))
                {
                    ChangeAndSaveKey(kcode);
                    break;
                }
            }
        }
    }

    // 키 변경 및 컴퓨터 세이브 정석 루틴
    private void ChangeAndSaveKey(KeyCode newKeyCode)
    {
        if (KeyManager.instance != null)
        {
            // 1. 메모리 장부 동기화
            if (!KeyManager.instance.keys.ContainsKey(currentChangingAction))
                KeyManager.instance.keys.Add(currentChangingAction, newKeyCode);
            else
                KeyManager.instance.keys[currentChangingAction] = newKeyCode;

            // 2. 컴퓨터 하드에 저장
            PlayerPrefs.SetString("Key_" + currentChangingAction, newKeyCode.ToString());
            PlayerPrefs.Save();
        }

        // 3. 상태 리셋 및 화면 글자 갱신
        isWaitingForInput = false;
        currentChangingAction = "";
        UpdateKeyUI();
    }

    private void ForceSyncKeyManager()
    {
        if (KeyManager.instance == null)
        {
            KeyManager.instance = FindObjectOfType<KeyManager>();
        }

        if (KeyManager.instance != null)
        {
            KeyManager.instance.LoadKeySettings();
        }
    }

    public void UpdateKeyUI()
    {
        if (KeyManager.instance == null) ForceSyncKeyManager();
        if (KeyManager.instance == null || KeyManager.instance.keys == null) return;

        SetKeyText(attackText, "Attack", KeyCode.Z);
        SetKeyText(skillXText, "Skill_X", KeyCode.X);
        SetKeyText(skillCText, "Skill_C", KeyCode.C);
        SetKeyText(dashText, "Dash", KeyCode.DownArrow);
        SetKeyText(jumpText, "Jump", KeyCode.UpArrow);
        SetKeyText(leftText, "Left", KeyCode.LeftArrow);
        SetKeyText(rightText, "Right", KeyCode.RightArrow);
    }

    private void SetKeyText(TextMeshProUGUI textComponent, string actionName, KeyCode defaultKey)
    {
        if (textComponent == null) return;

        if (KeyManager.instance.keys.ContainsKey(actionName))
        {
            textComponent.text = KeyManager.instance.keys[actionName].ToString();
        }
        else
        {
            string savedKey = PlayerPrefs.GetString("Key_" + actionName, defaultKey.ToString());
            textComponent.text = savedKey;
        }
    }

    public void ClickChangeAttack() { StartWaiting("Attack", attackText); }
    public void ClickChangeSkillX() { StartWaiting("Skill_X", skillXText); }
    public void ClickChangeSkillC() { StartWaiting("Skill_C", skillCText); }
    public void ClickChangeDash()   { StartWaiting("Dash", dashText); }
    public void ClickChangeJump()   { StartWaiting("Jump", jumpText); }
    public void ClickChangeLeft()   { StartWaiting("Left", leftText); }
    public void ClickChangeRight()  { StartWaiting("Right", rightText); }

    private void StartWaiting(string actionName, TextMeshProUGUI targetText)
    {
        // 이미 다른 키를 바꾸는 중이면 중복 작동 방지
        if (isWaitingForInput) return;

        isWaitingForInput = true;
        currentChangingAction = actionName;
        if (targetText != null) targetText.text = "입력 대기중...";
    }
}