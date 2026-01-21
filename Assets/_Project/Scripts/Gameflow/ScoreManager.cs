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

    // Events for HUD to subscribe to
    public delegate void ScoreChanged(int newScore);
    public event ScoreChanged OnScoreChanged;

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
    }

    /// <summary>
    /// Called by enemies when they die to award points
    /// </summary>
    [Server]
    public void AddKillScore()
    {
        int oldScore = teamScore.Value;
        teamScore.Value += killReward;
        Debug.LogWarning($"[ScoreManager] KILL SCORE! {oldScore} + {killReward} = {teamScore.Value}");
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

    // Public getters
    public int GetTeamScore() => teamScore.Value;
    public int GetKillReward() => killReward;
    public int GetWaveBonus() => waveClearBonus;
}
