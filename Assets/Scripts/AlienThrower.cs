using UnityEngine;
using System;
using System.Collections; // 코루틴을 사용하기 위해 필요합니다.

public class AlienThrower : MonoBehaviour
{
    [Header("던지기 횟수 설정")]
    [Tooltip("최대로 던질 수 있는 아군 외계인 횟수입니다.")]
    public int maxThrows = 10;

    public event Action<int> OnThrowCounbtChanged;

    [Header("프리팹 설정")]
    [Tooltip("던질 아군 외계인 유닛의 프리팹입니다.")]
    public GameObject friendlyAlienPrefab;

    [Header("발사 설정")]
    [Tooltip("유닛을 꺼내는 연출이 시작될 위치입니다. (바구니 안쪽 중앙)")]
    public Transform basketCenterPoint; 

    [Tooltip("유닛이 발사될 최종 위치입니다. (보통 카메라 앞)")]
    public Transform throwPoint;
    
    [Tooltip("외계인을 던지는 힘의 크기입니다.")]
    public float throwForce = 20f;

    [Header("연출 설정")]
    [Tooltip("바구니에서 유닛을 꺼내는 데 걸리는 시간입니다.")]
    public float pullOutDuration = 0.5f; 

    [Header("쿨타임 설정")]
    public float throwCooldown = 1.0f;
    
    private float lastThrowTime;
    private bool isThrowing = false; // 현재 던지는 연출이 진행 중인지 확인하는 변수
    private int remainingThrows;

    void Start()
    {
        remainingThrows = maxThrows;
        OnThrowCounbtChanged?.Invoke(remainingThrows);
    }

    void Update()
    {
        // 마우스 왼쪽 버튼을 클릭했고, 쿨타임이 지났고, 현재 던지는 중이 아닐 때
        if (Input.GetMouseButtonDown(0) &&remainingThrows > 0 && Time.time > lastThrowTime + throwCooldown && !isThrowing)
        {
            StartCoroutine(ThrowAlienSequence()); // 일반 함수 호출 대신 코루틴을 시작합니다.
        }
    }

    // 시간차를 두고 연출과 발사를 진행하는 코루틴 함수
    IEnumerator ThrowAlienSequence()
    {
        // --- 1. 준비 단계 ---
        isThrowing = true; // 던지는 중이라고 표시
        lastThrowTime = Time.time; // 쿨타임 계산 시작

        if (friendlyAlienPrefab == null || throwPoint == null || basketCenterPoint == null)
        {
            Debug.LogError("필요한 Transform 변수가 설정되지 않았습니다!");
            isThrowing = false;
            yield break; // 코루틴 즉시 종료
        }

        remainingThrows--;
        OnThrowCounbtChanged?.Invoke(remainingThrows);
        Debug.Log("아군 유닛 투척! 남은 횟수: " + remainingThrows);

        // --- 2. '꺼내는' 연출 단계 ---
        // 바구니 중앙에 작은 크기로 유닛 생성
        GameObject alienInstance = Instantiate(friendlyAlienPrefab, basketCenterPoint.position, basketCenterPoint.rotation);

        FriendlyAlienAI alienAI = alienInstance.GetComponent<FriendlyAlienAI>();
        if (alienAI != null) alienAI.enabled = false;

        Vector3 originalScale = alienInstance.transform.localScale;
        alienInstance.transform.localScale = Vector3.zero; // 처음엔 보이지 않도록 크기를 0으로 설정

        // 물리 효과를 잠시 꺼서 연출 도중 멋대로 움직이지 않게 함
        Rigidbody rb = alienInstance.GetComponent<Rigidbody>();
        if (rb != null) rb.isKinematic = true;

        float elapsedTime = 0f;
        while (elapsedTime < pullOutDuration)
        {
            // 시간에 따라 위치와 크기를 부드럽게 변경 (Lerp 사용)
            alienInstance.transform.position = Vector3.Lerp(basketCenterPoint.position, throwPoint.position, elapsedTime / pullOutDuration);
            alienInstance.transform.localScale = Vector3.Lerp(Vector3.zero, originalScale, elapsedTime / pullOutDuration);
            
            elapsedTime += Time.deltaTime;
            yield return null; // 다음 프레임까지 대기
        }

        // 연출이 끝난 후 위치와 크기를 정확하게 맞춰줌
        alienInstance.transform.position = throwPoint.position;
        alienInstance.transform.localScale = originalScale;

        // --- 3. 발사 단계 ---
        if (rb != null)
        {
            rb.isKinematic = false; // 물리 효과 다시 켜기
            rb.AddForce(throwPoint.forward * throwForce, ForceMode.Impulse);

            if (alienAI != null) alienAI.enabled = true;
        }
        
        isThrowing = false; // 던지기 완료
    }
}
