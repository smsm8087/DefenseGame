
using System;
using UnityEditor;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [SerializeField] private WaveManager waveManager;
    [SerializeField] private SharedHpManager sharedHpManager;

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
        NetworkManager.Instance.SetOnGamveOverAction( ()=> TriggerGameOver());
    }

    public void TriggerGameOver()
    {
        NetworkManager.Instance.RemoveAllEnemies();
        NetworkManager.Instance.ResetHp();
    }
}
