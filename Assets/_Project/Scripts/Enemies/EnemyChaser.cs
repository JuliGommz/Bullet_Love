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

    [Header("Damage Settings")]
    [SerializeField] private int collisionDamage = 20;
    [SerializeField] private bool diesOnCollision = true;

    private Transform targetPlayer;
    private Rigidbody2D rb;

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
        FindNearestPlayer();
    }

    void FixedUpdate()
    {
        if (!IsServerStarted) return;  // ✅ Use lifecycle property

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

        Vector2 direction = (targetPlayer.position - transform.position).normalized;
        Vector2 newPosition = Vector2.MoveTowards(rb.position, targetPlayer.position, moveSpeed * Time.fixedDeltaTime);

        rb.MovePosition(newPosition);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!IsServerStarted) return;

        if (collision.gameObject.CompareTag("Player"))
        {
            Debug.Log($"[EnemyChaser] Hit player! Damage: {collisionDamage}");

            PlayerController player = collision.gameObject.GetComponent<PlayerController>();
            if (player != null)
            {
                player.TakeDamageServerRpc(collisionDamage);
            }

            // Kamikaze behavior - die on collision (no score awarded)
            if (diesOnCollision)
            {
                EnemyHealth health = GetComponent<EnemyHealth>();
                if (health != null)
                {
                    health.TakeDamage(int.MaxValue, awardScore: false); // Suicide - no score
                }
            }
        }
    }
}