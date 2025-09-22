using UnityEngine;

public class Billboard : MonoBehaviour
{
    //  "BillboardType" 이라는 설계도(enum) 정의
    public enum BillboardType { LookAtCamera, CameraForward };

    [Header("Billboard Type")]
    //  위 설계도를 사용해 실제 변수(billboardType)를 선언
    [SerializeField] private BillboardType billboardType;

    [Header("Lock Rotation")]
    [SerializeField] private bool lockX;
    [SerializeField] private bool lockY;
    [SerializeField] private bool lockZ;

    private Vector3 originalRotation;
    private Transform mainCameraTransform;

    void Awake()
    {
        mainCameraTransform = Camera.main.transform;
        originalRotation = transform.rotation.eulerAngles;
    }

    void LateUpdate()
    {
        if (mainCameraTransform == null) return;

        // 3. 선언된 billboardType 변수를 사용
        switch (billboardType)
        {
            case BillboardType.LookAtCamera:
                transform.LookAt(mainCameraTransform.position, Vector3.up);
                break;
            case BillboardType.CameraForward:
                transform.forward = mainCameraTransform.forward;
                break;
        }

        //(축 잠금 로직)
        Vector3 rotation = transform.rotation.eulerAngles;
        if (lockX) { rotation.x = originalRotation.x; }
        if (lockY) { rotation.y = originalRotation.y; }
        if (lockZ) { rotation.z = originalRotation.z; }
        transform.rotation = Quaternion.Euler(rotation);
    }
}