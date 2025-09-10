using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    public float maxHealth = 100f;
    public int goldReward = 10;

    private float currentHealth;

    void Start()
    {
        currentHealth = maxHealth;
    }

    //외부(아군 AI)에서 호출할 피해 받는 함수
    public void TakeDamage(int damage)
    {
        currentHealth -= damage;

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    //사망 처리 함수
    void Die()
    {
        Debug.Log(gameObject.name + "처치! 골드 +" + goldReward);
        
        if(ResourceManager.Instance != null)
        {
            ResourceManager.Instance.AddGold(goldReward);
        }

        Destroy(gameObject);
    }
}
