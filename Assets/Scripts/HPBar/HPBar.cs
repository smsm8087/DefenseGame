using UnityEngine;
using UnityEngine.UI;

public class HPBar : MonoBehaviour
{
    Image hpImg;
    void Start()
    {
        hpImg = GetComponent<Image>();
    }
    
    public void UpdateHP(float currentHP, float maxHP)
    {
        float healthPercent = Mathf.Clamp01(currentHP / maxHP);
        hpImg.fillAmount = healthPercent;
    }
}