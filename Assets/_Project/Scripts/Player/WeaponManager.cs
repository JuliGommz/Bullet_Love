/*
====================================================================
* WeaponManager.cs - Multi-Weapon System Controller
====================================================================
* Project: Showroom_Tango
* Developer: Julian Gomez
* Date: 2025-01-08
* Version: 1.0
* 
* [HUMAN-AUTHORED]
* - 3-weapon slot limitation
* - Priority targeting system (nearest, 2nd nearest, 3rd nearest)
* - Directional offset strategy
* 
* [AI-ASSISTED]
* - Auto-targeting implementation
* - Weapon slot management
* - Enemy proximity detection with sorting
====================================================================
*/

using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using FishNet.Object;

public class WeaponManager : NetworkBehaviour
{
    [Header("Weapon Slots (Max 3)")]
    [SerializeField] private List<WeaponConfig> equippedWeapons = new List<WeaponConfig>();

    [Header("References")]
    [SerializeField] private BulletPool bulletPool; // ASSIGN IN INSPECTOR
    [SerializeField] private Transform playerTransform;

    [Header("Enemy Detection")]
    [SerializeField] private LayerMask enemyLayer;

    private Dictionary<WeaponConfig, float> weaponLastFireTime = new Dictionary<WeaponConfig, float>();
    private PlayerHealth playerHealth;

    void Start()
    {
        // Get PlayerHealth reference
        playerHealth = GetComponent<PlayerHealth>();

        // Initialize fire times
        foreach (var weapon in equippedWeapons)
        {
            weaponLastFireTime[weapon] = 0f;
        }

        // Auto-find BulletPool if not assigned
        if (bulletPool == null)
        {
            // Find ALL bullet pools, then select player pool by name
            BulletPool[] allPools = FindObjectsByType<BulletPool>(FindObjectsSortMode.None);

            foreach (BulletPool pool in allPools)
            {
                if (pool.gameObject.name.Contains("Player"))
                {
                    bulletPool = pool;
                    Debug.Log($"[WeaponManager] Player BulletPool auto-found: {pool.gameObject.name}");
                    break;
                }
            }

            if (bulletPool == null)
            {
                Debug.LogError("[WeaponManager] Player BulletPool not found in scene! Weapon system disabled.");
                enabled = false;
            }
        }
    }

    void Update()
    {
        if (!IsOwner) return;

        // Don't shoot if dead
        if (playerHealth != null && playerHealth.IsDead()) return;

        AutoFireAllWeapons();
    }

    private void AutoFireAllWeapons()
    {
        // Fire each weapon at priority target
        for (int i = 0; i < equippedWeapons.Count; i++)
        {
            WeaponConfig weapon = equippedWeapons[i];

            // Check cooldown
            if (Time.time < weaponLastFireTime[weapon] + weapon.CurrentFireRate) continue;

            // Find enemies in THIS weapon's range
            List<GameObject> enemiesInRange = FindEnemiesInRange(weapon.range);
            if (enemiesInRange.Count == 0) continue;

            // Get target based on weapon priority
            GameObject target = GetTargetForWeapon(i, enemiesInRange);
            if (target == null) continue;

            // Calculate fire position with offset
            Vector3 firePosition = playerTransform.position + (Vector3)weapon.firePointOffset;

            // Calculate direction to target with angle offset
            Vector2 directionToTarget = (target.transform.position - firePosition).normalized;
            float baseAngle = Mathf.Atan2(directionToTarget.y, directionToTarget.x) * Mathf.Rad2Deg;
            float finalAngle = baseAngle + weapon.directionAngleOffset;
            Quaternion rotation = Quaternion.Euler(0, 0, finalAngle - 90f); // -90 because sprite points up

            // Fire weapon
            weaponLastFireTime[weapon] = Time.time;
            FireWeaponServerRpc(firePosition, rotation, weapon.bulletSprite.name);
        }
    }

    private List<GameObject> FindEnemiesInRange(float range)
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(
            playerTransform.position,
            range,
            enemyLayer
        );

        List<GameObject> enemies = new List<GameObject>();
        foreach (var col in colliders)
        {
            if (col.CompareTag("Enemy"))
            {
                enemies.Add(col.gameObject);
            }
        }

        // Sort by distance (nearest first)
        enemies = enemies.OrderBy(e =>
            Vector2.Distance(playerTransform.position, e.transform.position)
        ).ToList();

        return enemies;
    }

    private GameObject GetTargetForWeapon(int weaponIndex, List<GameObject> enemies)
    {
        // Priority system:
        // Weapon 0: nearest enemy
        // Weapon 1: 2nd nearest (or nearest if only 1)
        // Weapon 2: 3rd nearest (or nearest if only 1-2)

        if (weaponIndex >= enemies.Count)
        {
            // More weapons than enemies - target nearest
            return enemies[0];
        }

        return enemies[weaponIndex];
    }

    [ServerRpc]
    private void FireWeaponServerRpc(Vector3 position, Quaternion rotation, string bulletSpriteName)
    {
        if (bulletPool == null)
        {
            Debug.LogError("BulletPool reference missing!");
            return;
        }

        GameObject bullet = bulletPool.GetBullet(position, rotation);

        if (bullet != null)
        {
            // Set correct sprite for this weapon
            SpriteRenderer sr = bullet.GetComponent<SpriteRenderer>();
            if (sr != null)
            {
                Sprite sprite = Resources.Load<Sprite>($"Sprites/{bulletSpriteName}");
                if (sprite != null) sr.sprite = sprite;
            }

            Bullet bulletScript = bullet.GetComponent<Bullet>();
            if (bulletScript != null)
            {
                bulletScript.Initialize(bulletPool, gameObject); // Pass this player as owner
            }
        }
    }

    /// <summary>
    /// Adds weapon to next available slot (max 3)
    /// </summary>
    public bool AddWeapon(WeaponConfig weapon)
    {
        if (equippedWeapons.Count >= 3)
        {
            Debug.LogWarning("Maximum 3 weapons equipped!");
            return false;
        }

        equippedWeapons.Add(weapon);
        weaponLastFireTime[weapon] = Time.time;
        Debug.Log($"Weapon added: {weapon.weaponName}");
        return true;
    }

    /// <summary>
    /// Upgrades fire rate for all weapons
    /// </summary>
    public void UpgradeFireRate()
    {
        foreach (var weapon in equippedWeapons)
        {
            weapon.fireRateUpgrades++;
        }
        Debug.Log("Fire rate upgraded!");
    }

    /// <summary>
    /// Upgrades cooldown for all weapons
    /// </summary>
    public void UpgradeCooldown()
    {
        foreach (var weapon in equippedWeapons)
        {
            weapon.cooldownUpgrades++;
        }
        Debug.Log("Cooldown upgraded!");
    }
}