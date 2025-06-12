
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
    [SerializeField] private Transform crystal;
    private int wave;
    private string enemyId;

    private IGameStateProvider gameStateProvider;

    public void Initialize(IGameStateProvider provider)
    {
        gameStateProvider = provider;
    }
    public GameObject SpawnEnemy(int wave, string enemyId)
    {
        if (gameStateProvider.IsGameOver()) return null;
        this.wave = wave;
        this.enemyId = enemyId;
        
        int index = UnityEngine.Random.Range(0, spawnPoints.Length);
        var enemy = Instantiate(enemyPrefab, spawnPoints[index].position, Quaternion.identity);
        var movement = enemy.GetComponent<EnemyMovement>();
        movement?.SetTarget(crystal.position);
        return enemy;
    }
}
