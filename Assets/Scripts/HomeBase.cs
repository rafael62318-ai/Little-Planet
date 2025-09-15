using UnityEngine;
using UnityEngine.SceneManagement;

public class HomeBase : MonoBehaviour
{
    public static HomeBase Instance { get; private set; }
    
    public float maxHealth = 1000f;
    private float currentHealth;

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
        Debug.Log("본진 생성 완료! 현재 체력: " + currentHealth);
    }

    public void TakeDamage(float damageAmount)
    {
        currentHealth -= damageAmount;
        Debug.Log("본진 피격! 남은 체력: " + currentHealth);

        if (currentHealth <= 0)
        {
            GameOver();
        }
    }

    void GameOver()
    {
        Debug.Log("본진이 파괴되었습니다... GAME OVER");
        Time.timeScale = 0;
    }

}
