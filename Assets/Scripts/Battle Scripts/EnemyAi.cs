using UnityEngine;
using System.Collections;

public class EnemyAI : MonoBehaviour
{
    private Health health; 

    [Header("AI 기본 설정")]
    public Transform player;      
    public float walkSpeed = 3f;  
    public float detectRange = 8f; 
    public float attackRange = 1.2f; 
    public float attackCooldown = 1.5f; 

    [Header("점프 설정")]
    public float jumpForce = 10f;
    public Transform obstacleCheck;
    public float checkDistance = 0.5f;
    public LayerMask groundLayer;

    [Header("스킬1 설정 (칼 던지기)")]
    public GameObject knifePrefab;
    public Transform firePoint;
    public float throwRange = 5f;
    public float throwCooldown = 3f;
    private float lastThrowTime = 0f;

    [Header("공격 판정 (근접)")]
    public Transform attackPoint; 
    public float hitRange = 0.6f; 
    public LayerMask playerLayer; 

    [Header("상태 확인")]
    public bool isGrounded;

    private bool isAttacking = false;
    private Rigidbody2D rb;
    private Animator anim;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        health = GetComponent<Health>(); 

        if (player == null)
        {
            GameObject playerObj = GameObject.FindWithTag("Player");
            if (playerObj != null) player = playerObj.transform;
        }
    }

    void Update()
    {
        if (health != null && health.currentHealth <= 0) return;
        if (isAttacking) return;

        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

    
        if (distanceToPlayer <= throwRange && distanceToPlayer > throwRange * 0.7f)
        {
            if (Time.time - lastThrowTime >= throwCooldown && isGrounded)
            {
                StartCoroutine(ThrowSkillRoutine());
                return;
            }
        }

        if (distanceToPlayer <= detectRange && distanceToPlayer > attackRange)
        {
            MoveAndJumpCheck();
        }
        else if (distanceToPlayer <= attackRange)
        {
            if (Time.time - lastAttackTime() >= attackCooldown)
            {
                StartCoroutine(AttackRoutine());
            }
            else
            {
                StopMovement();
            }
        }
        else
        {
            StopMovement();
        }

        if (anim != null) anim.SetBool("isGrounded", isGrounded);
    }


    void MoveAndJumpCheck()
    {
        float direction = (player.position.x > transform.position.x) ? 1 : -1;
        rb.velocity = new Vector2(direction * walkSpeed, rb.velocity.y);
        transform.localScale = new Vector3(direction, 1, 1);


        bool isObstacle = Physics2D.Raycast(obstacleCheck.position, Vector2.right * direction, checkDistance, groundLayer);
        if (isObstacle && isGrounded)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            isGrounded = false;
        }

        if (anim != null) anim.SetBool("isWalking", true);
    }


    IEnumerator ThrowSkillRoutine()
    {
        isAttacking = true;
        lastThrowTime = Time.time;
        StopMovement();

        if (anim != null) anim.SetTrigger("doSkill1");

        yield return new WaitForSeconds(0.2f);
        
 
        if (knifePrefab != null && firePoint != null)
        {
            float angle = transform.localScale.x > 0 ? 0 : 180;
            Instantiate(knifePrefab, firePoint.position, Quaternion.Euler(0, 0, angle));
        }

        yield return new WaitForSeconds(0.5f);
        isAttacking = false;
    }


    IEnumerator AttackRoutine()
    {
        isAttacking = true;
        StopMovement();
        if (anim != null) anim.SetTrigger("doAttack"); 
        yield return new WaitForSeconds(0.4f); 
        ExecuteAttack();
        yield return new WaitForSeconds(0.4f); 
        isAttacking = false;
    }


    void StopMovement() { rb.velocity = new Vector2(0, rb.velocity.y); if (anim != null) anim.SetBool("isWalking", false); }
    void ExecuteAttack() { Collider2D hit = Physics2D.OverlapCircle(attackPoint.position, hitRange, playerLayer); if (hit != null) { Player p = hit.GetComponent<Player>(); if (p != null) p.TakeDamage(1); } }
    float lastAttackTime() { return Time.time - attackCooldown; }

    private void OnCollisionEnter2D(Collision2D collision) { if (collision.gameObject.CompareTag("Ground")) isGrounded = true; }
    private void OnCollisionExit2D(Collision2D collision) { if (collision.gameObject.CompareTag("Ground")) isGrounded = false; }
}