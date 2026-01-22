/*
====================================================================
* GameStateManager.cs - Manages Game States and Flow
====================================================================
* Project: Showroom_Tango
* Course: PRG - Game & Multimedia Design SRH Hochschule
* Developer: Julian Gomez
* Date: 2025-01-20
* Version: 1.0
*
* ⚠️ WICHTIG: KOMMENTIERUNG NICHT LÖSCHEN! ⚠️
*
* [HUMAN-AUTHORED]
* - Game state requirements (Lobby, Playing, GameOver, Victory)
* - 3-wave victory condition
* - Both-players-dead game over condition
*
* [AI-ASSISTED]
* - NetworkBehaviour implementation
* - SyncVar state synchronization
* - Server-authority state transitions
* - Event system for UI integration
*
* [AI-GENERATED]
* - Complete implementation structure
====================================================================
*/

using FishNet;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum GameState
{
    Lobby,
    Playing,
    GameOver,
    Victory
}

public class GameStateManager : NetworkBehaviour
{
    public static GameStateManager Instance { get; private set; }

    [Header("Game State")]
    private readonly SyncVar<GameState> currentState = new SyncVar<GameState>(GameState.Lobby);

    [Header("References")]
    [SerializeField] private EnemySpawner enemySpawner;

    // Events for UI to subscribe to
    public delegate void GameStateChanged(GameState newState);
    public event GameStateChanged OnGameStateChanged;

    void Awake()
    {
        // Singleton pattern
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public override void OnStartNetwork()
    {
        base.OnStartNetwork();
        currentState.OnChange += HandleStateChange;
    }

    public override void OnStopNetwork()
    {
        base.OnStopNetwork();
        currentState.OnChange -= HandleStateChange;
    }

    public override void OnStartServer()
    {
        base.OnStartServer();

        // Auto-start game after 3 seconds
        Invoke(nameof(StartGameServer), 3f);
    }

    void Update()
    {
        if (!IsServerStarted) return;

        // Check win/loss conditions only during Playing state
        if (currentState.Value == GameState.Playing)
        {
            CheckVictoryCondition();
            CheckGameOverCondition();
        }
    }

    [Server]
    private void StartGameServer()
    {
        currentState.Value = GameState.Playing;
        Debug.Log("[GameStateManager] Game started!");
    }

    [Server]
    private void CheckVictoryCondition()
    {
        if (enemySpawner == null) return;

        // Victory: Wave 3 complete AND no enemies remaining
        if (enemySpawner.GetCurrentWave() >= 3 && !enemySpawner.IsWaveActive())
        {
            int enemiesRemaining = GameObject.FindGameObjectsWithTag("Enemy").Length;
            if (enemiesRemaining == 0)
            {
                currentState.Value = GameState.Victory;
                Debug.Log("[GameStateManager] VICTORY!");
            }
        }
    }

    [Server]
    private void CheckGameOverCondition()
    {
        // Game Over: All players dead
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");

        if (players.Length == 0)
        {
            currentState.Value = GameState.GameOver;
            Debug.Log("[GameStateManager] GAME OVER - No players remaining");
            return;
        }

        // Check if all players are dead
        int alivePlayers = 0;
        foreach (GameObject playerObj in players)
        {
            PlayerHealth health = playerObj.GetComponent<PlayerHealth>();
            if (health != null && health.IsAlive())
            {
                alivePlayers++;
            }
        }

        if (alivePlayers == 0)
        {
            currentState.Value = GameState.GameOver;
            Debug.Log("[GameStateManager] GAME OVER - All players dead");
        }
    }

    private void HandleStateChange(GameState prev, GameState next, bool asServer)
    {
        Debug.Log($"[GameStateManager] State changed: {prev} -> {next}");
        OnGameStateChanged?.Invoke(next);
    }

    /// <summary>
    /// Request restart from any client
    /// </summary>
    [ServerRpc(RequireOwnership = false)]
    public void RequestRestartServerRpc()
    {
        Debug.Log("[GameStateManager] Restart requested - beginning restart sequence");
        StartCoroutine(RestartGameSequence());
    }

    [Server]
    private System.Collections.IEnumerator RestartGameSequence()
    {
        Debug.Log("[GameStateManager] Step 1: Stop enemy spawner");

        // Step 1: Stop the spawner to prevent new enemies
        EnemySpawner spawner = FindAnyObjectByType<EnemySpawner>();
        if (spawner != null)
        {
            spawner.StopSpawning(); // Properly stop spawner's coroutines
        }

        Debug.Log("[GameStateManager] Step 2: Cleaning up game objects");

        // Step 2: Clean up all networked game objects
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        Debug.Log($"[GameStateManager] Despawning {enemies.Length} enemies");
        foreach (GameObject enemy in enemies)
        {
            if (enemy != null)
            {
                ServerManager.Despawn(enemy);
            }
        }

        GameObject[] bullets = GameObject.FindGameObjectsWithTag("Bullet");
        Debug.Log($"[GameStateManager] Despawning {bullets.Length} bullets");
        foreach (GameObject bullet in bullets)
        {
            if (bullet != null)
            {
                ServerManager.Despawn(bullet);
            }
        }

        // Step 3: Reset player health and position
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        Debug.Log($"[GameStateManager] Resetting {players.Length} players");

        // Find spawn points
        GameObject[] spawnPoints = GameObject.FindGameObjectsWithTag("Respawn");

        for (int i = 0; i < players.Length; i++)
        {
            PlayerHealth health = players[i].GetComponent<PlayerHealth>();
            if (health != null)
            {
                health.ResetHealthServerRpc();
            }

            // Reset position to spawn point
            if (i < spawnPoints.Length && spawnPoints[i] != null)
            {
                players[i].transform.position = spawnPoints[i].transform.position;
                Debug.Log($"[GameStateManager] Moved {players[i].name} to spawn point {i}");
            }
        }

        // Step 4: Reset score
        if (ScoreManager.Instance != null)
        {
            ScoreManager.Instance.ResetScore();
        }

        // Wait for cleanup
        yield return new WaitForSeconds(0.5f);

        Debug.Log("[GameStateManager] Step 3: Reset game state and restart");

        // Restart wave spawning
        if (spawner != null)
        {
            spawner.RestartWaves();
        }

        // CRITICAL: Reset state to Lobby, which will trigger game start after delay
        currentState.Value = GameState.Lobby;

        Debug.Log("[GameStateManager] Restart complete - game will auto-start in 3 seconds");
    }

    // Public getters
    public GameState GetCurrentState() => currentState.Value;
    public bool IsPlaying() => currentState.Value == GameState.Playing;
    public bool IsGameOver() => currentState.Value == GameState.GameOver;
    public bool IsVictory() => currentState.Value == GameState.Victory;
}
