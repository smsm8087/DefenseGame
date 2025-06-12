
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
    [SerializeField] private float waveInterval = 10f;
    [SerializeField] private int enemiesPerWave = 5;
    [SerializeField] private Transform crystal;

    private IGameStateProvider gameStateProvider;

    public void Initialize(IGameStateProvider provider)
    {
        gameStateProvider = provider;
    }

    public void StartWaveLoop()
    {
        StartCoroutine(SpawnWaves());
    }

    private IEnumerator SpawnWaves()
    {
        while (!gameStateProvider.IsGameOver())
        {
            for (int i = 0; i < enemiesPerWave; i++)
            {
                SpawnEnemy();
                yield return new WaitForSeconds(1f);
            }
            yield return new WaitForSeconds(waveInterval);
        }
    }

    private void SpawnEnemy()
    {
        if (gameStateProvider.IsGameOver()) return;
        int index = UnityEngine.Random.Range(0, spawnPoints.Length);
        var enemy = Instantiate(enemyPrefab, spawnPoints[index].position, Quaternion.identity);
        var movement = enemy.GetComponent<EnemyMovement>();
        movement?.SetTarget(crystal.position);
    }
}
