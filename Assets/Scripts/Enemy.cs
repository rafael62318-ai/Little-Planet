using UnityEngine;
using System.Collections;

public class Enemy : MonoBehaviour
{
    [Header("능력치 설정")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float rotationSpeed = 5f;
    [SerializeField] private int attackDamage = 10;

    [Header("애니메이션 설정")]
    private Animator animator;
    private Rigidbody rb;
    private bool isDead = false;
    private bool isHit = false; // 피격 중 이동 정지

    [HideInInspector] public Transform[] waypoints; // WaveManager에서 설정
    private int waypointIndex = 0;
    private bool hasReachedEnd = false;

    void Start()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();

        if (animator != null)
            animator.SetBool("IsMoving", false); // 시작 IdleBattle
    }

    void Update()
    {
        if (isDead) return;

        // 피격 중일 때 이동 정지
        if (isHit)
        {
            if (animator != null) animator.SetBool("IsMoving", false);
            if (rb != null) rb.linearVelocity = Vector3.zero; // 관성 제거
            return;
        }

        // 경로 끝 → 본진 공격
        if (hasReachedEnd) return;
        if (waypoints == null || waypointIndex >= waypoints.Length)
        {
            hasReachedEnd = true;
            if (animator != null) animator.SetBool("IsMoving", false);
            StartCoroutine(AttackBase());
            return;
        }

        // 이동 처리
        Transform targetWaypoint = waypoints[waypointIndex];
        Vector3 dir = (targetWaypoint.position - transform.position).normalized;

        // 회전
        Quaternion lookRotation = Quaternion.LookRotation(dir);
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * rotationSpeed);

        // Rigidbody 이동
        if (rb != null)
        {
            rb.MovePosition(transform.position + transform.forward * moveSpeed * Time.deltaTime);
        }

        if (animator != null) animator.SetBool("IsMoving", true);

        // 웨이포인트 도착 판정
        if (Vector3.Distance(transform.position, targetWaypoint.position) < 0.5f)
        {
            waypointIndex++;
        }
    }

    IEnumerator AttackBase()
    {
        while (!isDead)
        {
            if (HomeBase.Instance != null)
            {
                if (animator != null) animator.SetTrigger("Attack");
                HomeBase.Instance.TakeDamage(attackDamage);
            }
            yield return new WaitForSeconds(1f);
        }
    }

    // 피격 처리
    public void GetHit()
    {
        if (isDead) return;

        isHit = true;
        if (animator != null) animator.SetTrigger("GetHit");

        StartCoroutine(RecoverFromHitByAnim());
    }

    private IEnumerator RecoverFromHitByAnim()
    {
        // 애니메이션 전환 반영 대기
        yield return null;

        if (animator != null)
        {
            AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
            float hitAnimLength = stateInfo.length;
            yield return new WaitForSeconds(hitAnimLength);
        }

        isHit = false;
    }

    // 죽음 처리
    public void Die()
    {
        if (isDead) return;
        isDead = true;

        StopAllCoroutines();
        if (animator != null) animator.SetTrigger("Die");

        if (rb != null) rb.linearVelocity = Vector3.zero; // 멈춤
        moveSpeed = 0f;

        Destroy(gameObject, 3f);
    }
}
