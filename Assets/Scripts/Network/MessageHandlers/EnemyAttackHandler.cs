using System.Collections.Generic;
using UnityEngine;

namespace NativeWebSocket.MessageHandlers
{
    public class EnemyAttackHandler : INetworkMessageHandler
    {
        private readonly Dictionary<string, GameObject> Enemies = new ();

        public string Type => "enemy_attack";

        public EnemyAttackHandler(Dictionary<string, GameObject> Enemies)
        {
            this.Enemies = Enemies;
        }

        public void Handle(NetMsg msg)
        {
            var pid = msg.enemyId;
            if (!Enemies.ContainsKey(pid)) return;
            EnemyController enemyController = Enemies[pid].GetComponent<EnemyController>();
            enemyController.ChangeStateByEnum(EnemyState.Attack);
        }
    }
}