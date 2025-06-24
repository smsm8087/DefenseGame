using System.Collections.Generic;
using DataModels;

[System.Serializable]
public class NetMsg
{
    public string type;
    public int wave;
    
    public string playerId;
    public string enemyId;
    public List<PlayerInfo> players; // List<string> -> List<PlayerInfo>로 변경
    public string jobType;

    public string animation;
    //player
    public float x;
    public float y;
    //히트박스 영역
    public float attackBoxCenterX;
    public float attackBoxCenterY;
    public float attackBoxWidth;
    public float attackBoxHeight;
        
    //enemy
    public float spawnPosX;
    public float spawnPosY;
    public List<EnemySyncPacket> enemies;
    public List<string> deadEnemyIds;
    public List<EnemyDamageInfo> damagedEnemies;
    
    //ui
    public float currentHp;
    public float maxHp;
    public int countDown;
    public string message;
    
    //settlementPhase
    public int duration;
    public List<CardData> cards;
    
    //cheat
    public bool isCheat;
}

[System.Serializable]
public class PlayerInfo
{
    public string playerId;
    public string jobType;
}

public class EnemySyncPacket
{
    public string enemyId { get; set; }
    public float x { get; set; }
}

public class EnemyDamageInfo
{
    public string enemyId { get; set; }
    public int currentHp { get; set; }
    public int maxHp { get; set; }
    public int damage { get; set; }
}