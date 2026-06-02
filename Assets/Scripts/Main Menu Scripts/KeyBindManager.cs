using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class KeyBindManager : MonoBehaviour
{
    public static KeyBindManager instance;

    // 키 액션 이름과 실제 키코드를 매칭하는 딕셔너리 장부
    public Dictionary<string, KeyCode> keys = new Dictionary<string, KeyCode>();

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            InitKeys();
        }
        else
        {
            // 중복 매니저가 생기면 장부만 최신화시키고 파괴 (사운드 매니저와 동일 구조)
            instance.LoadKeySettings();
            Destroy(gameObject);
            return;
        }
    }

    // 최초 키 세팅 (컴퓨터에 저장된 게 없으면 기본 정해진 키로 세팅)
    void InitKeys()
    {
        // PlayerPrefs에 저장된 값이 없으면 기본값(Z, X, C, Shift 등)으로 세팅
        keys.Add("Attack", (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("Key_Attack", "Z")));
        keys.Add("SkillX", (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("Key_SkillX", "X")));
        keys.Add("SkillC", (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("Key_SkillC", "C")));
        keys.Add("Dash", (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("Key_Dash", "LeftShift")));
    }

    public void LoadKeySettings()
    {
        keys["Attack"] = (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("Key_Attack", "Z"));
        keys["SkillX"] = (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("Key_SkillX", "X"));
        keys["SkillC"] = (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("Key_SkillC", "C"));
        keys["Dash"] = (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("Key_Dash", "LeftShift"));
    }

    // 특정 액션의 키를 변경하고 저장하는 함수
    public void SetKey(string actionName, KeyCode newKeyCode)
    {
        if (keys.ContainsKey(actionName))
        {
            keys[actionName] = newKeyCode;
            PlayerPrefs.SetString("Key_" + actionName, newKeyCode.ToString());
            PlayerPrefs.Save();
            Debug.Log($"{actionName} 키가 {newKeyCode}로 변경 및 저장되었습니다.");
        }
    }
}