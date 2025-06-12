using System;
using UnityEngine;
using UnityEngine.UI;

public class HPText : MonoBehaviour
{
    Text hpText;

    private void Awake()
    {
        hpText = GetComponent<Text>();
    }

    public void UpdateHP(float currentHP, float maxHP)
    {
        if (!hpText) return;
        hpText.text = $"{currentHP}/{maxHP}";
    }
}
