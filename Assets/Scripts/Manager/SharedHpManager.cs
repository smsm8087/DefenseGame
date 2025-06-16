using System;
using UnityEngine;

public class SharedHpManager : MonoBehaviour
{
    public HPBar hpBar;
    public HPText hpText;

    public Action OnDeath;
    public void UpdateHPBar(float currentHp, float maxHp)
    {
        hpBar?.UpdateHP(currentHp, maxHp);
        hpText?.UpdateHP(currentHp, maxHp);
        if (currentHp <= 0f)
        {
            //추후에 서버에서 보내줄 예정.
            OnDeath?.Invoke();
        }
    }
}