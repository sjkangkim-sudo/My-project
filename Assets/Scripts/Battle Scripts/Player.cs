using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour
{
    private Health health; 

    [Header("이동 및 슬라이딩(Down) 설정")]
    public float walkSpeed = 5f;      
    public float dashSpeed = 20f;    
    public float dashDuration = 0.2f; 
    public float dashDelay = 1.0f;
    private bool isDashing = false;
    public bool canDash = true;

    private bool isInvincible = false;

    [Header("스킬 설정 (C키)")]
    public float skillDashForce = 15f;    
    public float skillJumpForce = 10f;    
    public float skillDuration = 0.4f;    
    public float skillCDelay = 2.0f;
    private bool isUsingSkill = false;
    public bool canSkillC = true;

    [Header("스킬1 설정 (X키 - 칼 던지기)")]
    public GameObject knifePrefab;        
    public Transform firePoint;           
    public float skill1Delay = 3f;      
    private bool isUsingSkill1 = false;
    public bool canSkillX = true;

    [Header("콤보 설정")]
    public int comboStep = 0;           
    public float comboWaitTime = 1.0f;  
    private float lastComboTime = 0f;   

    [Header("공격 판정 설정")]
    public Transform attackPoint;    
    public float attackRange = 0.6f; 
    public LayerMask enemyLayers;    
    public int attackDamage = 1; 

    [Header("기타 설정")]
    public float jumpForce = 14f;
    private bool isGrounded;
    
    private Rigidbody2D rb;
    private SpriteRenderer sprite;
    private Animator anim;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        sprite = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
        rb.gravityScale = 3.5f;

        health = GetComponent<Health>();
    }

    void Update()
    {
        if (comboStep > 0 && Time.time - lastComboTime > comboWaitTime)
        {
            comboStep = 0;
            if (anim != null) anim.SetInteger("comboCount", 0);
        }

        if (!isDashing && !isUsingSkill && !isUsingSkill1)
        {
            HandleMove();
            HandleJump();
            HandleDash();  
            HandleSkill(); 
            HandleSkill1();
            HandleCombat(); 
        }
    }

    public void TakeDamage(int damage)
    {
        if (isInvincible) return; 

        if (health != null) health.TakeDamage(damage);
    }

    void ExecuteAttack()
    {
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, enemyLayers);
        foreach (Collider2D enemy in hitEnemies)
        {
            Health enemyHealth = enemy.GetComponent<Health>();
            if (enemyHealth != null)
            {
                enemyHealth.TakeDamage(1); 
            }
        }
    }

    // --- 스킬 1 (X키: 칼 던지기) --- //
    void HandleSkill1() 
    { 
        if (Input.GetKeyDown(KeyCode.X) && isGrounded && canSkillX) 
        {
            StartCoroutine(Skill1Routine()); 
        }
    }

    IEnumerator Skill1Routine() 
    { 
        isUsingSkill1 = true; 
        canSkillX = false; 
        rb.velocity = Vector2.zero; 
        if (anim != null) anim.SetTrigger("doSkill1"); 

        yield return new WaitForSeconds(0.1f); 
        ThrowKnife(); 

        yield return new WaitForSeconds(0.2f); 
        isUsingSkill1 = false; 

        yield return new WaitForSeconds(skill1Delay); 
        canSkillX = true; 
    }

    void ThrowKnife()
    {
        if (knifePrefab == null || firePoint == null) return;
        float angle = transform.localScale.x > 0 ? 180 : 0;
        Quaternion rotation = Quaternion.Euler(0, 0, angle);
        
        GameObject knife = Instantiate(knifePrefab, firePoint.position, rotation);

        Collider2D knifeCol = knife.GetComponent<Collider2D>();
        Collider2D playerCol = GetComponent<Collider2D>();

        if (knifeCol != null && playerCol != null)
        {
            Physics2D.IgnoreCollision(knifeCol, playerCol);
        }
    }

    // --- 스킬 (C키: 돌진 점프 공격) --- //
    void HandleSkill() 
    { 
        if (Input.GetKeyDown(KeyCode.C) && isGrounded && canSkillC) 
        {
            StartCoroutine(SkillRoutine()); 
        }
    }

    IEnumerator SkillRoutine() 
    { 
        isUsingSkill = true; 
        canSkillC = false;
        isGrounded = false; 
        if (anim != null) anim.SetTrigger("doSkill"); 
        float dir = transform.localScale.x * -1f; 
        rb.velocity = Vector2.zero; 
        rb.AddForce(new Vector2(dir * skillDashForce, skillJumpForce), ForceMode2D.Impulse); 
        
        float elapsed = 0f; 
        while (elapsed < skillDuration) 
        { 
            ExecuteAttack(); 
            elapsed += Time.deltaTime; 
            yield return null; 
        } 
        
        isUsingSkill = false; 

        yield return new WaitForSeconds(skillCDelay);
        canSkillC = true; 
    }

    // --- 이동 (AI용: 왼쪽 원본 스프라이트 기준) --- //
    void HandleMove() 
    { 
        float moveInput = 0; 
        if (Input.GetKey(KeyCode.RightArrow)) moveInput = 1f; 
        else if (Input.GetKey(KeyCode.LeftArrow)) moveInput = -1f; 

        rb.velocity = new Vector2(moveInput * walkSpeed, rb.velocity.y); 

        if (anim != null) anim.SetBool("isWalking", moveInput != 0); 
        
        if (moveInput > 0) transform.localScale = new Vector3(-1, 1, 1); 
        else if (moveInput < 0) transform.localScale = new Vector3(1, 1, 1); 
    }

    // --- 점프 로직 --- //
    void HandleJump() 
    { 
        if (Input.GetKeyDown(KeyCode.UpArrow) && isGrounded) 
        { 
            rb.velocity = new Vector2(rb.velocity.x, jumpForce); 
            isGrounded = false; 
            if (anim != null) anim.SetBool("isGrounded", false); 
        } 
    }

    // --- 슬라이딩 --- //
    void HandleDash() 
    { 
        if (Input.GetKeyDown(KeyCode.DownArrow) && isGrounded && canDash) 
        { 
            float dir = transform.localScale.x * -1f; 
            StartCoroutine(DashRoutine(dir)); 
        } 
    }

    IEnumerator DashRoutine(float direction) 
    { 
        isDashing = true; 
        canDash = false;
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

        yield return new WaitForSeconds(dashDelay); 
        canDash = true; 
    }

    // --- 기본 공격 (Z키) --- //
    void HandleCombat() 
    { 
        if (Input.GetKeyDown(KeyCode.Z)) 
        { 
            lastComboTime = Time.time; 
            if (!isGrounded) 
            { 
                if (anim != null) anim.SetTrigger("doJumpAttack"); 
            } 
            else 
            { 
                comboStep++; 
                if (comboStep > 2) comboStep = 1; 
                if (anim != null) 
                { 
                    anim.SetInteger("comboCount", comboStep); 
                    anim.SetTrigger("doAttack"); 
                } 
            } 
        } 
    }

    private void OnCollisionEnter2D(Collision2D collision) 
    { 
        if (collision.gameObject.CompareTag("Ground")) 
        { 
            isGrounded = true; 
            if (anim != null) anim.SetBool("isGrounded", true); 
        } 
    }

    void OnDrawGizmosSelected() 
    { 
        if (attackPoint == null) return; 
        Gizmos.color = Color.red; 
        Gizmos.DrawWireSphere(attackPoint.position, attackRange); 
    }
}