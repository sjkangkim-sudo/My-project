using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class BattleDialogueManager : MonoBehaviour
{
    public static BattleDialogueManager instance;

    [System.Serializable]
    public class DialogueData
    {
        public string speaker;      
        [TextArea(2, 4)]
        public string content;      
        public AudioClip voiceClip; 
    }

    [Header("UI 컴포넌트 연결")]
    public GameObject dialoguePanel;      
    public TMP_Text speakerTextDisplay;   
    public TMP_Text contentTextDisplay;   

    [Header("캐릭터 일러스트 이미지 연결")]
    public Image heroImageDisplay;        
    public Image bossImageDisplay;        

    [Header("설정")]
    public float typingSpeed = 0.04f;     
    public float displayDuration = 0.8f; 

    private AudioSource dialogueAudioSource;
    private Queue<DialogueData> dialogueQueue = new Queue<DialogueData>();
    private bool isDialoguePlaying = false; 

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            
            dialogueAudioSource = GetComponent<AudioSource>();
            if (dialogueAudioSource == null) dialogueAudioSource = gameObject.AddComponent<AudioSource>();
            
            dialogueAudioSource.playOnAwake = false;
            dialogueAudioSource.loop = false;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    public void PlayDialogue(string speaker, string content)
    {
        PlayDialogue(speaker, content, null);
    }

    public void PlayDialogue(string speaker, string content, AudioClip voice)
    {
        DialogueData data = new DialogueData { speaker = speaker, content = content, voiceClip = voice };
        dialogueQueue.Enqueue(data);

        if (!isDialoguePlaying)
        {
            StartCoroutine(DialogueSequenceLoop());
        }
    }

    IEnumerator DialogueSequenceLoop()
    {
        isDialoguePlaying = true;
        Time.timeScale = 0f; 

        while (dialogueQueue.Count > 0)
        {
            DialogueData currentDialogue = dialogueQueue.Dequeue();

            dialoguePanel.SetActive(true);
            speakerTextDisplay.text = currentDialogue.speaker;
            contentTextDisplay.text = "";

            UpdateIllustration(currentDialogue.speaker);

            if (dialogueAudioSource != null)
            {
                dialogueAudioSource.Stop();
                if (currentDialogue.voiceClip != null)
                {
                    dialogueAudioSource.clip = currentDialogue.voiceClip;
                    dialogueAudioSource.Play();
                }
            }

            foreach (char letter in currentDialogue.content.ToCharArray())
            {
                contentTextDisplay.text += letter;
                yield return new WaitForSecondsRealtime(typingSpeed);
            }

            if (dialogueAudioSource != null)
            {
                dialogueAudioSource.Stop();
            }

            yield return new WaitForSecondsRealtime(displayDuration);
        }

        dialoguePanel.SetActive(false);
        if (heroImageDisplay != null) heroImageDisplay.gameObject.SetActive(false);
        if (bossImageDisplay != null) bossImageDisplay.gameObject.SetActive(false);

        if (dialogueAudioSource != null) dialogueAudioSource.Stop();

        isDialoguePlaying = false;
        Time.timeScale = 1f; 
    }

    void UpdateIllustration(string speaker)
    {
        if (speaker == "용사")
        {
            if (heroImageDisplay != null) {
                heroImageDisplay.gameObject.SetActive(true);
                heroImageDisplay.color = Color.white;
            }
            if (bossImageDisplay != null) {
                bossImageDisplay.color = new Color(0.3f, 0.3f, 0.3f, 1f); 
            }
        }
        else if (speaker == "마왕")
        {
            if (bossImageDisplay != null) {
                bossImageDisplay.gameObject.SetActive(true);
                bossImageDisplay.color = Color.white;
            }
            if (heroImageDisplay != null) {
                heroImageDisplay.color = new Color(0.3f, 0.3f, 0.3f, 1f);
            }
        }
    }

    public void ForceCloseDialogue()
    {
        StopAllCoroutines();
        dialogueQueue.Clear();
        dialoguePanel.SetActive(false);
        if (heroImageDisplay != null) heroImageDisplay.gameObject.SetActive(false);
        if (bossImageDisplay != null) bossImageDisplay.gameObject.SetActive(false);
        if (dialogueAudioSource != null) dialogueAudioSource.Stop();
        isDialoguePlaying = false;
        Time.timeScale = 1f;
    }
}