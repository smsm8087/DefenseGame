using System.Collections.Generic;

[System.Serializable]
public class NetMsg
{
    public string type;
    public string playerId;
    public string enemyId;
    public List<string> players;
    //player
    public float x;
    public float y;
    public bool isJumping;
    public bool isRunning;
    public int wave;
    //enemy
    public float spawnPosX;
    public float spawnPosY;
    public List<EnemySyncPacket> enemies;
    public List<string> deadEnemyIds;
    
    //ui
    public float currentHp;
    public float maxHp;
    public int countDown;
    public string message;
}
public class EnemySyncPacket
{
    public string enemyId { get; set; }
    public float x { get; set; }
}
