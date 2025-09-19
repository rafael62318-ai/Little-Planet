using UnityEngine;
using UnityEngine.SceneManagement;

public class HomeBase : MonoBehaviour
{
    public static HomeBase Instance { get; private set; }

    [Header("능력치 설정")]
    public float maxHealth = 1000f;
    public float maxEnergy = 100f;

    [Header("UI 설정")]
    public WorldSpaceHealthBar myHealthBar;

    [Header("이펙트 설정")]
    public GameObject buffEffectPrefab;
    public GameObject sparkEffectPrefab;

    private GameObject currentBuffEffect;
    private GameObject currentSparkEffect;

    private bool isDamaged = false;

    public float currentHealth { get; private set; }
    public float currentEnergy { get; private set; }

    // 에너지를 5분 동안 100까지 충전하는 속도
    private float energyChargeRate;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    void Start()
    {
        currentHealth = maxHealth;
        currentEnergy = 0;
        // 5분(300초) 동안 에너지를 100까지 채우기 위한 충전 속도 계산
        energyChargeRate = maxEnergy / 300f;

        if (myHealthBar != null)
        {
            myHealthBar.UpdateHealth(currentHealth, maxHealth);
        }

        Debug.Log("본진 생성 완료! 현재 체력: " + currentHealth + ", 에너지 충전 속도: " + energyChargeRate + " per second.");
    }

    void Update()
    {
        // 에너지가 아직 꽉 차지 않았을 때만 자동으로 충전합니다.
        if (currentEnergy < maxEnergy)
        {
            currentEnergy += energyChargeRate * Time.deltaTime;
            currentEnergy = Mathf.Min(currentEnergy, maxEnergy);
        }
        // 에너지가 최대치를 넘지 않도록 보정합니다.
        
         // 스파크 이펙트 로직
        // 공격받고 있지 않고 스파크 이펙트가 재생 중일 때
        if (!isDamaged && currentSparkEffect != null)
        {
            Destroy(currentSparkEffect);
            currentSparkEffect = null;
        }
        // 버프 이펙트 로직
        // 에너지가 충전 중이고 (currentEnergy < maxEnergy) 공격받지 않을 때 (!isDamaged)
        if (currentEnergy < maxEnergy && !isDamaged)
        {
            if (currentBuffEffect == null)
            {
                // 버프 이펙트가 없으면 생성
                currentBuffEffect = Instantiate(buffEffectPrefab, transform.position, Quaternion.identity, transform);
            }
        }
        else
        {
            if (currentBuffEffect != null)
            {
                // 조건에 맞지 않으면 버프 이펙트 파괴
                Destroy(currentBuffEffect);
                currentBuffEffect = null;
            }
        }
    }

    // 적의 공격으로 체력을 깎는 메서드
    public void TakeDamage(float damageAmount)
    {
        currentHealth -= damageAmount;
        currentHealth = Mathf.Max(currentHealth, 0);
        Debug.Log("본진 피격! 남은 체력: " + currentHealth);

        if (myHealthBar != null)
        {
            myHealthBar.UpdateHealth(currentHealth, maxHealth);
        }
        // 스파크 이펙트 로직
        if (currentSparkEffect == null)
        {
            // 스파크 이펙트가 없으면 생성
            currentSparkEffect = Instantiate(sparkEffectPrefab, transform.position, Quaternion.identity, transform);
        }
        // 피격 상태를 true로 설정
        isDamaged = true;
        // 피격 상태를 일정 시간 후에 false로 되돌리는 코루틴 호출 (선택 사항)
        // StopCoroutine("ResetDamageState");
        // StartCoroutine("ResetDamageState");
    }
}