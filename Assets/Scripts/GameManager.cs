
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public GemHealthSystem gem;
    public bool isGameOver = false;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
    }

    public void GameOver()
    {
        isGameOver = true;
        Debug.Log("Game Over! Gem has been destroyed.");
        // TODO: Show UI, Stop Spawning
    }
}
