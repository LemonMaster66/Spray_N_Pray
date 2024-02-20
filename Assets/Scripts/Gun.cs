using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

[Serializable]
public class Gun : MonoBehaviour
{
    [Header("References")]
    public GameObject BulletPrefab;
    public LayerMask  layerMask;

    [Header("Types")]
    public bool Projectile        = false;   // If the fired Bullet is a Projectile
    public bool Automatic         = false;   // Hold Down the Mouse to Continue Firing
    public bool DestroyOnImpact   = true;    // Bullets get Destroyed when Colliding
    public bool ExploadOnImpact   = false;   // Bullets Expload when Colliding
    public bool RicoOnTargetHit   = false;   // Bullets can Ricochet off of enemies
    public bool ExplodeAfterTime  = false;   // Bullet will auto Explode after an amount of time
    public bool CanParryBullet    = false;   // Punch after Shooting to Parry Your Own Bullets
    public bool IgniteEnemies     = false;   // Sets Enemies on Fire
    public bool SelfDamage        = false;   // Can Deal Damage to Yourself

    [Header("Properties")]
    public float Damage;                     // Damage on Hit                                             |  0 = None
    public float MinDamage;                  // The Damage Applied at Max Falloff Distance                |  0 = None
    public int   Ammo;                       // Bullets Shot Before Reloading                             |  0 = Unlimited
    public float AttackSpeed;                // Time Between Shots                                        |  0 = 1 Frame
    public float ReloadSpeed;                // Time Taken to Reload                                      |  0 = Instant
    public int   MultiShot;                  // Number Bullets Shot at the Same Time                      |  0 = None
    public float MultiShotInterval;          // Time Between Each MultiShot Bullet                        |  0 = Instant
    public float Spread;                     // The Amount that Each Bullet Strays from the Target        |  0 = None
    public float Knockback;                  // The Force Applied to the Target from 1 Bullet             |  0 = None
    public float FalloffDistance;            // The Distance it takes to Lose All Damage                  |  0 = Disabled
    public float FalloffTime;                // The Time it takes to Lose All Damage                      |  0 = Disabled
    public int   RicochetCount;              // The Number of Times it Bounces before Destroying          |  0 = None
    public int   RicochetMultiplier;         // The Damage Multiplier Per Ricochet                        |  0 = None
    public int   PenetrateCount;             // The Amount of Targets it Pierces Through                  |  0 = None

    [Header("Hitscan Properties")]
    public float BulletTrailSpeed = 400f;    // The time it takes for the Bullet to Reach the Target      |  0 = Instant

    [Header("Projectile Properties")]
    public float ProjectileSpeed;            // The Speed of the Bullet                                   |  0 = Frozen
    public float ProjectileBounce;           // The Bounciness Value of the Bullet                        |  0 = None
    public float ProjectileGravity;          // The Gravity Force of the Bullet                           |  0 = None
    public float ProjectileLifeSpan;         // The Time it takes for the Projectile Dies                 |  0 = Unlimited

    [Header("Explosion Properties")]
    public float ExplosionSize;              // The Size of the Explosion
    public float SplashDamage;               // The Max Damage Recieved from being in the Explosion
    public float ExplosionKnockback;         // The Force Applied to the Target away from the Explosion

    [Header("States")]
    public bool CanShoot          = true;
    public bool AttackCooldown    = false;
    public bool Reloading         = false;
    public bool HoldingShoot      = false;

    [Header("Info")]
    public float attackCooldownTime;
    public float reloadTime;
    public float currentAmmo;
    public float currentMultiShot;

    private float FinalDamage;

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
        //Attack Speed Cooldown
        if(AttackCooldown) attackCooldownTime -= Time.deltaTime;
        if(attackCooldownTime < 0)
        {
            AttackCooldown = false;
            attackCooldownTime = 0;

            if(HoldingShoot && Automatic) ShootStart();
        }

        FinalDamage        = (float)Math.Round(FinalDamage, 2);
        attackCooldownTime = (float)Math.Round(attackCooldownTime, 2);
    }


    public virtual void ShootStart()
    {
        HoldingShoot = true;

        // Checks
        if (!CanShoot || AttackCooldown) return;

        // Values
        AttackCooldown = true;
        attackCooldownTime += AttackSpeed;

        // MultiShot
        if(MultiShot == 0) StartCoroutine(Shoot(0));
        if(MultiShot != 0)
        {
            for (int i = 0; i < MultiShot; i++)
            {
                StartCoroutine(Shoot(i * MultiShotInterval));
            }
        }
    }
    private IEnumerator Shoot(float interval)
    {
        yield return new WaitForSeconds(interval);

        Vector3 shootVector = Camera.transform.forward;
        if (Spread != 0)
        {
            shootVector.x += UnityEngine.Random.Range(-Spread / 200, Spread / 200);
            shootVector.y += UnityEngine.Random.Range(-Spread / 200, Spread / 200);
            shootVector.z += UnityEngine.Random.Range(-Spread / 200, Spread / 200);
        }

        //****************************************************************
        //Hitscan
        if (!Projectile)
        {
            //**********************************
            // Epic Hit
            if (Physics.Raycast(Camera.transform.position, shootVector, out RaycastHit hit, 100000 , layerMask))
            {
                FinalDamage = Damage;
                if(MultiShot != 0) FinalDamage /= MultiShot;

                GameObject bullet = Instantiate(BulletPrefab, GunTip.position, Quaternion.identity);
                StartCoroutine(SpawnBullet(bullet, hit.point, shootVector, hit, true, RicochetCount, FinalDamage, 0));

                #region Extra Logic
                    // Knockback
                    if (hit.rigidbody != null) hit.rigidbody.AddForce(-hit.normal * Knockback * 10);
                #endregion
            }

            //**********************************
            // Epic Miss
            else
            {
                FinalDamage = 0;
                GameObject bullet = Instantiate(BulletPrefab, GunTip.position, Quaternion.identity);
                bullet.transform.parent = hit.transform;
                StartCoroutine(SpawnBullet(bullet, shootVector*1000, shootVector, hit, false, 0, FinalDamage, 0));
            }
        }
    }
    
    public virtual void ShootEnd()
    {
        HoldingShoot = false;
    }

    public virtual void AltShootStart()
    {
        //Debug.Log("Shoot Gun");
    }
    public virtual void AltShootEnd()
    {
        
    }

    private IEnumerator SpawnBullet(GameObject bullet, Vector3 HitPoint, Vector3 shootVector, RaycastHit hit, bool Impacted, int ricoRemaining, float finalDamage, float finalDistance)
    {
        Vector3 startPosition = bullet.transform.position;

        float distance = Vector3.Distance(bullet.transform.position, HitPoint);
        finalDistance += distance;
        float startingDistance = distance;
        
        if(BulletTrailSpeed != 0)
        {
            while(distance > 0)
            {
                bullet.transform.position = Vector3.Lerp(startPosition, HitPoint, 1 - (distance / startingDistance));
                distance -= Time.deltaTime * BulletTrailSpeed;
                
                yield return null;
            }
        }
        bullet.transform.position = HitPoint;

        if(Impacted)
        {
            //Spawn Particle System

            // Damage Distance Falloff
            if (FalloffDistance != 0)
            {
                float falloffFactor = Mathf.Clamp01(Mathf.SmoothStep(0, 1, 1 - (finalDistance / FalloffDistance)));
                finalDamage = Mathf.Lerp(MinDamage, Damage, falloffFactor);
            }

            TargetPoint targetPoint = hit.transform.GetComponent<TargetPoint>();
            if(targetPoint != null)
            {
                targetPoint.OnHit(finalDamage);
                if(!RicoOnTargetHit) ricoRemaining = 0;
            }

            
            if(ricoRemaining > 0)
            {
                yield return new WaitForSeconds(0.05f);

                finalDamage *= RicochetMultiplier;

                Vector3 ricochetDirection = Vector3.Reflect(shootVector, hit.normal);
                if(Physics.Raycast(HitPoint, ricochetDirection, out RaycastHit ricoHit, 100000, layerMask))
                {
                    if(ricoHit.collider != null && ricoHit.collider.gameObject.layer == 3 && SelfDamage)
                    {
                        Debug.Log("Stop Hitting Yourself");
                    }
                    yield return StartCoroutine(SpawnBullet(bullet, ricoHit.point, ricochetDirection, ricoHit, true, ricoRemaining-1, finalDamage, finalDistance));
                }
                else
                {
                    yield return StartCoroutine(SpawnBullet(bullet, ricochetDirection*100, ricochetDirection, ricoHit, false, 0, 0, finalDistance));
                }
            }
            else
            {
                bullet.transform.parent = hit.transform;
            }

            if(DestroyOnImpact || (hit.collider != null && hit.collider.gameObject.layer == 3)) Destroy(bullet.gameObject, 0.1f);
        }
        else Destroy(bullet.gameObject, 0.1f);
    }
}
