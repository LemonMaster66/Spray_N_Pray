using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

[System.Serializable]
public class Gun : MonoBehaviour
{
    [Header("References")]
    public GameObject BulletPrefab;

    [Header("Types")]
    public bool Projectile;           // If the fired Bullet is a Projectile
    public bool Automatic;            // Hold Down the Mouse to Continue Firing
    public bool DestroyOnImpact;      // Bullets get Destroyed when Colliding
    public bool ExploadOnImpact;      // Bullets Expload when Colliding
    public bool ExplodeAfterTime;     // Bullet will auto Explode after an amount of time
    public bool CanParryBullet;       // Punch after Shooting to Parry Your Own Bullets
    public bool DamageFallOff;        // The Distance before the Bullets Lose All Damage
    public bool IgniteEnemies;        // Sets Enemies on Fire

    [Header("Properties")]
    public float Damage;               // Damage on Hit                                             |  0 = None
    public float MinDamage;            // The Damage Applied at Max Falloff Distance                |  0 = None
    public int   Ammo;                 // Bullets Shot Before Reloading                             |  0 = Unlimited
    public float AttackSpeed;          // Time Between Shots                                        |  0 = 1 Frame
    public float ReloadSpeed;          // Time Taken to Reload                                      |  0 = Instant
    public int   MultiShot;            // Number Bullets Shot at the Same Time                      |  0 = None
    public float MultiShotInterval;    // Time Between Each MultiShot Bullet                        |  0 = Instant
    public float Spread;               // The Amount that Each Bullet Strays from the Target        |  0 = None
    public float Knockback;            // The Force Applied to the Target from 1 Bullet             |  0 = None
    public float FalloffDistance;      // The Distance it takes to Lose All Damage                  |  0 = Disabled
    public float FalloffTime;          // The Time it takes to Lose All Damage                      |  0 = Disabled
    public int   Ricochete;            // The Number of Times it Bounces before Destroying          |  0 = None
    public int   Penetrate;            // The Amount of Targets it Pierces Through                  |  0 = None

    [Header("Projectile Properties")]
    public float ProjectileSpeed;      // The Speed of the Bullet                                   |  0 = Frozen
    public float ProjectileBounce;     // The Bounciness Value of the Bullet                        |  0 = None
    public float ProjectileGravity;    // The Gravity Force of the Bullet                           |  0 = None
    public float ProjectileLifeSpan;   // The Time it takes for the Projectile Dies                 |  0 = Unlimited

    [Header("Explosion Properties")]
    public float ExplosionSize;        // The Size of the Explosion
    public float SplashDamage;         // The Max Damage Recieved from being in the Explosion
    public float ExplosionKnockback;   // The Force Applied to the Target away from the Explosion

    [Header("States")]
    public bool CanShoot = true;
    public bool AttackCooldown;
    public bool Reloading;
    public bool HoldingShoot;

    [Header("Info")]
    public float finalDamage;
    public float attackCooldowntime;
    public float currentAmmo;

    #region Debug Values
        private Transform       GunTip;
        private Transform       Camera;
        private Rigidbody       rb;
        private PlayerMovement  playerMovement;

        [HideInInspector] public float _damage;
    #endregion


    void Awake()
    {
        GunTip          = gameObject.transform.GetChild(0);
        Camera          = GameObject.Find("Main Camera").transform;
        rb              = GetComponent<Rigidbody>();
        playerMovement  = FindFirstObjectByType<PlayerMovement>();
        
        currentAmmo = Ammo;
    }

    void FixedUpdate()
    {
        if(AttackCooldown) attackCooldowntime -= Time.deltaTime;
        if(attackCooldowntime < 0)
        {
            AttackCooldown = false;
            attackCooldowntime = 0;
        }

        finalDamage        = (float)Math.Round(finalDamage, 2);
        attackCooldowntime = (float)Math.Round(attackCooldowntime, 2);
    }


    public virtual void FireStarted()
    {
        HoldingShoot = true;
        Vector3 shootVector = Camera.transform.forward;

        // Checks
        if(!CanShoot || AttackCooldown) return;

        // Values
        AttackCooldown = true;
        attackCooldowntime += AttackSpeed;

        if(!Projectile)
        {
            Debug.DrawRay(Camera.transform.position, shootVector*FalloffDistance, Color.red, 1);
            RaycastHit hit;
            if(Physics.Raycast(Camera.transform.position, shootVector, out hit))
            {
                finalDamage = Damage;

                // Damage Distance Falloff
                if(FalloffDistance != 0)
                {
                    float falloffFactor = Mathf.Clamp01(Mathf.SmoothStep(0, 1, 1 - (hit.distance / FalloffDistance)));
                    finalDamage = Mathf.Lerp(MinDamage, Damage, falloffFactor);
                }


                // Knockback
                if(hit.rigidbody != null) hit.rigidbody.AddForce(-hit.normal * Knockback *10);
            }
        }
    }
    
    public virtual void FireEnded()
    {
        HoldingShoot = false;
    }

    public virtual void AltFireStarted()
    {
        Debug.Log("Shoot Gun");
    }
    public virtual void AltFireEnded()
    {
        
    }
}
