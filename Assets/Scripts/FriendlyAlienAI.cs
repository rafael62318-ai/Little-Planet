using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(GravityBody))]
public class FriendlyAlienAI : MonoBehaviour
{
    [Header("AI 설정")]
    public float searchRadius = 15f;
    public float attackRange = 5f;
    public float moveSpeed = 5f;

    [Header("공격 설정")]
    public int attackDamage = 10;
    public float attackCooldown = 1.5f;
    public GameObject attackEffectPrefab;
    public AudioClip attackSoundClip;

    // ★★★ 새로 추가된 부분 시작 ★★★
    [Header("자동 파괴 설정")]
    [Tooltip("행성 중심으로부터 이 거리보다 멀어지면 파괴 타이머가 시작됩니다.")]
    public float maxDistanceFromPlanet = 50f;
    [Tooltip("행성 밖에서 이 시간(초)이 지나면 자동으로 파괴됩니다.")]
    public float destroyOutOfBoundsDelay = 5f;
    
    private Transform planetTransform;
    private float outOfBoundsTimer = 0f;
    // ★★★ 새로 추가된 부분 끝 ★★★

    //내부 변수
    private Transform currentTarget;
    private float lastAttackTime;
    private Rigidbody rb;
    private Animator animator;
    private AudioSource audioSource;
    private bool isDead = false; // Die 함수와 연동하기 위해 추가

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null) audioSource = gameObject.AddComponent<AudioSource>();

        // 기존 애니메이션 로직
        animator.SetBool("isAttacking", false);

        // ★★★ 시작할 때 행성의 위치를 찾아 저장합니다. ★★★
        GameObject planetObj = GameObject.FindGameObjectWithTag("Planet");
        if (planetObj != null)
        {
            planetTransform = planetObj.transform;
        }

        InvokeRepeating("FindTarget", 0f, 0.5f);
    }

    void Update()
    {
        if (isDead) return;

        // ★★★ 새로 추가된 자동 파괴 로직 ★★★
        if (planetTransform != null)
        {
            float distance = Vector3.Distance(transform.position, planetTransform.position);
            if (distance > maxDistanceFromPlanet)
            {
                outOfBoundsTimer += Time.deltaTime;
                if (outOfBoundsTimer >= destroyOutOfBoundsDelay)
                {
                    Debug.Log(gameObject.name + "가 전장을 이탈하여 소멸합니다.");
                    Die(); 
                    return; 
                }
            }
            else
            {
                outOfBoundsTimer = 0f;
            }
        }
        // ★★★ 자동 파괴 로직 끝 ★★★

        // --- 이하 기존 로직 ---
        if (currentTarget == null)
        {
            animator.SetBool("isAttacking", false);
            return;
        }
        
        float distanceToTarget = Vector3.Distance(transform.position, currentTarget.position);
        
        if (distanceToTarget <= attackRange)
        {
            StopMovement();
            Attack();
        }
        else
        {
            MoveTowardsTarget();
        }
    }

    void FindTarget()
    {
        if (currentTarget != null) return;

        Collider[] colliders = Physics.OverlapSphere(transform.position, searchRadius);
        Transform closestEnemy = null;
        float minDistance = Mathf.Infinity;

        foreach (Collider col in colliders)
        {
            if (col.CompareTag("Enemy"))
            {
                float distance = Vector3.Distance(transform.position, col.transform.position);
                if (distance < minDistance)
                {
                    minDistance = distance;
                    closestEnemy = col.transform;
                }
            }
        }
        currentTarget = closestEnemy;
    }

    void MoveTowardsTarget()
    {
        animator.SetBool("isAttacking", false);
        
        Vector3 dir = (currentTarget.position - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(dir);
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);
        
        rb.MovePosition(transform.position + transform.forward * moveSpeed * Time.deltaTime);
    }

    void StopMovement()
    {
        rb.linearVelocity = Vector3.zero;
    }

    void Attack()
    {
        if (Time.time >= lastAttackTime + attackCooldown)
        {
            animator.SetBool("isAttacking", true);
            transform.LookAt(currentTarget);

            if (audioSource != null && attackSoundClip != null)
            {
                audioSource.PlayOneShot(attackSoundClip);
            }
            
            if (attackEffectPrefab != null)
            {
                GameObject effect = Instantiate(attackEffectPrefab, transform.position, Quaternion.identity);
                Destroy(effect, 2f);
            }
            
            EnemyHealth targetHealth = currentTarget.GetComponent<EnemyHealth>();
            if (targetHealth != null)
            {
                targetHealth.TakeDamage((int)attackDamage);
                
                if(targetHealth.IsDead())
                {
                     currentTarget = null;
                     animator.SetBool("isAttacking", false);
                }
            }
            else
            {
                currentTarget = null;
                animator.SetBool("isAttacking", false);
            }
            lastAttackTime = Time.time;
        }
    }
    
    // ★★★ 자동 파괴 로직을 위한 Die 함수 추가 ★★★
    public void Die()
    {
        if (isDead) return;
        isDead = true;
        Debug.LogWarning(gameObject.name + "가 파괴되었습니다!");
        // 모든 행동을 멈추고 3초 후에 오브젝트를 파괴합니다.
        Destroy(gameObject, 3f);
    }
}