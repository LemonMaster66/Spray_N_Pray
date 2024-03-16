using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitScan : MonoBehaviour
{
    [Header("References")]
    private Gun OriginGun;
    private LayerMask layerMask;
    public List<Target> targetsHit = new List<Target>();

    [Header("Types")]
    private bool Automatic         = false;   // Hold Down the Mouse to Continue Firing
    private bool DestroyOnImpact   = true;    // Bullets get Destroyed when Colliding
    private bool ExploadOnDestroy  = false;   // Bullets Expload when Colliding
    private bool RicoOnHit         = false;   // Bullets can Ricochet off of enemies
    private bool CanParryBullet    = false;   // Punch after Shooting to Parry Your Own Bullets
    private bool IgniteEnemies     = false;   // Sets Enemies on Fire
    private bool SelfDamage        = false;   // Can Deal Damage to Yourself

    [Header("Properties")]
    private float Damage;                     // Damage on Hit                                             |  0 = None
    private float MinDamage;                  // The Damage Applied at Max Falloff Distance                |  0 = None
    private float DamageFalloff;              // The Time it takes to go from Damage to MinDamage          |  0 = Disabled
    private int   MultiShot;                  // Number Bullets Shot at the Same Time                      |  0 = None
    private float MultiShotInterval;          // Time Between Each MultiShot Bullet                        |  0 = Instant
    private float Knockback;                  // The Force Applied to the Target from 1 Bullet             |  0 = None
    private int   RicochetCount;              // The Number of Times it Bounces before Destroying          |  0 = None
    private float RicochetMultiplier = 1;     // The Damage Multiplier Per Ricochet                        |  0 = None
    private int   PierceCount;                // The Amount of Targets it Pierces Through                  |  0 = None
    private float BulletTrailSpeed = 400f;    // The time it takes for the Bullet to Reach the Target      |  0 = Instant

    [Header("Explosion Properties")]
    private float ExplosionSize;              // The Size of the Explosion
    private float ExplosionDamage;            // The Max Damage Recieved from being in the Explosion
    private float ExplosionKnockback;         // The Force Applied to the Target away from the Explosion

    [Header("Info")]
    public float _age;
    public float finalDamage;
    public int   ricoRemaining;
    public int   pierceRemaining;


    public IEnumerator SpawnHitscanBullet(Vector3 HitPoint, Vector3 shootVector, RaycastHit hit, bool RayHit, float finalDistance)
    {
        Vector3 startPosition = transform.position;

        float distance = Vector3.Distance(transform.position, HitPoint);
        finalDistance += distance;
        float startingDistance = distance;
        
        if(BulletTrailSpeed != 0)
        {
            while(distance > 0)
            {
                transform.position = Vector3.Lerp(startPosition, HitPoint, 1 - (distance / startingDistance));
                distance -= Time.deltaTime * BulletTrailSpeed;
                
                yield return null;
            }
        }
        transform.position = HitPoint;

        if(RayHit)
        {
            // Hitscan Damage Distance Falloff
            if (DamageFalloff != 0)
            {
                float falloffFactor = Mathf.Clamp01(Mathf.SmoothStep(0, 1, 1 - (finalDistance / DamageFalloff/100)));
                finalDamage = Mathf.Lerp(MinDamage, Damage, falloffFactor);
            }

            // Knockback
            if (hit.rigidbody != null) hit.rigidbody.AddForce(-hit.normal * Knockback * 10);

            // Hit Enemy
            TargetPoint targetPoint = hit.transform.GetComponent<TargetPoint>();
            if(targetPoint != null)
            {
                //Unique
                if (!targetsHit.Contains(targetPoint.target))
                {
                    pierceRemaining--;
                    targetsHit.Add(targetPoint.target);
                    targetPoint.OnHit(finalDamage, HitPoint);
                }
                if(pierceRemaining > 0) Pierce(HitPoint, shootVector, finalDistance);
                else
                {
                    if(ricoRemaining > 0 && RicoOnHit)
                    {
                        yield return new WaitForSeconds(0.05f);
                        finalDamage *= RicochetMultiplier;
                        Ricochet(HitPoint, shootVector, hit, finalDistance);
                    }
                    else transform.parent = hit.transform;
                }
            }
            else
            {
                // Ricochet
                if(ricoRemaining > 0)
                {
                    yield return new WaitForSeconds(0.05f);

                    finalDamage *= RicochetMultiplier;
                    Ricochet(HitPoint, shootVector, hit, finalDistance);
                }
                else transform.parent = hit.transform;
            }

            
            if(DestroyOnImpact) DestroyBullet(0.1f);
            if(hit.collider.gameObject.layer == 3) Destroy(gameObject);
        }
        else Destroy(gameObject);
    }


    void Ricochet(Vector3 HitPoint, Vector3 shootVector, RaycastHit hit, float finalDistance)
    {
        Vector3 ricochetDirection = Vector3.Reflect(shootVector, hit.normal);
        if(Physics.Raycast(HitPoint, ricochetDirection, out RaycastHit ricoHit, 100000, layerMask))
        {
            ricoRemaining--;
            StartCoroutine(SpawnHitscanBullet(ricoHit.point, ricochetDirection, ricoHit, true, finalDistance));
        }
        else StartCoroutine(SpawnHitscanBullet(HitPoint + ricochetDirection * 100, ricochetDirection, ricoHit, false, finalDistance));
    }

    void Pierce(Vector3 HitPoint, Vector3 shootVector, float finalDistance)
    {
        Vector3 rayStartPoint = HitPoint + shootVector *0.1f;
        if(Physics.Raycast(rayStartPoint, shootVector, out RaycastHit PierceHit, 100000, layerMask))
        {
            finalDamage = Damage;
            StartCoroutine(SpawnHitscanBullet(PierceHit.point, shootVector, PierceHit, true, finalDistance));
        }
        else StartCoroutine(SpawnHitscanBullet(shootVector * 1000, shootVector, PierceHit, false, finalDistance));
    }


    public void DestroyBullet(float Delay)
    {
        if(!ExploadOnDestroy) Destroy(gameObject, Delay);
        else Explode();
    }

    public void Explode()
    {
        Debug.Log("Boom");
        Destroy(gameObject);
    }




    public void AssignOrigin(Gun gun)
    {
        OriginGun = gun;

        //Spaghetti Ass Code Lmao
        layerMask = gun.layerMask;

        Automatic           = gun.Automatic;
        DestroyOnImpact     = gun.DestroyOnImpact;
        ExploadOnDestroy    = gun.ExploadOnDestroy;
        RicoOnHit           = gun.RicoOnHit;
        CanParryBullet      = gun.CanParryBullet;
        IgniteEnemies       = gun.IgniteEnemies;
        SelfDamage          = gun.SelfDamage;

        Damage              = gun.Damage;
        MinDamage           = gun.MinDamage;
        MultiShot           = gun.MultiShot;
        MultiShotInterval   = gun.MultiShotInterval;
        Knockback           = gun.Knockback;
        DamageFalloff       = gun.DamageFalloff;
        RicochetCount       = gun.RicochetCount;
        RicochetMultiplier  = gun.RicochetMultiplier;
        PierceCount         = gun.PierceCount;

        ExplosionSize       = gun.ExplosionSize;
        ExplosionDamage     = gun.ExplosionDamage;
        ExplosionKnockback  = gun.ExplosionKnockback;

        if(MultiShot > 1) 
        {
            Damage /= MultiShot;
            Knockback /= MultiShot;
        }
        finalDamage = Damage;

        ricoRemaining = RicochetCount;
        pierceRemaining = PierceCount+1;
    }







}
