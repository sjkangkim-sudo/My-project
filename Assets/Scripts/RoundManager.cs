using UnityEngine;
using System.Collections;

public class RoundManager : MonoBehaviour
{
    public static RoundManager instance; 

    [Header("연결할 객체들")]
    public Player player;            
    public GameObject enemyPrefab;   
    public Transform spawnPoint;     
    public BattleUI battleUI;        

    [Header("화면에 고정된 적 하트들")]
    public GameObject[] uiEnemyHearts;

    [Header("★ 리스폰 대사 사운드 등록")]
    public AudioClip bossRespawnVoice; 
    public AudioClip heroRespawnVoice; 

    private bool hasPlayedRespawnDialogue = false;

    void Awake()
    {
        instance = this;
        hasPlayedRespawnDialogue = false;
    }

    void Start()
    {
        // [★ 핵심] 다시하기 시 날아간 선들을 자동으로 하이어라키에서 새로 찾아와 매핑합니다!
        if (player == null) player = FindObjectOfType<Player>();
        if (battleUI == null) battleUI = GetComponent<BattleUI>();
        if (spawnPoint == null) spawnPoint = GameObject.Find("SpawnPoint")?.transform;

        // 하트 UI 오브젝트 3개도 태어난 새 씬의 오브젝트로 강제 교체
        if (uiEnemyHearts == null || uiEnemyHearts.Length == 0 || uiEnemyHearts[0] == null)
        {
            uiEnemyHearts = new GameObject[3];
            uiEnemyHearts[0] = GameObject.Find("EnemyHeart0"); // 성준님 에디터상의 실제 하트 오브젝트 이름으로 매칭하세요!
            uiEnemyHearts[1] = GameObject.Find("EnemyHeart1");
            uiEnemyHearts[2] = GameObject.Find("EnemyHeart2");
        }
    }

    public void OnEnemyDeath()
    {
        if (battleUI != null) battleUI.NextRound();

        if (player != null)
        {
            Health playerHealth = player.GetComponent<Health>();
            if (playerHealth != null) playerHealth.HealFull();
        }

        StartCoroutine(RespawnRoutine());
    }

    IEnumerator RespawnRoutine()
    {
        yield return new WaitForSeconds(1.5f); 
        
        if (enemyPrefab != null && spawnPoint != null)
        {
            GameObject newEnemy = Instantiate(enemyPrefab, spawnPoint.position, Quaternion.identity);
            Health enemyHealth = newEnemy.GetComponent<Health>();
            
            if (enemyHealth != null) 
            {
                enemyHealth.heartObjects = uiEnemyHearts; 
                enemyHealth.currentHealth = enemyHealth.maxHealth;
                enemyHealth.UpdateHeartUI(); 
            }

            EnemyAI ai = newEnemy.GetComponent<EnemyAI>();
            if (ai != null && player != null) ai.player = player.transform;

            if (!hasPlayedRespawnDialogue)
            {
                hasPlayedRespawnDialogue = true; 
                StartCoroutine(PlayRespawnDialogueSequence());
            }
        }
    }

    IEnumerator PlayRespawnDialogueSequence()
    {
        yield return new WaitForSecondsRealtime(0.1f); 

        if (BattleDialogueManager.instance != null)
        {
            BattleDialogueManager.instance.PlayDialogue("마왕", "아니 어떻게 다시 돌아온거지..?", bossRespawnVoice);
            BattleDialogueManager.instance.PlayDialogue("용사", "널 쓰러뜨릴때까지 나는 계속 올거야.", heroRespawnVoice);
        }
    }
}