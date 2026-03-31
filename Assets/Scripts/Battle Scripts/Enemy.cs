using UnityEngine;

public class Enemy : MonoBehaviour
{
    [Header("체력 설정 (하트 3개)")]
    public int maxHealth = 3; 
    private int currentHealth;

    [Header("하트 오브젝트 설정")]

    public GameObject[] heartObjects; 

    private SpriteRenderer spriteRenderer;

    void Start()
    {
        currentHealth = maxHealth;
        spriteRenderer = GetComponent<SpriteRenderer>();
        UpdateHeartUI();
    }

    public void TakeDamage(int damage)
    {
        if (currentHealth <= 0) return;


        currentHealth -= 1; 

  
        UpdateHeartUI();

        Debug.Log($"{gameObject.name}의 남은 하트: {currentHealth}");


        if (spriteRenderer != null)
        {
            StopAllCoroutines();
            StartCoroutine(HitEffect());
        }

        if (currentHealth <= 0) Die();
    }

    void UpdateHeartUI()
    {
        if (heartObjects == null || heartObjects.Length == 0) return;

        for (int i = 0; i < heartObjects.Length; i++)
        {
            if (heartObjects[i] == null) continue;
            heartObjects[i].SetActive(i < currentHealth);
        }
    }

    System.Collections.IEnumerator HitEffect()
    {
        spriteRenderer.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        spriteRenderer.color = Color.white;
    }

    void Die()
    {
        Debug.Log("적이 죽었습니다!");
        Destroy(gameObject);
    }
}