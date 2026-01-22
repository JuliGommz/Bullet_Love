/*
====================================================================
* EnemyShooter.cs - Ranged Enemy AI
====================================================================
* Project: Showroom_Tango
* Developer: Julian Gomez
* Date: 2025-01-14
* Version: 1.0
* 
* [HUMAN-AUTHORED]
* - Keep distance behavior (5-7 units optimal)
* - Shooting interval (2 seconds)
* - Movement speed (2 units/second, slower than Chaser)
====================================================================
*/

using UnityEngine;
using FishNet.Object;
using System.Collections;

public class EnemyShooter : NetworkBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private float optimalDistance = 6f;
    [SerializeField] private float tooCloseDistance = 4f;
    [SerializeField] private float tooFarDistance = 10f;
    [SerializeField] private float shootingRange = 15f; // Max range to shoot at player

    [Header("Combat Settings")]
    [SerializeField] private float fireRate = 1.2f;
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private float bulletSpeed = 8f;
    [SerializeField] private int bulletDamage = 10;

    private Transform targetPlayer;
    private Rigidbody2D rb;
    private float lastFireTime;
    private BulletPool bulletPool;

    // Spawn protection
    private bool isInitialized = false;
    private float spawnTime;
    private const float SPAWN_PROTECTION_DURATION = 0.5f;

    // Targeting optimization
    private float targetUpdateTimer = 0f;
    private const float TARGET_UPDATE_INTERVAL = 0.5f; // Re-evaluate target every 0.5 seconds

    // Patrol behavior
    private Vector2 patrolTarget;
    private float patrolTimer = 0f;
    private const float PATROL_CHANGE_INTERVAL = 3f; // Change patrol direction every 3 seconds

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public override void OnStartServer()
    {
        base.OnStartServer();
        spawnTime = Time.time;
        isInitialized = false;
        FindNearestPlayer();
        PickNewPatrolTarget(); // Start with random patrol direction

        // Get enemy bullet pool from spawner
        EnemySpawner spawner = FindAnyObjectByType<EnemySpawner>();
        if (spawner != null)
        {
            bulletPool = spawner.GetEnemyBulletPool();
        }
        else
        {
            Debug.LogWarning("[EnemyShooter] No EnemySpawner found - bullets will not be pooled");
        }
    }

    void FixedUpdate()
    {
        if (!IsServerStarted) return;

        // Enable AI after spawn protection
        if (!isInitialized)
        {
            if (Time.time >= spawnTime + SPAWN_PROTECTION_DURATION)
            {
                isInitialized = true;
            }
            else
            {
                return; // Skip AI logic during spawn protection
            }
        }

        // Performance optimization: Re-evaluate target periodically instead of every frame
        targetUpdateTimer += Time.fixedDeltaTime;
        if (targetUpdateTimer >= TARGET_UPDATE_INTERVAL)
        {
            targetUpdateTimer = 0f;
            FindNearestPlayer();
        }

        // Also check if current target is dead
        if (targetPlayer != null)
        {
            PlayerHealth health = targetPlayer.GetComponent<PlayerHealth>();
            if (health != null && health.IsDead())
            {
                targetPlayer = null; // Force re-targeting
                FindNearestPlayer();
            }
        }

        if (targetPlayer == null)
        {
            FindNearestPlayer();
            return;
        }

        HandleMovement();
        TryShoot();
    }

    private void FindNearestPlayer()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");

        if (players.Length == 0) return;

        float closestDistance = Mathf.Infinity;
        Transform closest = null;

        foreach (GameObject player in players)
        {
            // Skip dead players
            PlayerHealth health = player.GetComponent<PlayerHealth>();
            if (health != null && health.IsDead()) continue;

            float distance = Vector2.Distance(transform.position, player.transform.position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closest = player.transform;
            }
        }

        targetPlayer = closest;
    }

    private void HandleMovement()
    {
        float distance = Vector2.Distance(transform.position, targetPlayer.position);

        Vector2 direction;

        // Back away if player gets too close
        if (distance < tooCloseDistance)
        {
            direction = (transform.position - targetPlayer.position).normalized;
        }
        // Move closer if player is too far
        else if (distance > tooFarDistance)
        {
            direction = (targetPlayer.position - transform.position).normalized;
        }
        // PATROL MODE: Move freely when in optimal range
        else
        {
            // Update patrol target periodically
            patrolTimer += Time.fixedDeltaTime;
            if (patrolTimer >= PATROL_CHANGE_INTERVAL)
            {
                patrolTimer = 0f;
                PickNewPatrolTarget();
            }

            // Move toward patrol target
            direction = (patrolTarget - rb.position).normalized;

            // If close to patrol target, pick a new one
            if (Vector2.Distance(rb.position, patrolTarget) < 1f)
            {
                PickNewPatrolTarget();
            }
        }

        Vector2 newPosition = rb.position + direction * moveSpeed * Time.fixedDeltaTime;
        rb.MovePosition(newPosition);
    }

    private void PickNewPatrolTarget()
    {
        // Pick random direction and distance for patrol
        float angle = Random.Range(0f, 360f) * Mathf.Deg2Rad;
        float distance = Random.Range(2f, 5f);
        Vector2 offset = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * distance;
        patrolTarget = rb.position + offset;
    }

    private void TryShoot()
    {
        if (Time.time < lastFireTime + fireRate) return;
        if (bulletPrefab == null) return;
        if (targetPlayer == null) return;

        // Only shoot if player is in shooting range
        float distance = Vector2.Distance(transform.position, targetPlayer.position);
        if (distance > shootingRange) return;

        lastFireTime = Time.time;

        // Fire 5 bullets in 360° star pattern (evenly distributed)
        int bulletCount = 5;
        float angleStep = 360f / bulletCount;

        for (int i = 0; i < bulletCount; i++)
        {
            // Calculate angle for this bullet (evenly distributed around 360°)
            float angle = i * angleStep;
            float angleInRadians = angle * Mathf.Deg2Rad;
            Vector2 bulletDirection = new Vector2(Mathf.Cos(angleInRadians), Mathf.Sin(angleInRadians));

            // Calculate rotation for bullet sprite
            float bulletAngle = Mathf.Atan2(bulletDirection.y, bulletDirection.x) * Mathf.Rad2Deg;
            Quaternion bulletRotation = Quaternion.Euler(0, 0, bulletAngle - 90f);

            GameObject bullet;

            // Use pool if available, otherwise fallback to instantiate
            if (bulletPool != null)
            {
                bullet = bulletPool.GetBullet(transform.position, bulletRotation);
            }
            else
            {
                // Fallback: Instantiate and spawn directly (no pooling)
                bullet = Instantiate(bulletPrefab, transform.position, bulletRotation);
                ServerManager.Spawn(bullet);
            }

            // Initialize bullet
            Bullet bulletScript = bullet.GetComponent<Bullet>();
            if (bulletScript != null)
            {
                bulletScript.Initialize(bulletPool); // Pass pool reference (null if not using pool)
            }

            // Note: Bullet movement is handled by Bullet.cs FixedUpdate using transform.up
            // The rotation (bulletRotation) determines the direction
        }

        Debug.Log($"[EnemyShooter] Fired {bulletCount}-bullet 360° star pattern");
    }
}