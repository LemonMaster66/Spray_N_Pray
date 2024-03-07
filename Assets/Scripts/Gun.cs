using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using Unity.Mathematics;

public class Gun : MonoBehaviour
{
    [Header("References")]
    public GameObject BulletPrefab;
    public LayerMask  layerMask;

    [Header("Types")]
    public bool Projectile        = false;   // If the fired Bullet is a Projectile
    public bool Automatic         = false;   // Hold Down the Mouse to Continue Firing
    public bool DestroyOnImpact   = true;    // Bullets get Destroyed when Colliding
    public bool ExploadOnDestroy  = false;   // Bullets Expload when Colliding
    public bool RicoOnHit         = false;   // Bullets can Ricochet off of enemies
    public bool CanParryBullet    = false;   // Punch after Shooting to Parry Your Own Bullets
    public bool IgniteEnemies     = false;   // Sets Enemies on Fire
    public bool SelfDamage        = false;   // Can Deal Damage to Yourself

    [Header("Properties")]
    public float Damage;                     // Damage on Hit                                             |  0 = None
    public float MinDamage;                  // The Damage Applied at Max Falloff Distance                |  0 = None
    public float DamageFalloff;              // The Time it takes to go from Damage to MinDamage          |  0 = Disabled
    public int   Ammo;                       // Bullets Shot Before Reloading                             |  0 = Unlimited
    public float AttackSpeed;                // Time Between Shots                                        |  0 = 1 Frame
    public float ReloadSpeed;                // Time Taken to Reload                                      |  0 = Instant
    public int   MultiShot;                  // Number Bullets Shot at the Same Time                      |  0 = None
    public float MultiShotInterval;          // Time Between Each MultiShot Bullet                        |  0 = Instant
    public float Spread;                     // The Amount that Each Bullet Strays from the Target        |  0 = None
    public float Knockback;                  // The Force Applied to the Target from 1 Bullet             |  0 = None
    public int   RicochetCount;              // The Number of Times it Bounces before Destroying          |  0 = None
    public float RicochetMultiplier = 1;     // The Damage Multiplier Per Ricochet                        |  0 = None
    public int   PenetrateCount;             // The Amount of Targets it Pierces Through                  |  0 = None

    [Header("Projectile Properties")]
    public bool  InheritVelocity;            // Adds the Players Velocity to the Projectile
    public bool  ProjectileSticky;           // Bullets get Stuck to whatever they Collide with
    public float ProjectileSpeed;            // The Speed of the Bullet                                   |  0 = Frozen
    public float ProjectileGravity;          // The Gravity Force of the Bullet                           |  0 = None
    public float ProjectileLifeSpan;         // The Time it takes for the Projectile Dies                 |  0 = Unlimited

    [Header("Explosion Properties")]
    public float ExplosionSize;              // The Size of the Explosion
    public float SplashDamage;               // The Max Damage Recieved from being in the Explosion
    public float ExplosionKnockback;         // The Force Applied to the Target away from the Explosion

    [Header("States")]
    public bool CanShoot           = true;
    public bool AttackCooldown     = false;
    public bool MultiShotCooldown  = false;
    public bool Reloading          = false;
    public bool HoldingShoot       = false;

    [Header("Info")]
    public float attackCooldownTime;
    public float multiShotCooldownTime;
    private float reloadTime;
    private float currentAmmo;
    private float currentMultiShot;

    private float FinalDamage;

    #region Debug Values
        private Transform       GunTip;
        private Transform       cam;
        private Rigidbody       rb;
        private PlayerMovement  playerMovement;

        [HideInInspector] public float _damage;
    #endregion


    void Awake()
    {
        GunTip          = gameObject.transform.GetChild(0);
        cam             = Camera.main.transform;
        rb              = GetComponent<Rigidbody>();
        playerMovement  = FindFirstObjectByType<PlayerMovement>();
        
        currentAmmo = Ammo;
    }

    void FixedUpdate()
    {
        //Attack Speed Cooldown
        if(AttackCooldown) attackCooldownTime -= Time.deltaTime;
        if(attackCooldownTime < 0)
        {
            AttackCooldown = false;
            attackCooldownTime = 0;

            if(HoldingShoot && Automatic) ShootStart();
        }

        //MultiShot Cooldown
        if(MultiShotCooldown) multiShotCooldownTime -= Time.deltaTime;
        if(multiShotCooldownTime < 0)
        {
            AttackCooldown = true;
            attackCooldownTime += AttackSpeed;

            MultiShotCooldown = false;
            multiShotCooldownTime = 0;

        }

        FinalDamage           = (float)Math.Round(FinalDamage, 2);
        attackCooldownTime    = (float)Math.Round(attackCooldownTime, 2);
        multiShotCooldownTime = (float)Math.Round(multiShotCooldownTime, 2);
    }


    public virtual void ShootStart()
    {
        HoldingShoot = true;

        // Checks
        if (!CanShoot || AttackCooldown || MultiShotCooldown) return;

        // MultiShot
        if(MultiShot < 2) StartCoroutine(Shoot(0));
        else if(MultiShot > 1)
        {
            for (int i = 0; i < MultiShot; i++)
            {
                multiShotCooldownTime += MultiShotInterval;
                StartCoroutine(Shoot(i));
            }
        }
        MultiShotCooldown = true;
        multiShotCooldownTime -= MultiShotInterval; 
    }

    private IEnumerator Shoot(int BulletNumber)
    {
        yield return new WaitForSeconds(BulletNumber * MultiShotInterval);

        Vector3 shootVector = CalculateShootVector();

        //****************************************************************
        //Hitscan
        if (!Projectile)
        {
            if (Physics.Raycast(cam.transform.position, shootVector, out RaycastHit hit, 100000, layerMask))
            {
                GameObject bullet = Instantiate(BulletPrefab, GunTip.position, Quaternion.identity);
                SpawnHitscanBullet(bullet, hit.point, shootVector, hit);
            }
            else //Miss
            {
                GameObject bullet = Instantiate(BulletPrefab, GunTip.position, Quaternion.identity);
                SpawnHitscanBullet(bullet, shootVector*1000, shootVector, hit);
            }
        }

        //Projectile
        else
        {
            GameObject bullet = Instantiate(BulletPrefab, GunTip.position, Quaternion.identity);
            SpawnProjectileBullet(bullet, shootVector);
        }
    }
    
    public virtual void ShootEnd()
    {
        HoldingShoot = false;
    }

    public virtual void AltShootStart()
    {
        GameObject[] Bullets = GameObject.FindGameObjectsWithTag("Bullet");
        foreach(GameObject bullet in Bullets) Destroy(bullet);
    }
    public virtual void AltShootEnd()
    {
        
    }


    //***********************************************************
    //***********************************************************
    // Logic

    public virtual Vector3 CalculateShootVector()
    {
        Vector3 shootVector = cam.transform.forward;
        if (Spread != 0)
        {
            shootVector.x += UnityEngine.Random.Range(-Spread / 200, Spread / 200);
            shootVector.y += UnityEngine.Random.Range(-Spread / 200, Spread / 200);
            shootVector.z += UnityEngine.Random.Range(-Spread / 200, Spread / 200);
        }
        shootVector = shootVector.normalized;
        return shootVector;
    }

    private void SpawnHitscanBullet(GameObject bullet, Vector3 HitPoint, Vector3 shootVector, RaycastHit hit)
    {
        HitScan hitScan = bullet.GetComponent<HitScan>();
        StartCoroutine(hitScan.SpawnHitscanBullet(HitPoint, shootVector, hit, true, RicochetCount, 0));
        hitScan.AssignOrigin(this);
    }

    private void SpawnProjectileBullet(GameObject bullet, Vector3 shootVector)
    {
        Rigidbody rb = bullet.GetComponent<Rigidbody>();

        rb.velocity =  shootVector * ProjectileSpeed;
        if(InheritVelocity) rb.velocity += playerMovement.rb.velocity;

        bullet.GetComponent<Projectile>().AssignOrigin(this);
    }

    public void Explode()
    {
        Debug.Log("Boom");
    }
}
