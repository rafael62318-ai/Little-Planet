using UnityEngine;

public class FriendlyAlienAI : MonoBehaviour
{
    [Header("AI 설정")]
    public float searchRadius = 15f; // 적 탐지 범위
    public float attackRange = 2f;   // 공격 사거리
    public float moveSpeed = 5f;

    [Header("공격 설정")]
    public int attackDamage = 10;
    public float attackCooldown = 1.5f;
    public float attackRadius = 1.5f;          // 실제 타격 범위 (OverlapSphere)
    public GameObject attackEffectPrefab;      // 공격 이펙트 프리팹
    public AudioClip attackSound;              // 공격 사운드
    public Transform attackPoint;              // 판정 기준 위치 (없으면 본체 앞)

    [Header("죽음 설정")]
    public AudioClip deathSound;

    // 내부 변수
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

        InvokeRepeating(nameof(FindTarget), 0f, 0.5f);
    }

    void Update()
    {
        if (isDead) return;
        if (currentTarget == null) return;

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

    // 주변에서 가장 가까운 적 탐색
    void FindTarget()
    {
        if (isDead) return;
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

    // 이동
    void MoveTowardsTarget()
    {
        Vector3 dir = (currentTarget.position - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(dir);
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);

        rb.MovePosition(transform.position + transform.forward * moveSpeed * Time.deltaTime);
    }

    // 이동 멈춤
    void StopMovement()
    {
        rb.linearVelocity = Vector3.zero;
    }

    // 공격
    void Attack()
    {
        if (Time.time >= lastAttackTime + attackCooldown)
        {
            transform.LookAt(currentTarget);

            // 공격 애니메이션
            if (animator != null) animator.SetTrigger("Attack");

            // 공격 판정 (근접 범위)
            Vector3 point = attackPoint != null ? attackPoint.position : transform.position + transform.forward;
            Collider[] hits = Physics.OverlapSphere(point, attackRadius);
            foreach (Collider hit in hits)
            {
                if (hit.CompareTag("Enemy"))
                {
                    EnemyHealth targetHealth = hit.GetComponent<EnemyHealth>();
                    if (targetHealth != null)
                    {
                        targetHealth.TakeDamage(attackDamage);

                        // 죽은 적이면 타겟 초기화
                        if (targetHealth.currentHealth <= 0)
                            currentTarget = null;
                    }
                }
            }

            // 이펙트
            if (attackEffectPrefab != null)
                Instantiate(attackEffectPrefab, point, transform.rotation);

            // 사운드
            if (attackSound != null)
                audioSource.PlayOneShot(attackSound);

            lastAttackTime = Time.time;
        }
    }

    // 죽음 처리
    public void Die()
    {
        if (isDead) return;
        isDead = true;

        if (animator != null) animator.SetTrigger("Die");
        if (deathSound != null) audioSource.PlayOneShot(deathSound);

        rb.linearVelocity = Vector3.zero;
        CancelInvoke(nameof(FindTarget));

        Destroy(gameObject, 3f);
    }

    // 디버그: 공격 범위 표시
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Vector3 point = attackPoint != null ? attackPoint.position : transform.position + transform.forward;
        Gizmos.DrawWireSphere(point, attackRadius);
    }
}
