/*
====================================================================
* EnemyHealth.cs - Enemy Health & Death System
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
* - Health values (Chaser: 30 HP, Shooter: 20 HP)
*
* [AI-ASSISTED]
* - NetworkBehaviour implementation
* - Server-authority damage pattern
* - Death event system
* - FishNet despawn logic
====================================================================
*/

using UnityEngine;
using FishNet.Object;
using FishNet.Object.Synchronizing;

public class EnemyHealth : NetworkBehaviour
{
    [Header("Health Settings")]
    [SerializeField] private int maxHealth = 30; // Default: Chaser=30, Shooter=50 (set in prefab)

    [Header("Score Value")]
    [SerializeField] private int scoreValue = 10;

    private readonly SyncVar<int> currentHealth = new SyncVar<int>();

    // Spawn protection to prevent instant despawn during network initialization
    private bool isInitialized = false;
    private float spawnTime;
    private const float SPAWN_PROTECTION_DURATION = 0.5f; // 500ms invulnerability

    public override void OnStartServer()
    {
        base.OnStartServer();
        currentHealth.Value = maxHealth;
        spawnTime = Time.time;
        isInitialized = false;
        Debug.Log($"[EnemyHealth] {gameObject.name} spawned at {transform.position} with {maxHealth} HP");
    }

    public override void OnStartClient()
    {
        base.OnStartClient();
        // Mark as initialized once network replication is complete
        if (!IsServerStarted)
        {
            isInitialized = true;
        }
    }

    void Update()
    {
        // Server: Enable damage after spawn protection period
        if (IsServerStarted && !isInitialized)
        {
            if (Time.time >= spawnTime + SPAWN_PROTECTION_DURATION)
            {
                isInitialized = true;
                Debug.Log($"[EnemyHealth] {gameObject.name} spawn protection ended - now vulnerable");
            }
        }
    }

    [Server]
    public void TakeDamage(int damage, bool awardScore = true)
    {
        // Ignore damage during spawn protection
        if (!isInitialized)
        {
            Debug.LogWarning($"[EnemyHealth] {gameObject.name} blocked damage during spawn protection");
            return;
        }

        int oldHealth = currentHealth.Value;
        currentHealth.Value -= damage;
        Debug.Log($"[EnemyHealth] {gameObject.name} took {damage} damage: {oldHealth} -> {currentHealth.Value} HP");

        if (currentHealth.Value <= 0)
        {
            Debug.Log($"[EnemyHealth] {gameObject.name} died (awardScore={awardScore})");
            Die(awardScore);
        }
    }

    [Server]
    private void Die(bool awardScore = true)
    {
        // Award score via ScoreManager (only if killed by player)
        if (awardScore && ScoreManager.Instance != null)
        {
            ScoreManager.Instance.AddKillScore();
        }
        else if (!awardScore)
        {
            Debug.Log("[EnemyHealth] Suicide death - no score awarded");
        }
        else
        {
            Debug.LogWarning("[EnemyHealth] ScoreManager not found - score not awarded");
        }

        Debug.Log($"[EnemyHealth] Enemy died.");

        ServerManager.Despawn(gameObject);
    }

    public int GetCurrentHealth() => currentHealth.Value;

    public int GetMaxHealth() => maxHealth;
}