using System;
using System.Collections.Generic;
using System.Text;
using NativeWebSocket;
using NativeWebSocket.MessageHandlers;
using UnityEngine;
using Newtonsoft.Json;
using UI;

public class NetworkManager : MonoBehaviour
{
    public static NetworkManager Instance;
    [Header("직업별 프리팹")]
    public GameObject tankPrefab;
    public GameObject sniperPrefab;
    public GameObject programmerPrefab;
    
    [Header("총알")]
    [SerializeField] private GameObject bulletPrefab;
    
    [SerializeField] private WaveManager waveManager;
    [SerializeField] private CenterText centerText;
    [SerializeField] private GameObject DamageTextPrefab;
    [SerializeField] private ProfileUI profileUI;
    
    [Header("보스")]
    [SerializeField] private GameObject bossPrefab;
    
    private Dictionary<string, GameObject> players = new();
    private Dictionary<string, GameObject> enemies = new();
    private Dictionary<string, INetworkMessageHandler> handlers = new();
    private Dictionary<string, GameObject> prefabMap = new();
    private Dictionary<string, GameObject> bullets = new();
    private Dictionary<string, GameObject> bossDict = new();
    private event Action onGameOver;
    public string MyGUID { get; private set; }
    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        
        prefabMap = new Dictionary<string, GameObject>()
        {
            { "tank", tankPrefab },
            { "sniper", sniperPrefab },
            { "programmer", programmerPrefab }
        };
        
        RegisterHandlers();
    }

    private void Start()
    {
        WebSocketClient.Instance.OnMessageReceived += HandleMessage;
    }

    public Dictionary<string, GameObject> GetEnemies()
    {
        return  enemies;
    }
    public Dictionary<string, GameObject> GetPlayers()
    {
        return  players;
    }
    

    public void SetOnGamveOverAction(Action onGameOver)
    {
        this.onGameOver += onGameOver;
    }

    public void TriggerGameOver()
    {
        this.onGameOver?.Invoke();
    }
    public void SetMyGUID(string guid)
    {
        MyGUID = guid;
    }

    public void RemovePlayer(string guid)
    {
        if (!players.ContainsKey(guid)) return;
        Destroy(players[guid]);
        players.Remove(guid);
    }

    public void RemoveEnemy(string guid)
    {
        if (!enemies.ContainsKey(guid)) return;
        Destroy(enemies[guid]);
        enemies.Remove(guid);
    }
    public void RemoveBullet(string guid)
    {
        if (!bullets.ContainsKey(guid)) return;
        Destroy(bullets[guid]);
        bullets.Remove(guid);
    }

    public void RemoveAllEnemies()
    {
        foreach (var enemyData in enemies)
        {
            GameObject enemyObj = enemyData.Value;
            Destroy(enemyObj);
        }
        enemies.Clear();
    }

    public void ResetHp()
    {
        //임시 초기화
        GameManager.Instance.UpdateHPBar(100,100);
    }
    private Dictionary<string, INetworkMessageHandler> _handlers;

    private void RegisterHandlers()
    {
        //서버 리시브 처리 부분.
        AddHandler(new PlayerJoinHandler(prefabMap,players, this,profileUI));
        AddHandler(new PlayerMoveHandler(players));
        AddHandler(new PlayerListHandler(prefabMap, players));
        AddHandler(new PlayerLeaveHandler());
        AddHandler(new SpawnEnemyHandler(enemies,waveManager));
        AddHandler(new EnemySyncHandler(enemies));
        AddHandler(new EnemyDieHandler(enemies));
        AddHandler(new SharedHpUpdateHandler());
        AddHandler(new CountDownHandler(centerText));
        AddHandler(new WaveStartHandler(centerText));
        AddHandler(new GameResultHandler(centerText));
        AddHandler(new RestartHandler());
        AddHandler(new PlayerAnimationHandler(players));
        AddHandler(new EnemyDamagedHandler(enemies,DamageTextPrefab));
        AddHandler(new EnemyChangeStateHandler(enemies));
        AddHandler(new SettlementStartHandler(UIManager.Instance.ShowCardSelectPopup));
        AddHandler(new SettlementTimerUpdateHandler());
        AddHandler(new UpdateUltGaugeHandler(profileUI));
        AddHandler(new UpdatePlayerDataHandler(players, profileUI));
        AddHandler(new PartyMemberHealthHandler());
        AddHandler(new PartyMemberUltHandler());
        AddHandler(new PartyMemberStatusHandler());
        AddHandler(new PartyInfoHandler());
        AddHandler(new PartyMemberLeftHandler());
        AddHandler(new InitialGameHandler());
        AddHandler(new PlayerUpdateHpHandler(players, profileUI));
        AddHandler(new BulletSpawnHandler(bullets, bulletPrefab));
        AddHandler(new BulletTickHandler(bullets));
        AddHandler(new BulletDestroyHandler(bullets));
        AddHandler(new PlayerDeathHandler(players));
        AddHandler(new BossStartHandler(bossPrefab, bossDict));
        AddHandler(new BossDamagedHandler(bossDict,DamageTextPrefab));
        AddHandler(new BossSyncHandler(bossDict));
        AddHandler(new BossDustWarningHandler(bossDict));
        AddHandler(new BossDeadHandler(bossDict));
        AddHandler(new RevivalStartedHandler());
        AddHandler(new RevivalProgressHandler());
        AddHandler(new RevivalCompletedHandler(players, profileUI));
        AddHandler(new RevivalCancelledHandler(players));
        AddHandler(new InvulnerabilityEndedHandler(players));
        Debug.Log("[NetworkManager] 부활 핸들러들 등록 완료");
    }

    private void AddHandler(INetworkMessageHandler handler)
    {
        handlers[handler.Type] = handler;
    }

    public void HandleMessage(string msg)
    {
        NetMsg netMsg = JsonConvert.DeserializeObject<NetMsg>(msg);
        if (handlers.TryGetValue(netMsg.type, out var handler))
        {
            handler.Handle(netMsg);
        }
        else
        {
            Debug.LogWarning($"Unhandled message type: {netMsg.type}");
        }
    }
    
    public void SendMsg(object msg)
    {
        string json = JsonConvert.SerializeObject(msg);
        WebSocketClient.Instance.Send(json);
    }
}
