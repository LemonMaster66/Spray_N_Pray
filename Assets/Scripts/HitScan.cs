using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitScan : MonoBehaviour
{
    [Header("References")]
    private Gun OriginGun;
    private LayerMask layerMask;

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
    private int   PenetrateCount;             // The Amount of Targets it Pierces Through                  |  0 = None

    [Header("Unique Properties")]
    public float BulletTrailSpeed = 400f;    // The time it takes for the Bullet to Reach the Target      |  0 = Instant

    [Header("States")]
    private bool Impacted = false;
    private bool RayHit = false;

    [Header("Info")]
    public float _age;
    public float finalDamage;
    public int   ricoRemaining;
    public int   ricoCount;


    public IEnumerator SpawnHitscanBullet(Vector3 HitPoint, Vector3 shootVector, RaycastHit hit, bool RayHit, int ricoRemaining, float finalDistance)
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
                targetPoint.OnHit(finalDamage, HitPoint);
                if(ricoRemaining > 0 && RicoOnHit) Ricochet(HitPoint, shootVector, hit, ricoRemaining, finalDistance);
                else
                {
                    transform.parent = hit.transform;
                }
            }
            else
            {
                // Ricochet
                if(ricoRemaining > 0)
                {
                    yield return new WaitForSeconds(0.05f);

                    finalDamage *= RicochetMultiplier;
                    Ricochet(HitPoint, shootVector, hit, ricoRemaining, finalDistance);
                }
                else transform.parent = hit.transform;
            }

            
            if(DestroyOnImpact) DestroyBullet(0.1f);
            if(hit.collider.gameObject.layer == 3) Destroy(gameObject);
        }
        else Destroy(gameObject);
    }


    void Ricochet(Vector3 HitPoint, Vector3 shootVector, RaycastHit hit, int ricoRemaining, float finalDistance)
    {
        Vector3 ricochetDirection = Vector3.Reflect(shootVector, hit.normal);
        if(Physics.Raycast(HitPoint, ricochetDirection, out RaycastHit ricoHit, 100000, layerMask))
        {
            StartCoroutine(SpawnHitscanBullet(ricoHit.point, ricochetDirection, ricoHit, true, ricoRemaining-1, finalDistance));
        }
        else StartCoroutine(SpawnHitscanBullet(HitPoint + ricochetDirection * 100, ricochetDirection, ricoHit, false, 0, finalDistance));
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
        PenetrateCount      = gun.PenetrateCount;

        if(MultiShot > 1) 
        {
            Damage /= MultiShot;
            Knockback /= MultiShot;
        }
        finalDamage = Damage;
    }







}
