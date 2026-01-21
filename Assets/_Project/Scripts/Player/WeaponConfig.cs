/*
====================================================================
* WeaponConfig.cs - Weapon Data Configuration (ScriptableObject)
====================================================================
* Project: Showroom_Tango
* Developer: Julian Gomez
* Date: 2025-01-08
* Version: 1.0
* 
* [HUMAN-AUTHORED]
* - 3-weapon slot design decision
* - Bullet sprite assignments
* - Fire rate values
* 
* [AI-ASSISTED]
* - ScriptableObject pattern implementation
* - Upgrade stat structure
====================================================================
*/

using UnityEngine;

[CreateAssetMenu(fileName = "WeaponConfig", menuName = "Showroom_Tango/Weapon Config")]
public class WeaponConfig : ScriptableObject
{
    [Header("Visual")]
    public Sprite bulletSprite;
    public string weaponName;

    [Header("Stats")]
    public float baseFireRate = 0.3f;
    public float baseCooldown = 0.2f;
    public float range = 8f;
    public int damage = 10;

    [Header("Firing Position")]
    public Vector2 firePointOffset = Vector2.zero;
    public float directionAngleOffset = 0f;

    // Runtime upgrade tracking (not serialized)
    [System.NonSerialized] public int fireRateUpgrades = 0;
    [System.NonSerialized] public int cooldownUpgrades = 0;

    public float CurrentFireRate => baseFireRate - (fireRateUpgrades * 0.05f);
    public float CurrentCooldown => baseCooldown - (cooldownUpgrades * 0.03f);
}