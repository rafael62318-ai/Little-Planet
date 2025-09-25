using UnityEngine;
using System.Collections;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(XRGrabInteractable))]
public class PocketReleaseGravity : MonoBehaviour
{
    [Header("설정")]
    [Tooltip("착지했을 때 소환될, AI가 포함된 최종 외계인 프리팹입니다.")]
    public GameObject finalAlienPrefab;
    [Tooltip("파괴된 후 다시 생성되기까지 걸리는 시간(초)입니다.")]
    public float respawnCooldown = 5f;

    [Header("자동 복귀 설정")]
    [Tooltip("행성 중심으로부터 이 거리보다 멀어지면 복귀 타이머가 시작됩니다.")]
    public float maxDistanceFromPlanet = 50f;
    [Tooltip("행성 밖에서 이 시간(초)이 지나면 자동으로 복귀합니다.")]
    public float returnOutOfBoundsDelay = 5f;

    private Transform planetTransform;
    private float outOfBoundsTimer = 0f;

    private Rigidbody rb;
    private XRGrabInteractable grab;
    [SerializeField] private Collider[] pocketColliders;
    private Vector3 startLocalPos;
    private Quaternion startLocalRot;
    private bool wasThrown = false;
    private bool ignoringCollisions = false;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        grab = GetComponent<XRGrabInteractable>();
        startLocalPos = transform.localPosition;
        startLocalRot = transform.localRotation;

        GameObject planetObj = GameObject.FindGameObjectWithTag("Planet");
        if (planetObj != null)
        {
            planetTransform = planetObj.transform;
        }
        else
        {
            Debug.LogError("자동 복귀 시스템 오류: 씬에서 'Planet' 태그를 가진 오브젝트를 찾을 수 없습니다!");
        }


        LockInPocket(); // 시작할 때 포켓 안 고정

        grab.selectEntered.AddListener(OnGrabbed);
        grab.selectExited.AddListener(OnReleased);
    }

    void OnCollisionEnter(Collision collision)
    {
        // 던져진 상태에서 'Planet' 태그를 가진 오브젝트와 충돌했을 때
        if (!grab.isSelected && collision.gameObject.CompareTag("Planet"))
        {
            if (finalAlienPrefab != null)
            {
                // 최종 AI 유닛을 현재 위치에 소환합니다.
                GameObject aiObject = Instantiate(finalAlienPrefab, transform.position, transform.rotation);
                FriendlyAlienAI aiScript = aiObject.GetComponent<FriendlyAlienAI>();
                if (aiScript != null)
                {
                    aiScript.spawner = this;
                }
            }
            gameObject.SetActive(false);
        }
    }
    
    // ★★★ AI가 파괴될 때 호출할 '부활' 함수 ★★★
    public void StartRespawn()
    {
        StartCoroutine(RespawnCoroutine());
    }

    private IEnumerator RespawnCoroutine()
    {
        // 쿨타임만큼 기다립니다.
        yield return new WaitForSeconds(respawnCooldown);
        
        // 자기 자신을 다시 활성화하고 원래 자리로 돌아갑니다.
        gameObject.SetActive(true);
        LockInPocket();
        Debug.Log(gameObject.name + "가 원래 자리로 돌아왔습니다.");
    }


    private void LockInPocket() 
    {
        rb.useGravity = false;
        rb.isKinematic = true; 
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        transform.localPosition = startLocalPos;
        transform.localRotation = startLocalRot;
    }
    private void OnGrabbed(SelectEnterEventArgs args) 
    {
        rb.isKinematic = false;
        rb.useGravity = false; 

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
        wasThrown = true;

        rb.isKinematic = false;
        
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
        if (rb.isKinematic && !grab.isSelected)
        {
            transform.localPosition = startLocalPos;
            transform.localRotation = startLocalRot;
        }
        
        if (wasThrown && planetTransform != null)
        {
            float distance = Vector3.Distance(transform.position, planetTransform.position);
            if (distance > maxDistanceFromPlanet)
            {
                outOfBoundsTimer += Time.deltaTime;
                if (outOfBoundsTimer >= returnOutOfBoundsDelay)
                {
                    RepositionToPlanet();
                }
            }
            else
            {
                outOfBoundsTimer = 0f; // 범위 안에 있으면 타이머 리셋
            }
        }
    }
    void RepositionToPlanet()
    {
        Debug.LogWarning(gameObject.name + "가 전장을 이탈하여 행성 표면으로 복귀합니다.");

        outOfBoundsTimer = 0f;
        
        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }

        Vector3 directionFromCenter = (transform.position - planetTransform.position).normalized;
        float planetRadius = maxDistanceFromPlanet - 2f; 
        transform.position = planetTransform.position + directionFromCenter * planetRadius;
    }


    void OnDestroy() 
    {
        if (grab != null)
        {
            grab.selectEntered.RemoveListener(OnGrabbed);
            grab.selectExited.RemoveListener(OnReleased);
        }
    }
}

