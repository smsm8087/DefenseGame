using System.Collections.Generic;
using DataModels;

[System.Serializable]
public class NetMsg
{
    public string type;
    public int wave;
    public int wave_id;
    
    public string playerId;
    public string enemyId;
    public int enemyDataId;
    public List<PlayerInfo> players; // List<string> -> List<PlayerInfo>로 변경
    public string jobType;

    public string animation;
    //player
    public float x;
    public float y;
    public float currentUlt;
    public float maxUlt;
    public PlayerInfo playerInfo { get; set; }
    
    //partymember
    public string player_id;           // 파티 멤버 ID
    public float current_health;       // 현재 체력
    public float max_health;          // 최대 체력
    public float current_ult;         // 현재 궁극기 (이미 있음)
    public float max_ult;             // 최대 궁극기 (이미 있음)
    public string status;             // 플레이어 상태 (dead, normal 등)
    public List<PartyMemberInfo> members;  // 파티 전체 정보용
    
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
    public string animName;
    
    //ui
    public float currentHp;
    public float maxHp;
    public int countDown;
    public string message;
    
    //settlementPhase
    public float duration;
    public List<CardData> cards;
    public int selectedCardId;
    public int readyCount;
    
    //bullet
    public string bulletId;
    public List<BulletInfo> bullets;

    //cheat
    public bool isCheat;
}

[System.Serializable]
public class PlayerInfo
{
    public string id { get; set; }
    public string job_type { get; set; }
    public int currentHp { get; set; }
    public float currentUlt { get; set; }
    public int currentMaxHp { get; set; }
    public float currentUltGauge { get; set; }
    public float currentMoveSpeed { get; set; }
    public float currentAttackSpeed { get; set; }
    public int currentCriPct { get; set; }
    public int currentCriDmg { get; set; }
    public float currentAttack { get; set; }
    public List<int> cardIds { get; set; } = new List<int>();
    public PlayerData playerBaseData { get; set; }
}

public class PartyMemberInfo
{
    public string id;
    public string job_type;
    public float current_health;
    public float max_health;
    public float current_ult;
    public float max_ult;
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
    public bool isCritical { get; set; }
}
public class BulletInfo
{
    public string bulletId;
    public float x;
    public float y;
}