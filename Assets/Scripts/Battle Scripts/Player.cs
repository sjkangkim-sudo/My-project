using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement; 

public class Player : MonoBehaviour
{
    private Health health; 

    [Header("Key Manager 연결")]
    public KeyManager keyManager; 

    [Header("이동 및 슬라이딩(Down) 설정")]
    public float walkSpeed = 5f;
    public float dashSpeed = 20f;    
    public float dashDuration = 0.2f; 
    public float dashDelay = 1.0f;
    private bool isDashing = false;
    public bool canDash = true;
    private bool isInvincible = false;

    [Header("스킬 설정 (C키 - 충격파/지진)")]
    public float shockwaveRange = 3.5f;    
    public int shockwaveDamage = 2;        
    public float skillCDelay = 2.0f;       
    public float skillDuration = 0.5f;
    private bool isUsingSkill = false;
    public bool canSkillC = true;

    [Header("스킬 연출 설정")]
    public GameObject shockwaveEffectPrefab; 
    public Transform skillSpawnPoint; 

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

    [Header("바닥 체크 및 점프 설정")]
    public float jumpForce = 14f;
    public float groundCheckDistance = 0.7f; 
    public LayerMask groundLayer;           
    private bool isGrounded;
    
    private Rigidbody2D rb;
    private SpriteRenderer sprite;
    private Animator anim;

    [Header("사망 및 씬 전환 설정")]
    private bool isDead = false;
    public string deathCutsceneSceneName = "GameOverScene"; 

    void Start()
    {
        isDead = false;

        rb = GetComponent<Rigidbody2D>();
        sprite = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
        health = GetComponent<Health>();

        if (rb != null)
        {
            rb.isKinematic = false;
            rb.gravityScale = 3.5f;
        }

        if (KeyManager.instance != null) keyManager = KeyManager.instance;
        else if (keyManager == null) keyManager = FindObjectOfType<KeyManager>();

        if (keyManager != null) keyManager.InitKeyDictionary();
    }

    void Update()
    {
        if (isDead) return;
        if (Time.timeScale == 0) return;
        if (keyManager == null && KeyManager.instance == null) return;

        CheckGrounded();

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

    private KeyCode GetSafeKey(string actionName, KeyCode defaultKey)
    {
        if (keyManager == null) keyManager = KeyManager.instance ?? FindObjectOfType<KeyManager>();

        if (keyManager != null && keyManager.keys != null && keyManager.keys.ContainsKey(actionName))
        {
            return keyManager.keys[actionName];
        }

        string savedKey = PlayerPrefs.GetString("Key_" + actionName, defaultKey.ToString());
        return (KeyCode)System.Enum.Parse(typeof(KeyCode), savedKey);
    }

    void CheckGrounded()
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, groundCheckDistance, groundLayer);
        isGrounded = hit.collider != null;
        if (anim != null) anim.SetBool("isGrounded", isGrounded);
    }

    public void TakeDamage(int damage)
    {
        if (isDead || isInvincible) return; 
        if (health != null) health.TakeDamage(damage);
    }

    public void Die()
    {
        if (isDead) return; 

        isDead = true;
        rb.velocity = Vector2.zero; 
        rb.isKinematic = true;      

        if (anim != null)
        {
            anim.SetBool("isWalking", false);
            anim.SetInteger("comboCount", 0);
            
            // 애니메이터에 세팅한 doDie 트리거를 작동시킵니다!
            anim.SetTrigger("doDie"); 
        }

        StartCoroutine(PlayDeathCutsceneRoutine());
    }

    IEnumerator PlayDeathCutsceneRoutine()
    {
        // 사망 애니메이션이 끝까지 재생될 수 있도록 여유 있게 1.2초 대기 후 넘깁니다.
        yield return new WaitForSeconds(1.2f);

        if (!string.IsNullOrEmpty(deathCutsceneSceneName))
        {
            SceneManager.LoadScene(deathCutsceneSceneName);
        }
        else
        {
            Debug.LogError("⚠️ 이동할 컷씬 씬 이름이 비어있습니다! Player 인스펙터창을 확인하세요.");
        }
    }

    void ExecuteAttack()
    {
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, enemyLayers);
        foreach (Collider2D enemy in hitEnemies)
        {
            Health enemyHealth = enemy.GetComponent<Health>();
            if (enemyHealth != null) enemyHealth.TakeDamage(attackDamage); 
        }
    }

    void HandleSkill1() 
    { 
        if (Input.GetKeyDown(GetSafeKey("Skill_X", KeyCode.X)) && isGrounded && canSkillX) 
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

        if (SoundManager.instance != null) SoundManager.instance.PlayPlayerSkillX();

        float angle = transform.localScale.x > 0 ? 180 : 0;
        Quaternion rotation = Quaternion.Euler(0, 0, angle);
        GameObject knife = Instantiate(knifePrefab, firePoint.position, rotation);
        Physics2D.IgnoreCollision(knife.GetComponent<Collider2D>(), GetComponent<Collider2D>());
    }

    void HandleSkill() 
    { 
        if (Input.GetKeyDown(GetSafeKey("Skill_C", KeyCode.C)) && isGrounded && canSkillC) 
        {
            StartCoroutine(SkillRoutine()); 
        }
    }

    IEnumerator SkillRoutine() 
    { 
        isUsingSkill = true; 
        canSkillC = false;
        rb.velocity = Vector2.zero; 
        
        if (anim != null) anim.SetTrigger("doSkill"); 

        if (SoundManager.instance != null) SoundManager.instance.PlayPlayerSkillC();
        
        yield return new WaitForSeconds(0.2f); 
        
        ExecuteShockwave(); 
        
        yield return new WaitForSeconds(skillDuration);
        isUsingSkill = false; 
        yield return new WaitForSeconds(skillCDelay);
        canSkillC = true; 
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

        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(transform.position, shockwaveRange, enemyLayers);
        foreach (Collider2D enemy in hitEnemies)
        {
            Health enemyHealth = enemy.GetComponent<Health>();
            if (enemyHealth != null) enemyHealth.TakeDamage(shockwaveDamage);
        }
    }

    void HandleMove() 
    { 
        float moveInput = 0; 
        
        if (Input.GetKey(GetSafeKey("Right", KeyCode.RightArrow))) moveInput = 1f; 
        else if (Input.GetKey(GetSafeKey("Left", KeyCode.LeftArrow))) moveInput = -1f; 

        rb.velocity = new Vector2(moveInput * walkSpeed, rb.velocity.y); 
        if (anim != null) anim.SetBool("isWalking", moveInput != 0); 
        
        if (moveInput > 0) transform.localScale = new Vector3(-1, 1, 1); 
        else if (moveInput < 0) transform.localScale = new Vector3(1, 1, 1); 
    }

    void HandleJump() 
    { 
        if (Input.GetKeyDown(GetSafeKey("Jump", KeyCode.UpArrow)) && isGrounded) 
        { 
            rb.velocity = new Vector2(rb.velocity.x, jumpForce); 
            isGrounded = false; 
        } 
    }

    void HandleDash() 
    { 
        if (Input.GetKeyDown(GetSafeKey("Dash", KeyCode.DownArrow)) && isGrounded && canDash) 
        { 
            float dir = transform.localScale.x * -1f; 
            StartCoroutine(DashRoutine(dir)); 
        } 
    }

    IEnumerator DashRoutine(float direction) 
    { 
        isDashing = true; canDash = false; isInvincible = true;

        if (SoundManager.instance != null) SoundManager.instance.PlayPlayerDash();

        if (anim != null) anim.SetTrigger("doDash"); 
        if (sprite != null) sprite.color = new Color(1, 1, 1, 0.5f);
        float originalGravity = rb.gravityScale; 
        rb.gravityScale = 0f; 
        rb.velocity = new Vector2(direction * dashSpeed, 0f); 
        yield return new WaitForSeconds(dashDuration); 
        rb.velocity = Vector2.zero; 
        rb.gravityScale = originalGravity; 
        isDashing = false; isInvincible = false;
        if (sprite != null) sprite.color = new Color(1, 1, 1, 1f);
        yield return new WaitForSeconds(dashDelay); 
        canDash = true; 
    }

    void HandleCombat() 
    { 
        if (Input.GetKeyDown(GetSafeKey("Attack", KeyCode.Z))) 
        { 
            lastComboTime = Time.time; 
            if (!isGrounded) 
            { 
                if (anim != null) anim.SetTrigger("doJumpAttack"); 
                if (SoundManager.instance != null) SoundManager.instance.PlayPlayerAttack();
            } 
            else 
            { 
                comboStep++; if (comboStep > 2) comboStep = 1; 
                if (anim != null) 
                { 
                    anim.SetInteger("comboCount", comboStep); 
                    anim.SetTrigger("doAttack"); 
                    if (SoundManager.instance != null) SoundManager.instance.PlayPlayerAttack();
                } 
            } 
        } 
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red; Gizmos.DrawRay(transform.position, Vector2.down * groundCheckDistance);
        Gizmos.color = Color.yellow; Gizmos.DrawWireSphere(transform.position, shockwaveRange);
    }
}