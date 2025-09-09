using UnityEngine;

public class FirstPersonController : MonoBehaviour
{
    public float mouseSensitivitX = 250f;
    public float mouseSensitivitY = 250f;
    public float walkSpeed = 8;
    public float jumpForse = 220;
    public LayerMask groundedMask;

    Vector3 moveAmount;
    Vector3 smoothMoveVelocity;

    Transform cameraT;
    float verticalLookRotation;

    Rigidbody rigidbody;

    bool Grounded;

// Awake는 Start보다 먼저 호출됩니다.
    void Awake()
    {
        // Rigidbody 컴포넌트를 가져와서 rigidbody 변수에 할당합니다.
        rigidbody = GetComponent<Rigidbody>();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        cameraT = Camera.main.transform;
    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(Vector3.up * Input.GetAxis("Mouse X") * Time.deltaTime * mouseSensitivitX);
        verticalLookRotation += Input.GetAxis("Mouse Y") * Time.deltaTime * mouseSensitivitY;
        verticalLookRotation = Mathf.Clamp(verticalLookRotation, -60, 60);
        cameraT.localEulerAngles = Vector3.left * verticalLookRotation;

        Vector3 moveDir = new Vector3(Input.GetAxisRaw("Horizontal"),
                                   0, Input.GetAxisRaw("Vertical")).normalized;
        Vector3 tagetMoveAmount = moveDir * walkSpeed;
        moveAmount = Vector3.SmoothDamp(moveAmount, tagetMoveAmount, ref smoothMoveVelocity, .15f);

        if (Input.GetButtonDown("Jump"))
        {
            if (Grounded)
            {
                rigidbody.AddForce(transform.up * jumpForse);
            }
        }

        // Grounded check
		Ray ray = new Ray(transform.position, -transform.up);
		RaycastHit hit;
        if (Physics.Raycast(ray, out hit, groundedMask))
        {
            Grounded = true; // 땅에 닿았을 때 Grounded를 true로 설정
        }
        else
        {
            Grounded = false; // 땅에 닿지 않았을 때 Grounded를 false로 설정
        }

    }

    void FixedUpdate()
    {
        // Apply movement to rigidbody
        Vector3 localMove = transform.TransformDirection(moveAmount) * Time.fixedDeltaTime;
        rigidbody.MovePosition(rigidbody.position + localMove);
    }
}