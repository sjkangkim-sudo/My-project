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

        if (!gameObject.CompareTag("Player"))
        {
            if (RoundManager.instance != null) RoundManager.instance.OnEnemyDeath();
            Destroy(gameObject);
        }
    }
}