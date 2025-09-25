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
    
    [HideInInspector]
    public PocketReleaseGravity spawner;

    //내부 변수
    private Transform currentTarget;
    private float lastAttackTime;
    private Rigidbody rb;
    private Animator animator;
    private AudioSource audioSource;
    private bool isDead = false;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null) audioSource = gameObject.AddComponent<AudioSource>();
        animator.SetBool("isAttacking", false);
        InvokeRepeating("FindTarget", 0f, 0.5f);
    }

    void Update()
    {
        if (isDead) return;
        
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
    
    public void Die()
    {
        if (isDead) return;
        isDead = true;

        // 만약 자신을 생성한 '스포너'가 있다면, 부활을 요청합니다.
        if (spawner != null)
        {
            spawner.StartRespawn();
        }

        Debug.LogWarning(gameObject.name + "가 파괴되었습니다!");
        Destroy(gameObject, 3f);
    }
}
