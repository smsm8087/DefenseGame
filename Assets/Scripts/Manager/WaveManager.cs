
using System.Collections;
using UnityEngine;

public interface IGameStateProvider
{
    bool IsGameOver();
}
public class WaveManager : MonoBehaviour
{
    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private Transform[] spawnPoints;

    private IGameStateProvider gameStateProvider;

    public void Initialize(IGameStateProvider provider)
    {
        gameStateProvider = provider;
    }
    public GameObject SpawnEnemy(float spawnPosX, float spawnPosY, float targetPosX, float targetPosY)
    {
        if (gameStateProvider.IsGameOver()) return null;
        
        Vector3 spawnPos = new Vector3(spawnPosX, spawnPosY, 0);
        Vector3 targetPos = new Vector3(targetPosX, targetPosY, 0);
        
        var enemy = Instantiate(enemyPrefab, spawnPos , Quaternion.identity);
        var movement = enemy.GetComponent<EnemyMovement>();
        movement?.SetTarget(targetPos);
        return enemy;
    }
}
