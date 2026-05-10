using UnityEngine;
using System.Collections.Generic;
using TMPro;

public class KeyManager : MonoBehaviour
{
    public static KeyManager instance;
    [Header("현재 키 표시 텍스트 (직접 연결)")]
    public TextMeshProUGUI leftKeyText;
    public TextMeshProUGUI rightKeyText;
    public TextMeshProUGUI jumpKeyText;
    public TextMeshProUGUI dashKeyText;
    public TextMeshProUGUI attackKeyText;
    public TextMeshProUGUI skillCKeyText;
    public TextMeshProUGUI skillXKeyText;

    private string currentChangingKey = "";
    private bool isWaiting = false;

    public Dictionary<string, KeyCode> keys = new Dictionary<string, KeyCode>();

    void Awake()
    {
        keys["LEFT"] = KeyCode.LeftArrow;
        keys["RIGHT"] = KeyCode.RightArrow;
        keys["JUMP"] = KeyCode.UpArrow;
        keys["DASH"] = KeyCode.DownArrow;
        keys["ATTACK"] = KeyCode.Z;
        keys["SKILL_C"] = KeyCode.C;
        keys["SKILL_X"] = KeyCode.X;
        
        UpdateUI();
    }

    public void StartWaiting(string keyName)
    {
        Debug.Log(keyName + " 입력 대기 시작!"); 
        currentChangingKey = keyName;
        isWaiting = true;

        UpdateTargetText(keyName, "???");
    }

    void OnGUI()
    {
        if (!isWaiting) return;

        Event e = Event.current;
        if (e.isKey && e.type == EventType.KeyDown)
        {
            if (e.keyCode != KeyCode.None)
            {
                keys[currentChangingKey] = e.keyCode;
                Debug.Log(currentChangingKey + " 키가 " + e.keyCode + "로 변경됨!");
                
                isWaiting = false;
                currentChangingKey = "";
                UpdateUI();
            }
        }
    }

    void UpdateUI()
    {
        if (leftKeyText != null) leftKeyText.text = keys["LEFT"].ToString();
        if (rightKeyText != null) rightKeyText.text = keys["RIGHT"].ToString();
        if (jumpKeyText != null) jumpKeyText.text = keys["JUMP"].ToString();
        if (dashKeyText != null) dashKeyText.text = keys["DASH"].ToString();
        if (attackKeyText != null) attackKeyText.text = keys["ATTACK"].ToString();
        if (skillCKeyText != null) skillCKeyText.text = keys["SKILL_C"].ToString();
        if (skillXKeyText != null) skillXKeyText.text = keys["SKILL_X"].ToString();
    }

    void UpdateTargetText(string keyName, string value)
    {
        if (keyName == "LEFT" && leftKeyText != null) leftKeyText.text = value;
        else if (keyName == "RIGHT" && rightKeyText != null) rightKeyText.text = value;
        else if (keyName == "JUMP" && jumpKeyText != null) jumpKeyText.text = value;
        else if (keyName == "DASH" && dashKeyText != null) dashKeyText.text = value;
        else if (keyName == "ATTACK" && attackKeyText != null) attackKeyText.text = value;
        else if (keyName == "SKILL_C" && skillCKeyText != null) skillCKeyText.text = value;
        else if (keyName == "SKILL_X" && skillXKeyText != null) skillXKeyText.text = value;
    }
}