using UnityEngine;

public class PlanetBillboard : MonoBehaviour
{
    public Transform planetCenter;
    private Transform mainCameraTransform;

    void Awake()
    {
        mainCameraTransform = Camera.main.transform;
    }

    void LateUpdate()
    {
        if (mainCameraTransform == null || planetCenter == null) return;

        // UI의 회전을 계산합니다.
        // 1. 카메라의 회전 방향을 기준으로 UI의 앞 방향(forward)을 설정합니다.
        //    이것이 UI가 카메라를 바라보게 하는 핵심 로직입니다.
        transform.forward = mainCameraTransform.forward;

        // 2. UI의 상단 방향(up)을 행성 표면의 법선 방향(normal)과 일치시킵니다.
        //    행성 중심에서 현재 UI 위치까지의 벡터가 법선 방향입니다.
        Vector3 planetNormal = (transform.position - planetCenter.position).normalized;
        
        // 3. UI의 회전을 재계산하여 올바른 방향을 향하게 합니다.
        //    LookRotation을 사용하여 새로운 forward와 up 벡터를 기반으로 최종 회전을 만듭니다.
        //    이렇게 하면 UI가 플레이어를 바라보면서도 행성에 대해 수직을 유지합니다.
        Quaternion targetRotation = Quaternion.LookRotation(transform.forward, planetNormal);
        transform.rotation = targetRotation;
    }
}