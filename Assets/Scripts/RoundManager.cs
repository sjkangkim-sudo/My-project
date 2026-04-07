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

    void Awake()
    {
        instance = this;
    }

    public void OnEnemyDeath()
    {
        if (battleUI != null) battleUI.NextRound();

        Health playerHealth = player.GetComponent<Health>();
        if (playerHealth != null) playerHealth.HealFull();

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
            if (ai != null) ai.player = player.transform;
            Debug.Log("새로운 적이 UI 하트와 연결되어 나타났습니다!");
        }
    }
}