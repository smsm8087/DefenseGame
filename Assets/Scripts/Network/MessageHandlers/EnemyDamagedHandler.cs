using UnityEngine;
using System.Collections.Generic;
using UI;
using UnityEditor;

public class EnemyDamagedHandler : INetworkMessageHandler
{
    private readonly Dictionary<string, GameObject> Enemies = new ();
    private readonly GameObject DamageTextPrefab;

    public string Type => "enemy_damaged";

    public EnemyDamagedHandler(Dictionary<string, GameObject> Enemies,  GameObject DamageTextPrefab)
    {
        this.Enemies = Enemies;
        this.DamageTextPrefab = DamageTextPrefab;
    }

    public void Handle(NetMsg msg)
    {
        List<EnemyDamageInfo> damagedEnemies = msg.damagedEnemies;
        foreach (var damagedEnemy in damagedEnemies)
        {
            var pid = damagedEnemy.enemyId;
            if (Enemies.TryGetValue(pid, out var enemyObj))
            {
                var hpBar = enemyObj.GetComponentInChildren<HPBar>();
                if (hpBar != null)
                {
                    hpBar.UpdateHP(damagedEnemy.currentHp, damagedEnemy.maxHp);
                }
                
                // HPBarCanvas 의 transform (부모로 넣기)
                Transform damageTextRoot = enemyObj.transform.Find("UICanvas/DamagePos");
                Vector3 spawnPos = damageTextRoot.position;

                var dmgTextObj = GameObject.Instantiate(DamageTextPrefab, spawnPos, Quaternion.identity, damageTextRoot);

                var dmgText = dmgTextObj.GetComponent<DamageText>();
                if (dmgText != null)
                {
                    dmgText.Init(damagedEnemy.damage);
                }
            }
        }
    }
}