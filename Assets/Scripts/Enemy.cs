using UnityEngine;
using System.Collections;

public class Enemy : MonoBehaviour
{
    [Header("능력치 설정")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float rotationSpeed = 5f;
    [SerializeField] private int attackDamage = 10;
    
    // WaveManager가 이 변수에 경로 정보를 자동으로 넣어줍니다.
    [HideInInspector]
    public Transform[] waypoints;

    // 애니메이터 컴포넌트 참조 변수
    private Animator _animator;
    
    private int waypointIndex = 0;
    private bool hasReachedEnd = false;
    private bool isAttacking = false;

    void Start()
    {
        _animator = GetComponent<Animator>();
        if (_animator == null)
        {
            Debug.LogWarning("Animator 컴포넌트를 찾을 수 없습니다. " + gameObject.name + "에 Animator를 추가했는지 확인해주세요.");
        }
    }

    void Update()
    {
        // 이미 공격 중이라면 더 이상 움직이지 않습니다.
        if (isAttacking) return;

        // 웨이포인트가 할당되지 않았거나, 모든 경로를 통과했다면
        if (waypoints == null || waypointIndex >= waypoints.Length)
        {
            // 경로 끝에 도달했으므로 공격을 시작합니다.
            hasReachedEnd = true;
            isAttacking = true;
            StopMovement();
            StartCoroutine(AttackBase());
            return;
        }

        // 현재 목표 웨이포인트를 설정합니다.
        Transform targetWaypoint = waypoints[waypointIndex];
        
        if (_animator != null)
        {
            _animator.SetBool("IsMoving", true);
        }

        // --- 행성 표면에 맞는 이동 및 회전 로직 ---
        Vector3 dir = (targetWaypoint.position - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(dir);
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * rotationSpeed);
        transform.Translate(Vector3.forward * moveSpeed * Time.deltaTime);

        // 목표 웨이포인트에 가까워지면 다음 목표를 설정합니다.
        if (Vector3.Distance(transform.position, targetWaypoint.position) < 0.5f)
        {
            waypointIndex++;
        }
    }

    // 트리거 충돌 감지 메서드 (본진과 충돌 시 호출)
    private void OnTriggerEnter(Collider other)
    {
        // 본진(HomeBase)과 충돌했을 때
        if (other.GetComponent<HomeBase>() != null)
        {
            // 아직 공격 중이 아니라면
            if (!isAttacking)
            {
                isAttacking = true;
                StopMovement();
                StartCoroutine(AttackBase());
            }
        }
    }
    
    // 본진을 주기적으로 공격하는 로직입니다.
    IEnumerator AttackBase()
    {
        while (true)
        {
            if (_animator != null)
            {
                _animator.SetTrigger("Attack");
            }

            if (HomeBase.Instance != null)
            {
                HomeBase.Instance.TakeDamage(attackDamage);
            }
            else
            {
                Debug.LogWarning("경고! HomeBase.Instance가 null입니다. 본진에 HomeBase 스크립트가 할당되었는지 확인하세요.");
            }
            yield return new WaitForSeconds(1f);
        }
    }

    public void StopMovement()
    {
        if (_animator != null)
        {
            _animator.SetBool("IsMoving", false);
        }
    }
    
    // --- 다른 스크립트에서 호출할 수 있는 애니메이션 관련 메서드 ---
    public void TakeHit()
    {
        if (_animator != null)
        {
            _animator.SetTrigger("GetHit");
        }
    }
    public void Die()
    {
        if (_animator != null)
        {
            _animator.SetTrigger("Die");
        }
    }
    public void CelebrateVictory()
    {
        if (_animator != null)
        {
            _animator.SetTrigger("Victory");
        }
    }
}