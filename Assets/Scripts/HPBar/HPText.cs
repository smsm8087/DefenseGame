using UnityEngine;
using UnityEngine.UI;

public class HPText : MonoBehaviour
{
    Text hpText;
    void Start()
    {
        hpText = GetComponent<Text>();
    }
    
    public void UpdateHP(float currentHP, float maxHP)
    {
        float healthPercent = Mathf.Clamp01(currentHP / maxHP);
        hpText.text = $"{currentHP}/{maxHP}";
    }
}
