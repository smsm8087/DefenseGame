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
        var pid = msg.enemyId;
        if (!Enemies.ContainsKey(pid)) return;
        var enemyObj =  Enemies[pid];
        enemyObj.GetComponent<EnemyMovement>().SyncFromServer(msg.x, msg.y);
    }
}