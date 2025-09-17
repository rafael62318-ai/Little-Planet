using UnityEngine;
using UnityEngine.UI;

public class WorldSpaceHealthBar : MonoBehaviour
{
    // 인스펙터에서 연결할 UI 슬라이더
    public Slider healthSlider;

    // EnemyHealth 스크립트가 호출할 함수
    public void UpdateHealth(float currentHealth, float maxHealth)
    {
        if (healthSlider != null)
        {
            // 현재 체력 / 최대 체력으로 비율을 계산하여 슬라이더 값에 적용
            healthSlider.value = currentHealth / maxHealth;
        }
    }
}