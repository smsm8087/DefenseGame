
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
    public GameObject SpawnEnemy(string guid, float spawnPosX, float spawnPosY)
    {
        if (gameStateProvider.IsGameOver()) return null;
        
        Vector3 spawnPos = new Vector3(spawnPosX, spawnPosY, 0);
        
        var enemy = Instantiate(enemyPrefab, spawnPos , Quaternion.identity);
        var movement = enemy.GetComponent<EnemyMovement>();
        movement?.setEnemy(guid);
        return enemy;
    }
}
