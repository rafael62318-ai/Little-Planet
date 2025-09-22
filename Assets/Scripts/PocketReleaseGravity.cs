using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable))]
public class PocketReleaseGravity : MonoBehaviour
{
    private Rigidbody rb;
    private UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable grab;

    [SerializeField] private Collider[] pocketColliders;

    private Vector3 startLocalPos;
    private Quaternion startLocalRot;
    private bool ignoringCollisions = false;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        grab = GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>();

        startLocalPos = transform.localPosition;
        startLocalRot = transform.localRotation;

        // 시작할 때 포켓 안 고정
        LockInPocket();

        grab.selectEntered.AddListener(OnGrabbed);
        grab.selectExited.AddListener(OnReleased);
    }

    private void LockInPocket()
    {
        rb.useGravity = false;
        rb.isKinematic = true; // 포켓 안에서만 Kinematic
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        transform.localPosition = startLocalPos;
        transform.localRotation = startLocalRot;
    }

    private void OnGrabbed(SelectEnterEventArgs args)
    {
        // 잡으면 Dynamic으로 전환 (XRGrabInteractable이 velocity 설정 가능)
        rb.isKinematic = false;
        rb.useGravity = false; // 아직 중력 X

        // 포켓 Collider 무시
        if (pocketColliders != null && pocketColliders.Length > 0)
        {
            foreach (var c in pocketColliders)
            {
                if (c != null)
                    Physics.IgnoreCollision(GetComponent<Collider>(), c, true);
            }
            ignoringCollisions = true;
        }
    }

    private void OnReleased(SelectExitEventArgs args)
    {
        // Dynamic + Gravity On
        rb.isKinematic = false;
        rb.useGravity = true;

        // 충돌 복원
        if (ignoringCollisions && pocketColliders != null && pocketColliders.Length > 0)
        {
            foreach (var c in pocketColliders)
            {
                if (c != null)
                    Physics.IgnoreCollision(GetComponent<Collider>(), c, false);
            }
            ignoringCollisions = false;
        }
    }

    void Update()
    {
        // 포켓 안에서만 위치·회전 강제 고정
        if (rb.isKinematic && !grab.isSelected)
        {
            transform.localPosition = startLocalPos;
            transform.localRotation = startLocalRot;
        }
    }

    void OnDestroy()
    {
        grab.selectEntered.RemoveListener(OnGrabbed);
        grab.selectExited.RemoveListener(OnReleased);
    }
}
