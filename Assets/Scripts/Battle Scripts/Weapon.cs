using UnityEngine;

public class Weapon : MonoBehaviour
{
    public float speed = 30f;      
    public int damage = 1;         
    public float lifetime = 4f;    

    private Rigidbody2D rb;
    private bool isHit = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.velocity = transform.right * speed;
        Destroy(gameObject, lifetime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {

        if (isHit) return;

 
        if (collision.CompareTag("Player"))
        {
            Player player = collision.GetComponent<Player>();
            if (player != null)
            {
                isHit = true; // 맞았음을 표시
                player.TakeDamage(damage);
                Debug.Log("플레이어 피격! 데미지: " + damage);
                Destroy(gameObject); // 즉시 삭제
            }
        }


        else if (collision.CompareTag("Enemy"))
        {
            Health targetHealth = collision.GetComponent<Health>();
            if (targetHealth != null)
            {
                isHit = true; // 맞았음을 표시
                targetHealth.TakeDamage(damage);
                Debug.Log("적 피격! 데미지: " + damage);
                Destroy(gameObject); // 즉시 삭제
            }
        }


        else if (collision.CompareTag("Ground"))
        {
            Destroy(gameObject);
        }
    }
}