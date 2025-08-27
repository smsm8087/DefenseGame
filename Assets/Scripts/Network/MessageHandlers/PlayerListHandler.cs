using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class PlayerListHandler : INetworkMessageHandler
{
    private readonly Dictionary<string, GameObject> prefabMap;
    private readonly Dictionary<string, GameObject> players;

    public string Type => "player_list";
    private List<PlayerInfo> playerList;

    // ★ 인게임에서 첫 동기화 여부 (이 씬 동안 1회만 전체 재빌드)
    private static bool _syncedOnceInThisScene = false;

    public PlayerListHandler(Dictionary<string, GameObject> prefabMap, Dictionary<string, GameObject> players)
    {
        this.prefabMap = prefabMap;
        this.players = players;
    }

    public void Handle(NetMsg msg)
    {
        if (msg.players == null) return;
        playerList = msg.players;
        Debug.Log($"[PlayerListHandler] 플레이어 목록 처리 시작. 총 {msg.players.Count}명");

        // ★ 인게임 첫 수신 시: 서버 권위로 전원 재스폰 (중복/불일치 방지)
        if (SceneManager.GetActiveScene().name == "IngameScene" && !_syncedOnceInThisScene)
        {
            // 기존 오브젝트 정리
            foreach (var kv in players)
            {
                if (kv.Value != null) GameObject.Destroy(kv.Value);
            }
            players.Clear();
            _syncedOnceInThisScene = true;
            Debug.Log("[PlayerListHandler] 첫 동기화: 기존 오브젝트 정리 및 서버 목록으로 재스폰");
        }

        foreach (var playerData in playerList)
        {
            string pid = playerData.id; // .ToString() 제거
            string jobType = playerData.job_type;

            Debug.Log($"[PlayerListHandler] 처리 중: PID={pid}, JobType={jobType}, MyGUID={NetworkManager.Instance.MyUserId}");

            // 잘못된 데이터 필터링
            if (string.IsNullOrEmpty(pid) || string.IsNullOrEmpty(jobType))
            {
                Debug.LogWarning($"[PlayerListHandler] 잘못된 플레이어 데이터 스킵: PID={pid}, JobType={jobType}");
                continue;
            }

            // ★ jobType 키 정규화(프리팹 맵 키는 소문자 가정)
            string key = jobType.ToLowerInvariant();

            // 이미 존재하는 플레이어는 스킵 (첫 동기화 이후 재수신 대비)
            if (players.ContainsKey(pid))
            {
                Debug.Log($"[PlayerListHandler] 이미 존재하는 플레이어 스킵: {pid}");
                continue;
            }

            // ★ 프리팹 매핑 방어 + 폴백 처리
            if (!prefabMap.TryGetValue(key, out var prefab) || prefab == null)
            {
                Debug.LogWarning($"[PlayerListHandler] 프리팹 없음/누락: jobType={jobType} → tank로 폴백");
                if (!prefabMap.TryGetValue("tank", out prefab) || prefab == null)
                {
                    Debug.LogError("[PlayerListHandler] tank 폴백 프리팹도 없음. 스폰 불가");
                    continue;
                }
            }

            var playerObj = GameObject.Instantiate(prefab);
            playerObj.name = $"Player_{pid}_{key}";
            players[pid] = playerObj;

            NetworkCharacterFollower playerFollower = playerObj.GetComponent<NetworkCharacterFollower>();
            if (playerFollower)
            {
                playerFollower.enabled = true;
            }

            Debug.Log($"[PlayerListHandler] 플레이어 생성 완료: {pid} ({key})");
        }
    }
}
