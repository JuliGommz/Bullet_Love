/*
====================================================================
* ScoreManager.cs - Multiplayer Score Tracking System
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
* - Kill reward value (10 points)
* - Wave bonus value (50 points)
* - Combined team score concept
*
* [AI-ASSISTED]
* - NetworkBehaviour implementation
* - SyncVar score synchronization
* - Server-authority score updates
* - Event system for HUD updates
*
* [AI-GENERATED]
* - Complete implementation structure
====================================================================
*/

using FishNet.Object;
using FishNet.Object.Synchronizing;
using FishNet.Connection;
using UnityEngine;

public class ScoreManager : NetworkBehaviour
{
    public static ScoreManager Instance { get; private set; }

    [Header("Score Values")]
    [SerializeField] private int killReward = 10;
    [SerializeField] private int waveClearBonus = 50;

    [Header("Synchronized Scores")]
    private readonly SyncVar<int> teamScore = new SyncVar<int>(0);
    private readonly SyncVar<int> totalKills = new SyncVar<int>(0);

    // Individual player scores (tracked by NetworkObject ID)
    private readonly SyncDictionary<int, int> playerScores = new SyncDictionary<int, int>();

    // Events for HUD to subscribe to
    public delegate void ScoreChanged(int newScore);
    public event ScoreChanged OnScoreChanged;

    public delegate void PlayerScoreChanged(GameObject player, int newScore);
    public event PlayerScoreChanged OnPlayerScoreChanged;

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
        teamScore.OnChange += HandleScoreChange;
    }

    public override void OnStopNetwork()
    {
        base.OnStopNetwork();
        teamScore.OnChange -= HandleScoreChange;
    }

    public override void OnStartServer()
    {
        base.OnStartServer();
        teamScore.Value = 0;
        totalKills.Value = 0;
    }

    /// <summary>
    /// Called by enemies when they die to award points
    /// </summary>
    [Server]
    public void AddKillScore(GameObject killerPlayer = null)
    {
        int oldScore = teamScore.Value;
        teamScore.Value += killReward;
        totalKills.Value++;

        // Award individual score if killer specified
        if (killerPlayer != null)
        {
            FishNet.Object.NetworkObject netObj = killerPlayer.GetComponent<FishNet.Object.NetworkObject>();
            if (netObj != null)
            {
                int playerId = netObj.ObjectId;

                if (!playerScores.ContainsKey(playerId))
                {
                    playerScores.Add(playerId, 0);
                }

                playerScores[playerId] += killReward;
                Debug.LogWarning($"[ScoreManager] Player {killerPlayer.name} (ID: {playerId}) scored kill! Individual: {playerScores[playerId]}, Team: {teamScore.Value}");

                // Trigger event for HUD update
                OnPlayerScoreChanged?.Invoke(killerPlayer, playerScores[playerId]);
            }
        }

        Debug.LogWarning($"[ScoreManager] KILL SCORE! {oldScore} + {killReward} = {teamScore.Value} (Kill #{totalKills.Value})");
    }

    /// <summary>
    /// Called when a wave is cleared
    /// </summary>
    [Server]
    public void AddWaveBonus(int waveNumber)
    {
        teamScore.Value += waveClearBonus;
        Debug.Log($"[ScoreManager] Wave {waveNumber} cleared! +{waveClearBonus} points. Total: {teamScore.Value}");
    }

    /// <summary>
    /// Allows clients to request score addition (for testing)
    /// </summary>
    [ServerRpc(RequireOwnership = false)]
    public void AddScoreServerRpc(int points)
    {
        teamScore.Value += points;
        Debug.Log($"[ScoreManager] Score added: +{points}. Total: {teamScore.Value}");
    }

    private void HandleScoreChange(int prev, int next, bool asServer)
    {
        Debug.Log($"[ScoreManager] Score updated: {prev} -> {next}");
        OnScoreChanged?.Invoke(next);
    }

    /// <summary>
    /// Reset score to zero (for restart)
    /// </summary>
    [Server]
    public void ResetScore()
    {
        teamScore.Value = 0;
        totalKills.Value = 0;
        playerScores.Clear();
        Debug.Log("[ScoreManager] Score, kills, and individual scores reset to 0");
    }

    // Public getters
    public int GetTeamScore() => teamScore.Value;
    public int GetTotalKills() => totalKills.Value;
    public int GetKillReward() => killReward;
    public int GetWaveBonus() => waveClearBonus;

    /// <summary>
    /// Get individual score for a specific player
    /// </summary>
    public int GetPlayerScore(GameObject player)
    {
        if (player == null) return 0;

        FishNet.Object.NetworkObject netObj = player.GetComponent<FishNet.Object.NetworkObject>();
        if (netObj == null) return 0;

        int playerId = netObj.ObjectId;
        return playerScores.ContainsKey(playerId) ? playerScores[playerId] : 0;
    }
}
