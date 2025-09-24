using UnityEngine;
using System.Collections;

public class Enemy : MonoBehaviour
{
    [Header("능력치 설정")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float rotationSpeed = 5f;
    [SerializeField] private int attackDamage = 10;

    [Header("폭발 설정")]
    public GameObject explosionPrefab; // 폭발 파티클 프리팹
    public AudioClip explosionSoundClip; // 폭발 사운드 클립
    public float destroyDelay = 0.5f; // 오브젝트 삭제 지연

    [Header("사운드 설정")]
    public AudioClip movingSoundClip; // 이동 효과음(발자국 등)을 담을 변수
    private AudioSource audioSource; // AudioSource 컴포넌트 참조 변수

    [Header("자동 파괴 설정")]
    [Tooltip("행성 중심으로부터 이 거리보다 멀어지면 파괴 타이머가 시작됩니다.")]
    public float maxDistanceFromPlanet = 50f;
    [Tooltip("행성 밖에서 이 시간(초)이 지나면 자동으로 파괴됩니다.")]
    public float destroyOutOfBoundsDelay = 5f;

    private Transform planetTransform;
    private float outOfBoundsTimer = 0f;
    private EnemyHealth enemyHealth; // EnemyHealth 스크립트 참조


    // WaveManager가 이 변수에 경로 정보를 자동으로 넣어줍니다.
    [HideInInspector]
    public Transform[] waypoints;

    // 애니메이터 컴포넌트 참조 변수
    private Animator _animator;
    private int waypointIndex = 0;
    private bool hasReachedEnd = false;
    private bool isAttacking = false;
    private bool isDead = false;

    void Start()
    {
        _animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
        if (_animator == null)
        {
            Debug.LogWarning("Animator 컴포넌트를 찾을 수 없습니다. " + gameObject.name + "에 Animator를 추가했는지 확인해주세요.");
        }
        GameObject planetObj = GameObject.FindGameObjectWithTag("Planet");
        if (planetObj != null)
        {
            planetTransform = planetObj.transform;
        }
        enemyHealth = GetComponent<EnemyHealth>();
    }

    void Update()
    {
        if (isAttacking || isDead) return;

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
                    // EnemyHealth의 Die 함수를 호출하여 골드 획득 및 파괴 처리
                    if (enemyHealth != null)
                    {
                        enemyHealth.Die(); 
                    }
                    else
                    {
                        Destroy(gameObject); // 만약 EnemyHealth가 없다면 그냥 파괴
                    }
                    return; 
                }
            }
            else
            {
                outOfBoundsTimer = 0f;
            }
        }
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
            // 이동 중이 아닐 때만 소리를 재생
            if (movingSoundClip != null && !audioSource.isPlaying)
            {
                audioSource.PlayOneShot(movingSoundClip);
            }

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
            // 폭발과 사운드를 재생
            PlayExplosion();

            // 기존의 공격 로직은 필요에 따라 유지
            if (!isAttacking)
            {
                isAttacking = true;
                StopMovement();
                StartCoroutine(AttackBase());
            }

            // 오브젝트를 파괴 (소리가 재생될 시간을 주기 위해 지연)
            Destroy(gameObject, destroyDelay);
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
            // 이동을 멈출 때 소리도 함께 멈춤
            if (audioSource != null && audioSource.isPlaying)
            {
                audioSource.Stop();
            }
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
    // 폭발 효과와 사운드를 재생하는 함수
    void PlayExplosion()
    {
    // 1) 폭발 이펙트 생성
        if (explosionPrefab != null)
        {
            Instantiate(explosionPrefab, transform.position, Quaternion.identity);
        }

    // 2) 폭발 사운드 재생
        if (explosionSoundClip != null)
        {
            AudioSource.PlayClipAtPoint(explosionSoundClip, transform.position);
        }
    }
}