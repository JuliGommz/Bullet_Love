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
* - Death triggers collectible drop (every 3rd kill)
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
    [SerializeField] private int maxHealth = 30;

    [Header("Score Value")]
    [SerializeField] private int scoreValue = 10;

    private readonly SyncVar<int> currentHealth = new SyncVar<int>();

    private static int killCount = 0;

    public override void OnStartServer()
    {
        base.OnStartServer();
        currentHealth.Value = maxHealth;
    }

    [Server]
    public void TakeDamage(int damage, bool awardScore = true)
    {
        currentHealth.Value -= damage;

        if (currentHealth.Value <= 0)
        {
            Die(awardScore);
        }
    }

    [Server]
    private void Die(bool awardScore = true)
    {
        killCount++;

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

        if (killCount % 3 == 0)
        {
            Debug.Log($"[EnemyHealth] 3rd kill reached! Collectible should spawn at {transform.position}");
        }

        Debug.Log($"[EnemyHealth] Enemy died.");

        ServerManager.Despawn(gameObject);
    }

    public int GetCurrentHealth() => currentHealth.Value;

    public int GetMaxHealth() => maxHealth;
}