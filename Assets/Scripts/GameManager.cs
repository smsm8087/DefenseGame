
using UnityEditor;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public WaveManager waveManager;
    public GemHealthSystem gem;
    public bool isGameOver = false;
    public string myGUID {get; set;}
    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        waveManager.StartWaveLoop();
    }

    public void GameOver()
    {
        isGameOver = true;
        Debug.Log("Game Over! Gem has been destroyed.");
        // TODO: Show UI, Stop Spawning
    }
}
