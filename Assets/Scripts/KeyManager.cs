using System.Collections.Generic;
using UnityEngine;

public class KeyManager : MonoBehaviour
{
    public static KeyManager instance;


    public Dictionary<string, KeyCode> keys = new Dictionary<string, KeyCode>();

    [System.Serializable]
    public struct KeyBind
    {
        public string actionName;
        public KeyCode keyCode;
    }

    [Header("최초 기본 키 설정")]
    public KeyBind[] defaultKeys;

    void Awake()
    {
        if (instance == null)
        {
            // 1. 메인메뉴에서 처음 태어난 진짜 원본 매니저 고정
            instance = this;
            
            // 만약 하이어라키 최상위에 있다면 씬이 바뀌어도 파괴되지 않게 보호
            if (transform.parent == null)
            {
                DontDestroyOnLoad(gameObject);
            }
            
            InitKeyDictionary();
        }
        else
        {

            instance.LoadKeySettings();
            

            if (transform.parent != null)
            {
                Destroy(this); // 스크립트만 제거
            }
            else
            {
                Destroy(gameObject); // 독립 오브젝트일 때만 오브젝트 파괴
            }
            return;
        }
    }


    public void InitKeyDictionary()
    {
        keys.Clear();
        foreach (KeyBind bind in defaultKeys)
        {
            if (string.IsNullOrEmpty(bind.actionName)) continue;

            string savedKey = PlayerPrefs.GetString("Key_" + bind.actionName, bind.keyCode.ToString());
            KeyCode finalKeyCode = (KeyCode)System.Enum.Parse(typeof(KeyCode), savedKey);
            
            if (!keys.ContainsKey(bind.actionName))
            {
                keys.Add(bind.actionName, finalKeyCode);
            }
        }
    }

    // 다른 씬으로 넘어갔을 때 저장 장부를 새로고침하는 함수
    public void LoadKeySettings()
    {
        // 만약 어떤 이유로 장부가 비어있다면 새로 채웁니다.
        if (keys == null || keys.Count == 0)
        {
            InitKeyDictionary();
            return;
        }

        foreach (KeyBind bind in defaultKeys)
        {
            if (string.IsNullOrEmpty(bind.actionName)) continue;

            string savedKey = PlayerPrefs.GetString("Key_" + bind.actionName, bind.keyCode.ToString());
            if (keys.ContainsKey(bind.actionName))
            {
                keys[bind.actionName] = (KeyCode)System.Enum.Parse(typeof(KeyCode), savedKey);
            }
            else
            {
                keys.Add(bind.actionName, (KeyCode)System.Enum.Parse(typeof(KeyCode), savedKey));
            }
        }
    }
}