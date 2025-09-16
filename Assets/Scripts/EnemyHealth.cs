using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    public float maxHealth = 100f;
    public int goldReward = 10;
    public float currentHealth;

    private Enemy enemyScript;

    void Start()
    {
        currentHealth = maxHealth;
        enemyScript = GetComponent<Enemy>();
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;

        if (currentHealth > 0)
        {
            if (enemyScript != null)
                enemyScript.GetHit(); // 피격 애니메이션 & 이동 정지
        }
        else
        {
            Die();
        }
    }

    void Die()
    {
        if (enemyScript != null)
            enemyScript.Die();

        Debug.Log(gameObject.name + " 처치! 골드 +" + goldReward);

        if (ResourceManager.Instance != null)
        {
            ResourceManager.Instance.AddGold(goldReward);
        }
    }
}
