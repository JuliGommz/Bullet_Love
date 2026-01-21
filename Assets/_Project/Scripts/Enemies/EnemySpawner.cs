/*
====================================================================
* EnemySpawner.cs - Wave-Based Enemy Spawning System
====================================================================
* Project: Showroom_Tango
* Course: PRG - Game & Multimedia Design SRH Hochschule
* Developer: Julian Gomez
* Date: 2025-01-14
* Version: 1.0
* 
* ⚠️ WICHTIG: KOMMENTIERUNG NICHT LÖSCHEN! ⚠️
* 
* [HUMAN-AUTHORED]
* - Wave structure (3 waves, 15→30→50 enemies)
* - Spawn timing (30% burst, 70% trickle)
* - Enemy ratio (70% Chaser, 30% Shooter)
* 
* [AI-ASSISTED]
* - Server-authority spawning
* - Circular spawn point calculation
* - Wave progression system
* - NetworkObject spawn pattern
====================================================================
*/

using FishNet.Object;
using FishNet.Object.Synchronizing;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEditorInternal.ReorderableList;

public class EnemySpawner : NetworkBehaviour
{
    [Header("Enemy Prefabs")]
    [SerializeField] private GameObject chaserPrefab;
    [SerializeField] private GameObject shooterPrefab;

    [Header("Spawn Settings")]
    [SerializeField] private float spawnRadius = 18f;
    [SerializeField] private float minSpawnDistance = 5f;
    [SerializeField] private int spawnPointCount = 8;

    [Header("Wave Configuration")]
    private readonly SyncVar<int> currentWave = new SyncVar<int>(1);
    [SerializeField] private int maxWaves = 3;

    private int[] waveEnemyCounts = { 30, 50, 80 };
    private float chaserSpawnWeight = 0.5f;

    private bool waveActive = false;

    public override void OnStartServer()
    {
        base.OnStartServer();
        Debug.Log("[EnemySpawner] Server started - ready to spawn");

        StartCoroutine(WaveSequence());
    }

    private IEnumerator WaveSequence()
    {
        yield return new WaitForSeconds(2f);

        for (int wave = 0; wave < maxWaves; wave++)
        {
            currentWave.Value = wave + 1;
            Debug.Log($"[EnemySpawner] Starting Wave {currentWave.Value}/{maxWaves}");

            yield return StartCoroutine(SpawnWave(currentWave.Value));

            Debug.Log($"[EnemySpawner] Wave {currentWave.Value} complete");

            if (currentWave.Value < maxWaves)
            {
                yield return new WaitForSeconds(5f);
            }
        }

        Debug.Log("[EnemySpawner] All waves complete - VICTORY!");
    }

    private IEnumerator SpawnWave(int wave)
    {
        waveActive = true;

        int totalEnemies = waveEnemyCounts[wave - 1];

        // Spread spawning evenly over 60 seconds with some randomization
        float waveDuration = 60f;
        float baseInterval = waveDuration / totalEnemies;

        for (int i = 0; i < totalEnemies; i++)
        {
            SpawnRandomEnemy();

            // Add randomization (±30%) to prevent synchronized movement
            float randomFactor = Random.Range(0.7f, 1.3f);
            float delay = baseInterval * randomFactor;

            yield return new WaitForSeconds(delay);
        }

        waveActive = false;
    }

    [Server]
    private void SpawnRandomEnemy()
    {
        Vector2 spawnPosition = GetRandomSpawnPoint();

        GameObject prefabToSpawn;
        if (Random.value < chaserSpawnWeight)
        {
            prefabToSpawn = chaserPrefab;
        }
        else
        {
            prefabToSpawn = shooterPrefab;
        }

        if (prefabToSpawn == null)
        {
            Debug.LogError("[EnemySpawner] Enemy prefab is null!");
            return;
        }

        GameObject enemy = Instantiate(prefabToSpawn, spawnPosition, Quaternion.identity);
        ServerManager.Spawn(enemy);
    }

    private Vector2 GetRandomSpawnPoint()
    {
        Vector2 spawnPos;
        int attempts = 0;

        do
        {
            float angle = Random.Range(0f, 360f) * Mathf.Deg2Rad;
            float x = Mathf.Cos(angle) * spawnRadius;
            float y = Mathf.Sin(angle) * spawnRadius;
            spawnPos = new Vector2(x, y);
            attempts++;
        }
        while (IsTooCloseToPlayers(spawnPos) && attempts < 10);

        return spawnPos;
    }

    private bool IsTooCloseToPlayers(Vector2 position)
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject player in players)
        {
            if (Vector2.Distance(position, player.transform.position) < minSpawnDistance)
            {
                return true;
            }
        }
        return false;
    }

    public int GetCurrentWave() => currentWave.Value;
    public bool IsWaveActive() => waveActive;
}