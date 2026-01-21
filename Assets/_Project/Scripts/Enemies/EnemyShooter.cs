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
    [SerializeField] private float tooCloseDistance = 5f;
    [SerializeField] private float tooFarDistance = 7f;

    [Header("Combat Settings")]
    [SerializeField] private float fireRate = 1.2f;
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private float bulletSpeed = 8f;
    [SerializeField] private int bulletDamage = 10;

    private Transform targetPlayer;
    private Rigidbody2D rb;
    private float lastFireTime;
    private BulletPool bulletPool;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public override void OnStartServer()
    {
        base.OnStartServer();
        FindNearestPlayer();

        // Find BulletPool in scene
        bulletPool = FindAnyObjectByType<BulletPool>();
        if (bulletPool == null)
        {
            Debug.LogError("[EnemyShooter] BulletPool not found!");
        }
    }

    void FixedUpdate()
    {
        if (!IsServerStarted) return;

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

        if (distance < tooCloseDistance)
        {
            direction = (transform.position - targetPlayer.position).normalized;
        }
        else if (distance > tooFarDistance)
        {
            direction = (targetPlayer.position - transform.position).normalized;
        }
        else
        {
            Vector2 toPlayer = (targetPlayer.position - transform.position).normalized;
            direction = new Vector2(-toPlayer.y, toPlayer.x);
        }

        Vector2 newPosition = rb.position + direction * moveSpeed * Time.fixedDeltaTime;
        rb.MovePosition(newPosition);
    }

    private void TryShoot()
    {
        if (Time.time < lastFireTime + fireRate) return;
        if (bulletPool == null) return;
        if (targetPlayer == null) return;

        lastFireTime = Time.time;

        // Calculate direction to player
        Vector2 directionToPlayer = (targetPlayer.position - transform.position).normalized;

        // Fire 5 bullets in star pattern
        float[] angles = { -40f, -20f, 0f, 20f, 40f };  // 5 directions

        foreach (float angleOffset in angles)
        {
            // Calculate rotated direction
            float angleInRadians = Mathf.Atan2(directionToPlayer.y, directionToPlayer.x) + (angleOffset * Mathf.Deg2Rad);
            Vector2 bulletDirection = new Vector2(Mathf.Cos(angleInRadians), Mathf.Sin(angleInRadians));

            // Calculate rotation for bullet sprite
            float bulletAngle = Mathf.Atan2(bulletDirection.y, bulletDirection.x) * Mathf.Rad2Deg;
            Quaternion bulletRotation = Quaternion.Euler(0, 0, bulletAngle - 90f);

            // Spawn bullet from pool
            GameObject bullet = bulletPool.GetBullet(transform.position, bulletRotation);

            if (bullet != null)
            {
                // Initialize bullet
                Bullet bulletScript = bullet.GetComponent<Bullet>();
                if (bulletScript != null)
                {
                    bulletScript.Initialize(bulletPool);
                }

                // Set bullet velocity (star pattern)
                Rigidbody2D bulletRb = bullet.GetComponent<Rigidbody2D>();
                if (bulletRb != null)
                {
                    bulletRb.linearVelocity = bulletDirection * bulletSpeed;
                }
            }
        }

        Debug.Log($"[EnemyShooter] Fired 5-bullet star pattern at player");
    }
}