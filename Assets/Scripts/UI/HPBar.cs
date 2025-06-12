using UnityEngine;
using UnityEngine.UI;

public class HPBar : MonoBehaviour
{
    Image hpImg;
    void Awake()
    {
        hpImg = GetComponent<Image>();
    }
    
    public void UpdateHP(float currentHP, float maxHP)
    {
        if (!hpImg) return;
        float healthPercent = Mathf.Clamp01(currentHP / maxHP);
        hpImg.fillAmount = healthPercent;
    }
}