
using UnityEditor;
using UnityEngine;

public class GameManager : MonoBehaviour,  IGameStateProvider
{
    public static GameManager Instance { get; private set; }

    [SerializeField] private WaveManager waveManager;
    [SerializeField] private SharedHpManager sharedHpManager;

    public bool IsGameOver { get; private set; }
    bool IGameStateProvider.IsGameOver() => IsGameOver;
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        StartGame();
    }

    private void StartGame()
    {
        if (waveManager != null)
        {
            waveManager.Initialize(this);
        }
        sharedHpManager.OnDeath += TriggerGameOver;
    }

    public void TriggerGameOver()
    {
        if (IsGameOver) return;
        IsGameOver = true;
        Debug.Log("Game Over! Gem has been destroyed.");
        // TODO: UI 호출 or Event로 추상화
    }
}
