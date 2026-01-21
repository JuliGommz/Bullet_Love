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
    [SerializeField] private int damage = 10;

    private float lifetimeTimer;
    private BulletPool ownerPool;

    public void Initialize(BulletPool pool)
    {
        ownerPool = pool;
        lifetimeTimer = 0f;
    } 

    void FixedUpdate()
    {
        if (!IsServerStarted) return;

        transform.position += transform.up * speed * Time.fixedDeltaTime;

        // Rotate if enabled (for star bullet)
        if (rotateWhileMoving)
        {
            transform.Rotate(0, 0, rotationSpeed * Time.fixedDeltaTime);
        }

        // Lifetime check
        lifetimeTimer += Time.fixedDeltaTime;
        if (lifetimeTimer >= lifetime)
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
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!IsServerStarted) return;

        // Check if hit enemy
        if (collision.CompareTag("Enemy"))
        {
            EnemyHealth enemyHealth = collision.GetComponent<EnemyHealth>();
            if (enemyHealth != null)
            {
                enemyHealth.TakeDamage(damage);
                Debug.Log($"[Bullet] Hit enemy for {damage} damage");
            }
        }

        // Return bullet to pool after hit
        ReturnToPool();
    }
}