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
        Debug.Log("[GameStateManager] Restart requested");
        RestartGame();
    }

    [Server]
    private void RestartGame()
    {
        // Reload current scene using FishNet's SceneManager for network sync
        string currentScene = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;

        FishNet.Managing.Scened.SceneLoadData sld = new FishNet.Managing.Scened.SceneLoadData(currentScene);
        sld.ReplaceScenes = FishNet.Managing.Scened.ReplaceOption.All;

        NetworkManager.SceneManager.LoadGlobalScenes(sld);
    }

    // Public getters
    public GameState GetCurrentState() => currentState.Value;
    public bool IsPlaying() => currentState.Value == GameState.Playing;
    public bool IsGameOver() => currentState.Value == GameState.GameOver;
    public bool IsVictory() => currentState.Value == GameState.Victory;
}
