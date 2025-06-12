using UnityEngine;
using System.Collections.Generic;
public class SpawnEnemyHandler : INetworkMessageHandler
{
    private readonly Dictionary<string, GameObject> Enemies = new Dictionary<string, GameObject>();
    private readonly WaveManager waveManager;

    public string Type => "spawn_enemy";

    public SpawnEnemyHandler(
        Dictionary<string, GameObject> Enemies,
        WaveManager waveManager)
    {
        this.Enemies = Enemies;
        this.waveManager = waveManager;
    }

    public void Handle(NetMsg msg)
    {
        var pid = msg.enemyId;
        if (!Enemies.ContainsKey(pid))
        {
            var enemy = waveManager.SpawnEnemy(msg.wave,msg.enemyId);
            Enemies[pid] = enemy;
        }
    }
}