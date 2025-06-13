using UnityEngine;
using System.Collections.Generic;
public class EnemySyncHandler : INetworkMessageHandler
{
    private readonly Dictionary<string, GameObject> Enemies = new ();

    public string Type => "enemy_sync";

    public EnemySyncHandler(Dictionary<string, GameObject> Enemies)
    {
        this.Enemies = Enemies;
    }

    public void Handle(NetMsg msg)
    {
        List<EnemySyncPacket> enemies = msg.enemies;
        foreach (var enemySyncPacket in enemies)
        {
            var pid = enemySyncPacket.enemyId;
            if (!Enemies.ContainsKey(pid)) continue;
            var enemyObj =  Enemies[pid];
            if (!enemyObj) continue;
            EnemyMovement em = enemyObj.GetComponent<EnemyMovement>();
            if (!em) continue;
            enemyObj.GetComponent<EnemyMovement>().SyncFromServer(enemySyncPacket.x);
        }
    }
}