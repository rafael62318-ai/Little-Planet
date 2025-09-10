using UnityEngine;

public class FriendlyAlienAI : MonoBehaviour
{
    [Header("AI 설정")]
    public float searchRadius = 15f; //적을 탐지할 수 있는 최대 반겅
    public float attackRange = 5f; //공격이 가능한 최대 사거리
    public float moveSpeed = 5f;

    [Header("공격 설정")]
    public int attackDamage = 10; //적에게 가하는 공격력
    public float attackCooldown = 1.5f; //공격 속도

    //내부 변수
    private Transform currentTarget;
    private float lastAttackTime;
    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        //일정 시간마다 새로운 타겟을 찾는 탐색 루틴 시작
        InvokeRepeating("FindTarget", 0f, 0.5f);
    }

    void Update()
    {
        //현재 타겟이 없다면 아무것도 하지 않는다
        if (currentTarget == null)
        {
            return;
        }

        //타겟과의 거리를 계산한다
        float distanceToTarget = Vector3.Distance(transform.position, currentTarget.position);
        //타켓이 공격 사거리 안에 있다면 공격한다
        if (distanceToTarget <= attackRange)
        {
            StopMovement();
            Attack();
        }
        //타겟이 공격 사거리 밖에 있다면, 타켓을 향해 이동한다
        else
        {
            MoveTowardsTarget();
        }
    }

    //주변에서 가장 가까운 적을 찾는 함수
    void FindTarget()
    {
        //이미 유효한 타겟이 있다면 다시 찾지 않는다
        if (currentTarget != null) return;

        Collider[] colliders = Physics.OverlapSphere(transform.position, searchRadius);
        Transform closestEnemy = null;
        float minDistance = Mathf.Infinity;

        foreach (Collider col in colliders)
        {
            if (col.CompareTag("Enemy")) //"Enemy" 태그를 가진 오브잭트를 찾는다
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

    //타겟을 향해 이동하는 함수
    void MoveTowardsTarget()
    {
        //행성 표면을 따라 자연스럽게 회전 및 이동
        Vector3 dir = (currentTarget.position = transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(dir);
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);

        //Rigidbody를 이용해 물리적으로 이동
        rb.MovePosition(transform.position + transform.forward * moveSpeed * Time.deltaTime);
    }

    //이동을 멈추는 함수
    void StopMovement()
    {
        rb.linearVelocity = Vector3.zero; //물리적 속도를 0으로 만들어 멈춤
    }

    //타겟을 공격하는 함수
    void Attack()
    {
        //공격 쿨타임이 지났는지 확인
        if (Time.time >= lastAttackTime + attackCooldown)
        {
            //타겟을 향해 바라보도록 방향 고정
            transform.LookAt(currentTarget);

            Debug.Log(currentTarget.name + "을(를) 공격!");
            //여기에 공격 애니메이션, 사운드, 이펙트 재생 코드를 추가할 수 있습니다. 

            EnemyHealth targetHealth = currentTarget.GetComponent<EnemyHealth>();
            if (targetHealth != null)
            {
                targetHealth.TakeDamage(attackDamage);
            }
            else
            {
                //타겟이 파괴되거나 사라졌을 경우
                currentTarget = null;
            }

            lastAttackTime = Time.time;
        }
    }
}
