using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    [Header("능력치 설정")]
    public float maxHealth = 100f;
    public int goldReward = 10;

    [Header("UI 설정")]
    // 인스펙터에서 적 프리팹의 자식으로 있는 3D 체력 바 캔버스를 연결
    public WorldSpaceHealthBar myHealthBar;

    private float currentHealth;

    void Start()
    {
        currentHealth = maxHealth;
        // 시작할 때 체력 바를 100%로 채워줌
        if (myHealthBar != null)
        {
            myHealthBar.UpdateHealth(currentHealth, maxHealth);
        }
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        // 체력 바에 변경된 체력을 업데이트하도록 신호를 보냄
        if (myHealthBar != null)
        {
            myHealthBar.UpdateHealth(currentHealth, maxHealth);
        }

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        if (ResourceManager.Instance != null)
        {
            ResourceManager.Instance.AddGold(goldReward);
        }
        Destroy(gameObject);
    }
}