using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HPText : MonoBehaviour
{
    TextMeshPro hpText;

    private void Awake()
    {
        hpText = GetComponent<TextMeshPro>();
    }

    public void UpdateHP(float currentHP, float maxHP)
    {
        if (!hpText) return;
        hpText.text = $"{currentHP}/{maxHP}";
    }
}
