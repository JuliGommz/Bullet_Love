/*
====================================================================
* EnemyChaser.cs - Follow Player AI
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
* - Simple follow behavior (Vector2.MoveTowards)
* - Speed value (3 units/second)
* - Collision damage concept (20 HP)
* 
* [AI-ASSISTED]
* - NetworkBehaviour implementation
* - Server-authority movement
* - Nearest player detection
* - Collision damage system
====================================================================
*/

using UnityEngine;
using FishNet.Object;

public class EnemyChaser : NetworkBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 3f;
    [SerializeField] private float speedVariation = 0.3f; // ±30% speed randomization

    [Header("Damage Settings")]
    [SerializeField] private int collisionDamage = 20;
    [SerializeField] private bool diesOnCollision = true;

    private Transform targetPlayer;
    private Rigidbody2D rb;
    private float targetUpdateTimer = 0f;
    private const float TARGET_UPDATE_INTERVAL = 0.5f; // Re-evaluate target every 0.5 seconds

    // Movement variety to prevent hordes
    private float actualMoveSpeed; // Randomized speed per enemy
    private Vector2 randomOffset; // Small random offset to spread enemies

    // Spawn protection
    private bool isInitialized = false;
    private float spawnTime;
    private const float SPAWN_PROTECTION_DURATION = 0.5f;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();

        if (rb == null)
        {
            Debug.LogError("[EnemyChaser] Rigidbody2D missing!");
        }
    }

    public override void OnStartServer()
    {
        base.OnStartServer();
        spawnTime = Time.time;
        isInitialized = false;

        // Randomize movement for variety (prevents hordes)
        actualMoveSpeed = moveSpeed * Random.Range(1f - speedVariation, 1f + speedVariation);

        // Random offset so enemies don't all converge on exact same point
        float angle = Random.Range(0f, 360f) * Mathf.Deg2Rad;
        float offsetDist = Random.Range(0.5f, 1.5f);
        randomOffset = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * offsetDist;

        FindNearestPlayer();
        Debug.Log($"[EnemyChaser] Spawned with speed {actualMoveSpeed:F2}, offset {randomOffset}");
    }

    void FixedUpdate()
    {
        if (!IsServerStarted) return;  // ✅ Use lifecycle property

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

        MoveTowardsPlayer();
    }

    private void FindNearestPlayer()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");

        if (players.Length == 0)
        {
            Debug.LogWarning("[EnemyChaser] No players found!");
            return;
        }

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

    private void MoveTowardsPlayer()
    {
        if (targetPlayer == null) return;

        // Move directly toward player + small random offset (prevents perfect stacking)
        Vector2 targetPosition = (Vector2)targetPlayer.position + randomOffset;
        Vector2 direction = (targetPosition - rb.position).normalized;

        // Move using randomized speed (creates natural spread)
        Vector2 newPosition = rb.position + direction * actualMoveSpeed * Time.fixedDeltaTime;

        rb.MovePosition(newPosition);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!IsServerStarted) return;
        if (!isInitialized) return; // Ignore collisions during spawn protection

        if (collision.gameObject.CompareTag("Player"))
        {
            Debug.Log($"[EnemyChaser] Hit player! Damage: {collisionDamage}");

            PlayerController player = collision.gameObject.GetComponent<PlayerController>();
            if (player != null)
            {
                player.TakeDamageServerRpc(collisionDamage);
            }

            // Kamikaze behavior - die on collision (no individual score, but team score awarded)
            if (diesOnCollision)
            {
                EnemyHealth health = GetComponent<EnemyHealth>();
                if (health != null)
                {
                    health.TakeDamage(int.MaxValue, attackerPlayer: null); // Suicide - team score only
                }
            }
        }
    }
}