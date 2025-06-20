
using System.Collections;
using UnityEngine;

public class WaveManager : MonoBehaviour
{
    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private Transform[] spawnPoints;


    public GameObject SpawnEnemy(string guid, float spawnPosX, float spawnPosY)
    {
        Vector3 spawnPos = new Vector3(spawnPosX, spawnPosY, 0);
        
        var enemy = Instantiate(enemyPrefab, spawnPos , Quaternion.identity);
        var movement = enemy.GetComponent<EnemyController>();
        movement?.SetGuid(guid);
        return enemy;
    }
}
