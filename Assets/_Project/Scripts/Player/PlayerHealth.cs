/*
====================================================================
* PlayerHealth.cs - Player Health & Death System
====================================================================
* Project: Showroom_Tango
* Course: PRG - Game & Multimedia Design SRH Hochschule
* Developer: Julian Gomez
* Date: 2025-01-15
* Version: 1.0
* 
* ⚠️ WICHTIG: KOMMENTIERUNG NICHT LÖSCHEN! ⚠️
* 
* [HUMAN-AUTHORED]
* - Death behavior: Spectate until both players dead
* - Visual feedback: SetActive(false)
* - Max HP: 100
* 
* [AI-ASSISTED]
* - NetworkBehaviour implementation
* - Server-authority death handling
* - SyncVar HP tracking
====================================================================
*/

using UnityEngine;
using FishNet.Object;
using FishNet.Object.Synchronizing;

public class PlayerHealth : NetworkBehaviour
{
    [Header("Health Settings")]
    [SerializeField] private int maxHealth = 100;

    private readonly SyncVar<int> currentHealth = new SyncVar<int>();
    private readonly SyncVar<bool> isDead = new SyncVar<bool>();

    public override void OnStartServer()
    {
        base.OnStartServer();
        currentHealth.Value = maxHealth;
        isDead.Value = false;
        Debug.Log($"[PlayerHealth] {gameObject.name} initialized: HP={currentHealth.Value}/{maxHealth}");
    }

    public override void OnStartNetwork()
    {
        base.OnStartNetwork();

        // Subscribe to death state changes
        isDead.OnChange += OnDeathStateChanged;
    }

    public override void OnStopNetwork()
    {
        base.OnStopNetwork();
        isDead.OnChange -= OnDeathStateChanged;
    }

    [Server]
    public void ApplyDamage(int damage)
    {
        if (isDead.Value)
        {
            Debug.LogWarning($"[PlayerHealth] {gameObject.name} is already dead, ignoring {damage} damage");
            return;
        }

        int oldHP = currentHealth.Value;
        currentHealth.Value -= damage;

        if (currentHealth.Value <= 0)
        {
            currentHealth.Value = 0;
            Die();
        }

        Debug.Log($"[PlayerHealth] {gameObject.name} took {damage} damage. HP: {oldHP} -> {currentHealth.Value}/{maxHealth} (isDead: {isDead.Value})");
    }

    [Server]
    private void Die()
    {
        isDead.Value = true;
        Debug.Log($"[PlayerHealth] {gameObject.name} died!");

        // Check if both players are dead (Game Over condition)
        CheckGameOver();
    }

    [Server]
    private void CheckGameOver()
    {
        // Find all players
        PlayerHealth[] allPlayers = FindObjectsByType<PlayerHealth>(FindObjectsSortMode.None);

        bool allDead = true;
        foreach (var player in allPlayers)
        {
            if (!player.isDead.Value)
            {
                allDead = false;
                break;
            }
        }

        if (allDead)
        {
            Debug.Log("[PlayerHealth] ALL PLAYERS DEAD - GAME OVER!");
            // TODO Phase 6: Trigger Game Over screen
        }
    }

    private void OnDeathStateChanged(bool prev, bool next, bool asServer)
    {
        if (next) // Player died
        {
            // Visual feedback: Hide sprites only (keep colliders active)
            SpriteRenderer[] sprites = GetComponentsInChildren<SpriteRenderer>();
            foreach (var sprite in sprites)
            {
                sprite.enabled = false;
            }
            Debug.Log($"[PlayerHealth] {gameObject.name} visual hidden (spectating)");
        }
    }

    // Public getters
    public int GetCurrentHealth() => currentHealth.Value;
    public int GetMaxHealth() => maxHealth;
    public bool IsDead() => isDead.Value;
    public bool IsAlive() => !isDead.Value;
}