/*
====================================================================
* Bullet.cs - Projectile Movement and Lifetime
====================================================================
* Project: Showroom_Tango
* Course: PRG - Game & Multimedia Design SRH Hochschule
* Developer: Julian Gomez
* Date: 2025-01-08
* Version: 1.0
* 
* ⚠️ WICHTIG: KOMMENTIERUNG NICHT LÖSCHEN! ⚠️
* 
* [HUMAN-AUTHORED]
* - Lifetime requirement (5 seconds max)
* - Speed values (bullet-hell pacing)
* 
* [AI-ASSISTED]
* - NetworkBehaviour implementation
* - Pooling return integration
* - Rotation for star bullet
====================================================================
*/

using UnityEngine;
using FishNet.Object;

public class Bullet : NetworkBehaviour
{
    [Header("Movement")]
    [SerializeField] private float speed = 10f;
    [SerializeField] private bool rotateWhileMoving = false;
    [SerializeField] private float rotationSpeed = 360f;

    [Header("Lifetime")]
    [SerializeField] private float lifetime = 5f;
    [SerializeField] private float maxRange = 20f; // Maximum travel distance
    [SerializeField] private int damage = 10;

    [Header("Bullet Type")]
    [SerializeField] private bool isPlayerBullet = true; // True for player bullets, false for enemy bullets

    private float lifetimeTimer;
    private BulletPool ownerPool;
    private Vector3 spawnPosition;
    private Vector3 movementDirection; // Store initial direction for straight movement
    private GameObject ownerPlayer; // Track which player fired this bullet (for score attribution)

    public void Initialize(BulletPool pool, GameObject player = null)
    {
        ownerPool = pool;
        ownerPlayer = player; // Store player reference for score tracking
        lifetimeTimer = 0f;
        spawnPosition = transform.position;
        // Capture initial direction when bullet spawns (before any rotation)
        movementDirection = transform.up;
    }

    /// <summary>
    /// Get the player who owns this bullet (for score attribution)
    /// </summary>
    public GameObject GetOwnerPlayer() => ownerPlayer; 

    void FixedUpdate()
    {
        if (!IsServerStarted) return;

        // Move in straight line using stored direction (unaffected by visual rotation)
        transform.position += movementDirection * speed * Time.fixedDeltaTime;

        // Rotate sprite visually (for star bullet) - this doesn't affect movement direction
        if (rotateWhileMoving)
        {
            transform.Rotate(0, 0, rotationSpeed * Time.fixedDeltaTime);
        }

        // Lifetime check
        lifetimeTimer += Time.fixedDeltaTime;
        if (lifetimeTimer >= lifetime)
        {
            ReturnToPool();
            return;
        }

        // Range check (prevents hitting enemies off-screen)
        float distanceTraveled = Vector3.Distance(transform.position, spawnPosition);
        if (distanceTraveled >= maxRange)
        {
            ReturnToPool();
        }
    }

    [Server]
    private void ReturnToPool()
    {
        if (ownerPool != null)
        {
            ownerPool.ReturnBullet(gameObject);
        }
        else
        {
            // Enemy bullets aren't pooled, despawn them
            ServerManager.Despawn(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!IsServerStarted) return;

        Debug.Log($"[Bullet] {gameObject.name} (isPlayerBullet={isPlayerBullet}) collided with {collision.gameObject.name} (tag: {collision.tag})");

        // Player bullets hit enemies
        if (isPlayerBullet && collision.CompareTag("Enemy"))
        {
            EnemyHealth enemyHealth = collision.GetComponent<EnemyHealth>();
            if (enemyHealth != null)
            {
                enemyHealth.TakeDamage(damage, ownerPlayer); // Pass owner for score attribution
                Debug.Log($"[Bullet] Player bullet (owner: {ownerPlayer?.name}) hit enemy {collision.gameObject.name} for {damage} damage");
            }
            ReturnToPool();
            return;
        }

        // Enemy bullets hit players
        if (!isPlayerBullet && collision.CompareTag("Player"))
        {
            PlayerHealth playerHealth = collision.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.ApplyDamage(damage);
                Debug.Log($"[Bullet] Enemy bullet hit player {collision.gameObject.name} for {damage} damage");
            }
            ReturnToPool();
            return;
        }

        // Ignore other collisions (enemy bullets don't hit enemies, player bullets don't hit players)
    }
}