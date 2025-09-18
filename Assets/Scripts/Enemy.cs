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

    // 게임 시작 시 애니메이터 컴포넌트를 찾습니다.
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
        // 경로 끝에 도달했다면 더 이상 움직이지 않습니다.
        if (hasReachedEnd)
        {
            // 정지 상태이므로 IsMoving을 false로 설정합니다.
            if (_animator != null)
            {
                _animator.SetBool("IsMoving", false);
            }
            return;
        }

        // 웨이포인트가 설정되지 않았거나, 모든 경로를 통과했다면
        if (waypoints == null || waypointIndex >= waypoints.Length)
        {
            hasReachedEnd = true;
            // 이동을 멈추고 공격을 시작합니다.
            StartCoroutine(AttackBase());
            return;
        }

        // 현재 목표 웨이포인트를 설정합니다.
        Transform targetWaypoint = waypoints[waypointIndex];

        // 이동 중이므로 IsMoving을 true로 설정합니다.
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
    
    // 본진을 주기적으로 공격하는 로직입니다.
    IEnumerator AttackBase()
    {
        while (true)
        {
            // 공격 애니메이션 트리거를 실행합니다.
            if (_animator != null)
            {
                _animator.SetTrigger("Attack");
            }

            // HomeBase 스크립트의 싱글톤 인스턴스를 통해 공격합니다.
            if (HomeBase.Instance != null)
            {
                HomeBase.Instance.TakeDamage(attackDamage);
            }
            
            // 1초 대기 후 다시 공격합니다.
            // 애니메이션 길이에 맞춰 yield return 시간을 조절하면 더 자연스럽습니다.
            yield return new WaitForSeconds(1f);
        }
    }

    // --- 다른 스크립트에서 호출할 수 있는 애니메이션 관련 메서드 ---
    
    // 공격을 받았을 때 호출합니다.
    public void TakeHit()
    {
        if (_animator != null)
        {
            _animator.SetTrigger("GetHit");
        }
    }

    // 적이 죽었을 때 호출합니다.
    public void Die()
    {
        if (_animator != null)
        {
            _animator.SetTrigger("Die");
        }
    }

    // 게임에서 승리했을 때 호출합니다.
    public void CelebrateVictory()
    {
        if (_animator != null)
        {
            _animator.SetTrigger("Victory");
        }
    }
}