using UnityEngine;
using UnityEngine.UI;

public class ThrowCounterUI : MonoBehaviour
{
    public AlienThrower alienThrower;
    public Text countText;

   
    void Start()
    {
        alienThrower.OnThrowCountChanged += UpdateText;
        countText.text = $"Throw Count : {alienThrower.maxThrows}";
    }

    
    void UpdateText(int remainingCount)
    {
        countText.text =  $"Throw Count : {remainingCount}";
    }

    void OnDestroy()
    {
        if (alienThrower != null)
        {
            alienThrower.OnThrowCountChanged -= UpdateText;
        }
    }
}
