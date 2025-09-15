using UnityEngine;
using UnityEngine.InputSystem; // XR 입력을 위해 추가

public class FirstPersonController : MonoBehaviour
{
    // --- [수정됨] VR 입력 및 카메라 참조 추가 ---
    [Header("VR Settings")]
    public InputActionProperty moveInputAction; // XR 컨트롤러 이동 액션을 연결할 변수
    public Transform cameraTransform; // VR 카메라(Main Camera)의 Transform을 연결할 변수

    // --- [추가됨] VR 회전 입력 관련 변수 ---
    [Header("VR Rotation Settings")]
    public InputActionProperty turnInputAction; // XR 컨트롤러 회전 액션을 연결할 변수
    public float turnSpeed = 90f; // 부드러운 회전 속도 (초당 각도)
    public float snapTurnAngle = 45f; // 스냅 턴 각도 (0으로 설정하면 부드러운 회전)
    
    private float lastTurnInput = 0f;

    [Header("Original Settings")]
    //public float mouseSensitivitX = 250f; // 마우스 코드는 제거되므로 주석 처리
    //public float mouseSensitivitY = 250f;
    public float walkSpeed = 8;
    public float jumpForse = 220;
    public LayerMask groundedMask;

    Vector3 moveAmount;
    Vector3 smoothMoveVelocity;

    //Transform cameraT; // cameraTransform 변수로 대체
    //float verticalLookRotation;

    Rigidbody rigidbody;
    bool Grounded;
    private Vector2 moveInput; // [추가됨] 입력 값을 저장할 변수

    void Awake()
    {
        rigidbody = GetComponent<Rigidbody>();
    }

    void Start()
    {
        // cameraTransform 변수가 인스펙터에서 할당되었는지 확인
        if (cameraTransform == null)
        {
            Debug.LogError("VR 카메라(Main Camera)의 Transform이 할당되지 않았습니다. 인스펙터에서 설정해주세요.");
        }
    }

    void Update()
    {
        // --- [제거됨] 마우스 회전 코드 ---
        // transform.Rotate(Vector3.up * Input.GetAxis("Mouse X") * Time.deltaTime * mouseSensitivitX);
        // verticalLookRotation += Input.GetAxis("Mouse Y") * Time.deltaTime * mouseSensitivitY;
        // verticalLookRotation = Mathf.Clamp(verticalLookRotation, -60, 60);
        // cameraT.localEulerAngles = Vector3.left * verticalLookRotation;

        // --- [수정됨] XR 컨트롤러 입력 받기 ---
        moveInput = moveInputAction.action.ReadValue<Vector2>();
        Vector3 moveDir = new Vector3(moveInput.x, 0, moveInput.y).normalized;
        
        Vector3 tagetMoveAmount = moveDir * walkSpeed;
        moveAmount = Vector3.SmoothDamp(moveAmount, tagetMoveAmount, ref smoothMoveVelocity, .15f);

        // --- [추가됨] XR 컨트롤러 회전 입력 처리 ---
        float currentTurnInput = turnInputAction.action.ReadValue<Vector2>().x;

        // 스냅 턴 로직
        if (snapTurnAngle > 0f)
        {
            // 입력이 데드존을 벗어났고, 이전 입력이 데드존 내부에 있었을 때만 회전
            if (Mathf.Abs(currentTurnInput) > 0.5f && Mathf.Abs(lastTurnInput) < 0.5f)
            {
                float angle = Mathf.Sign(currentTurnInput) * snapTurnAngle;
                transform.Rotate(transform.up, angle, Space.World);
            }
        }
        // 부드러운 회전 로직
        else
        {
            float angle = currentTurnInput * turnSpeed * Time.deltaTime;
            transform.Rotate(transform.up, angle, Space.World);
        }
        
        lastTurnInput = currentTurnInput;

        if (Input.GetButtonDown("Jump")) // 점프는 기존 버튼 입력 유지 (필요시 XR 입력으로 변경 가능)
        {
            if (Grounded)
            {
                rigidbody.AddForce(transform.up * jumpForse);
            }
        }

        Ray ray = new Ray(transform.position, -transform.up);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 1.1f, groundedMask)) // Ray 길이를 약간 늘려 안정성 확보
        {
            Grounded = true;
        }
        else
        {
            Grounded = false;
        }
    }

    void FixedUpdate()
    {
        // --- [수정됨] 이동 방향 기준을 VR 카메라로 변경 ---
        // 기존 코드: Vector3 localMove = transform.TransformDirection(moveAmount) * Time.fixedDeltaTime;

        // 1. 카메라의 정면과 오른쪽 방향을 가져옴
        Vector3 forward = cameraTransform.forward;
        Vector3 right = cameraTransform.right;

        // 2. 이동 방향을 현재 플레이어가 서 있는 평면에 맞게 투영 (행성 표면을 따라 걷기 위함)
        forward = Vector3.ProjectOnPlane(forward, transform.up).normalized;
        right = Vector3.ProjectOnPlane(right, transform.up).normalized;

        // 3. 최종 이동 방향 계산
        Vector3 worldMove = (forward * moveAmount.z + right * moveAmount.x) * Time.fixedDeltaTime;
        
        rigidbody.MovePosition(rigidbody.position + worldMove);
    }
}