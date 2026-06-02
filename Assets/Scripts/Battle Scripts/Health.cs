using UnityEngine;
using System.Collections;

public class Health : MonoBehaviour
{
    [Header("체력 설정")]
    public int maxHealth = 3;
    public int currentHealth;

    [Header("하트 오브젝트 (자식 오브젝트를 드래그해서 넣으세요)")]
    public GameObject[] heartObjects;

    private SpriteRenderer spriteRenderer;
    private bool isDead = false;

    void Start()
    {
        ResetHealth();
    }
    public void ResetHealth()
    {
        currentHealth = maxHealth;
        isDead = false;
        spriteRenderer = GetComponent<SpriteRenderer>();
        UpdateHeartUI();
    }

    public void HealFull()
    {
        ResetHealth();
        Debug.Log(gameObject.name + " 체력 완전 회복!");
    }

    public void TakeDamage(int damage)
    {
        if (isDead) return;
        currentHealth -= damage;
        if (currentHealth < 0) currentHealth = 0;

        UpdateHeartUI();
        if (spriteRenderer != null) StartCoroutine(HitEffect());
        if (currentHealth <= 0) Die();
    }

    public void UpdateHeartUI()
    {
        if (heartObjects == null) return;
        for (int i = 0; i < heartObjects.Length; i++)
        {
            if (heartObjects[i] != null)
            {
                heartObjects[i].SetActive(i < currentHealth);
            }
        }
    }

    IEnumerator HitEffect()
    {
        spriteRenderer.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        spriteRenderer.color = Color.white;
    }

    void Die()
    {
        if (isDead) return;
        isDead = true;

        // 1. 이 오브젝트가 플레이어일 때 처리 구역
        if (gameObject.CompareTag("Player"))
        {
            Debug.Log("🎯 플레이어 사망 감지! Player.Die() 함수를 호출합니다.");
            
            Player player = GetComponent<Player>();
            if (player != null)
            {
                player.Die(); // Player.cs에 있는 사망 및 즉시 컷씬 켜기 함수 실행!
            }
            else
            {
                Debug.LogWarning("⚠️ 오브젝트에 Player 스크립트 컴포넌트가 없습니다!");
            }
        }
        // 2. 이 오브젝트가 적(Enemy)일 때 기존 로직 유지
        else
        {
            if (RoundManager.instance != null) RoundManager.instance.OnEnemyDeath();
            Destroy(gameObject);
        }
    }
}