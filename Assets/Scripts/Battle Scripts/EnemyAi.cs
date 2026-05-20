using UnityEngine;
using System.Collections;

public class EnemyAI : MonoBehaviour
{
    private Health health; 

    [Header("AI 기본 설정")]
    public Transform player;      
    public float walkSpeed = 3f;  
    public float detectRange = 12f; 
    public float attackRange = 1.5f; 
    public float attackCooldown = 1.5f; 
    private float lastAttackTimeValue = 0f;

    [Header("점프 및 바닥 체크 설정")]
    public float jumpForce = 12f;
    public Transform obstacleCheck;
    public float checkDistance = 0.5f;
    public float groundCheckDistance = 0.7f; 
    public LayerMask groundLayer;

    [Header("스킬 연출 및 범위 세팅")]
    public GameObject shockwaveEffectPrefab;
    public Transform skillSpawnPoint;
    public float shockwaveRange = 3.5f;    
    public int shockwaveDamage = 1;        
    public float skillCCooldown = 4f;       
    private float lastSkillCTime = 0f;

    [Header("스킬1 설정 (X키 - 칼 던지기)")]
    public GameObject knifePrefab;
    public Transform firePoint;
    public float throwRange = 7f;
    public float throwCooldown = 3f;
    private float lastThrowTime = 0f;

    [Header("구르기(대시) 설정")]
    public float dashSpeed = 15f;
    public float dashDuration = 0.2f;
    public float dashCooldown = 5f;
    private float lastDashTime = 0f;
    private bool isDashing = false;
    private bool isInvincible = false;

    [Header("공격 판정 (근접)")]
    public Transform attackPoint; 
    public float hitRange = 0.6f; 
    public LayerMask playerLayer; 

    [Header("상태 확인")]
    public bool isGrounded;
    private bool isAttacking = false;
    private Rigidbody2D rb;
    private Animator anim;
    private SpriteRenderer sprite;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        sprite = GetComponent<SpriteRenderer>();
        health = GetComponent<Health>(); 

        if (player == null)
        {
            GameObject playerObj = GameObject.FindWithTag("Player");
            if (playerObj != null) player = playerObj.transform;
        }
    }

    void Update()
    {
        if (Time.timeScale == 0) return;
        CheckGrounded();

        // 사망 시 행동 불능
        if (health != null && health.currentHealth <= 0) 
        {
            StopMovement();
            return;
        }

        if (isAttacking || isDashing || player == null) return;

        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        // [패턴 1] 구르기 조건
        if (distanceToPlayer <= 2f && Time.time - lastDashTime >= dashCooldown && isGrounded)
        {
            float dashDir = (player.position.x > transform.position.x) ? -1f : 1f; 
            StartCoroutine(DashRoutine(dashDir));
            return;
        }

        // [패턴 2] X스킬 (칼 던지기)
        if (distanceToPlayer <= throwRange && distanceToPlayer > throwRange * 0.6f)
        {
            if (Time.time - lastThrowTime >= throwCooldown && isGrounded)
            {
                StartCoroutine(ThrowSkillRoutine());
                return;
            }
        }

        // [패턴 3] C스킬 (충격파/지진)
        if (distanceToPlayer <= shockwaveRange && distanceToPlayer > attackRange)
        {
            if (Time.time - lastSkillCTime >= skillCCooldown && isGrounded)
            {
                StartCoroutine(ShockwaveSkillRoutine());
                return;
            }
        }

        // [기본 행동] 추적 및 근접 평타
        if (distanceToPlayer <= detectRange && distanceToPlayer > attackRange)
        {
            MoveAndJumpCheck();
        }
        else if (distanceToPlayer <= attackRange)
        {
            if (Time.time - lastAttackTimeValue >= attackCooldown)
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
    }

    void CheckGrounded()
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, groundCheckDistance, groundLayer);
        isGrounded = hit.collider != null;

        if (anim != null) anim.SetBool("isGrounded", isGrounded);
    }

    void MoveAndJumpCheck()
    {
        float direction = (player.position.x > transform.position.x) ? 1 : -1;
        rb.velocity = new Vector2(direction * walkSpeed, rb.velocity.y);
        transform.localScale = new Vector3(direction, 1, 1);

        // 전방 장애물 체크
        bool isObstacle = Physics2D.Raycast(obstacleCheck.position, Vector2.right * direction, checkDistance, groundLayer);
        
        if (isObstacle && isGrounded && rb.velocity.y <= 0.1f)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            isGrounded = false; 
            if (anim != null) anim.SetTrigger("doJump"); 
        }

        if (anim != null) anim.SetBool("isWalking", true);
    }

    // --- [X 스킬 : 칼 던지기 패턴] ---
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
            GameObject knife = Instantiate(knifePrefab, firePoint.position, Quaternion.Euler(0, 0, angle));
            Physics2D.IgnoreCollision(knife.GetComponent<Collider2D>(), GetComponent<Collider2D>());
        }

        yield return new WaitForSeconds(0.5f);
        isAttacking = false;
    }

    // --- [C 스킬 : 지진 충격파 패턴] ---
    IEnumerator ShockwaveSkillRoutine()
    {
        isAttacking = true;
        lastSkillCTime = Time.time;
        StopMovement();

        if (anim != null) anim.SetTrigger("doSkill"); 

        yield return new WaitForSeconds(0.2f);

        ExecuteShockwave();

        yield return new WaitForSeconds(0.5f);
        isAttacking = false;
    }

    void ExecuteShockwave()
    {
        if (shockwaveEffectPrefab != null)
        {
            Vector3 spawnPos = transform.position;
            if (skillSpawnPoint != null)
            {
                float direction = transform.localScale.x; 
                float localOffsetX = skillSpawnPoint.localPosition.x;
                float localOffsetY = skillSpawnPoint.localPosition.y;

                spawnPos = new Vector3(
                    transform.position.x + (localOffsetX * direction), 
                    transform.position.y + localOffsetY, 
                    transform.position.z
                );
            }
            Instantiate(shockwaveEffectPrefab, spawnPos, Quaternion.identity);
        }

        Collider2D hitPlayer = Physics2D.OverlapCircle(transform.position, shockwaveRange, playerLayer);
        if (hitPlayer != null)
        {
            Player p = hitPlayer.GetComponent<Player>();
            if (p != null) p.TakeDamage(shockwaveDamage);
        }
    }

    // --- [구르기 / 대시 패턴] ---
    IEnumerator DashRoutine(float direction)
    {
        isDashing = true;
        lastDashTime = Time.time;
        isInvincible = true;

        if (anim != null) anim.SetTrigger("doDash"); 
        if (sprite != null) sprite.color = new Color(1, 1, 1, 0.5f); 

        float originalGravity = rb.gravityScale; 
        rb.gravityScale = 0f; 
        rb.velocity = new Vector2(direction * dashSpeed, 0f); 

        yield return new WaitForSeconds(dashDuration); 

        rb.velocity = Vector2.zero; 
        rb.gravityScale = originalGravity; 
        isDashing = false; 
        isInvincible = false;

        if (sprite != null) sprite.color = new Color(1, 1, 1, 1f);
    }

    // --- [근접 일반 공격 패턴] ---
    IEnumerator AttackRoutine()
    {
        isAttacking = true;
        lastAttackTimeValue = Time.time;
        StopMovement();
        
        if (anim != null) anim.SetTrigger("doAttack"); 
        yield return new WaitForSeconds(0.4f); 
        ExecuteAttack();
        yield return new WaitForSeconds(0.4f); 
        isAttacking = false;
    }

    void ExecuteAttack() 
    { 
        Collider2D hit = Physics2D.OverlapCircle(attackPoint.position, hitRange, playerLayer); 
        if (hit != null) 
        { 
            Player p = hit.GetComponent<Player>(); 
            if (p != null) p.TakeDamage(1); 
        } 
    }

    void StopMovement() 
    { 
        rb.velocity = new Vector2(0, rb.velocity.y); 
        if (anim != null) anim.SetBool("isWalking", false); 
    }

    public void TakeDamage(int damage)
    {
        if (isInvincible) return; 
        if (health != null) health.TakeDamage(damage);
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawRay(transform.position, Vector2.down * groundCheckDistance);
        if (obstacleCheck != null)
        {
            float dir = transform.localScale.x;
            Gizmos.DrawRay(obstacleCheck.position, Vector2.right * dir * checkDistance);
        }

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, shockwaveRange);
        
        Gizmos.color = Color.red;
        if (attackPoint != null) Gizmos.DrawWireSphere(attackPoint.position, hitRange);
    }
}