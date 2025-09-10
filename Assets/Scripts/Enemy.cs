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
    
    private int waypointIndex = 0;
    private bool hasReachedEnd = false;

    void Update()
    {
        // 경로 끝에 도달했다면 더 이상 움직이지 않습니다.
        if (hasReachedEnd) return;
        
        // 웨이포인트가 설정되지 않았거나, 모든 경로를 통과했다면
        if (waypoints == null || waypointIndex >= waypoints.Length)
        {
            hasReachedEnd = true;
            StartCoroutine(AttackBase()); // 공격을 시작합니다.
            return;
        }

        // 현재 목표 웨이포인트를 설정합니다.
        Transform targetWaypoint = waypoints[waypointIndex];

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
            // HomeBase 스크립트의 싱글톤 인스턴스를 통해 공격합니다.
            if (HomeBase.Instance != null)
            {
                HomeBase.Instance.TakeDamage(attackDamage);
            }
            // 1초 대기 후 다시 공격합니다.
            yield return new WaitForSeconds(1f);
        }
    }
}