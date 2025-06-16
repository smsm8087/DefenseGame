using System;
using UnityEngine;

public class SharedHpManager : MonoBehaviour
{
    public HPBar hpBar;
    public HPText hpText;

    public void UpdateHPBar(float currentHp, float maxHp)
    {
        hpBar?.UpdateHP(currentHp, maxHp);
        hpText?.UpdateHP(currentHp, maxHp);
    }
}