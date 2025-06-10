
using System.Collections;
using UnityEngine;

public class WaveManager : MonoBehaviour
{
    public GameObject enemyPrefab;
    public Transform[] spawnPoints;
    public float waveInterval = 10f;
    public int enemiesPerWave = 5;
    public Transform Crystal;
    public void StartWaveLoop()
    {
        StartCoroutine(SpawnWaves());
    }

    IEnumerator SpawnWaves()
    {
        while (!GameManager.Instance.isGameOver)
        {
            for (int i = 0; i < enemiesPerWave; i++)
            {
                SpawnEnemy();
                yield return new WaitForSeconds(1f);
            }
            yield return new WaitForSeconds(waveInterval);
        }
    }

    void SpawnEnemy()
    {
        if (GameManager.Instance.isGameOver) return;
        int spawnIndex = Random.Range(0, spawnPoints.Length);
        GameObject enemy = Instantiate(enemyPrefab, spawnPoints[spawnIndex].position, Quaternion.identity);
        EnemyMovement enemyMovement = enemy.GetComponent<EnemyMovement>();
        if (enemyMovement)
        {
            enemyMovement.SetTarget(Crystal.position);
        }
    }
}
