using System.Collections;
using System.Collections.Generic;
using UnityEditor.Callbacks;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    private Gun OriginGun;
    private Rigidbody rb;

    [Header("Types")]
    private bool Sticky            = false;   // Bullets get Stuck to whatever they Collide with
    private bool DestroyOnImpact   = false;   // Bullets get Destroyed when Colliding
    private bool ExploadOnImpact   = false;   // Bullets Expload when Colliding
    private bool RicoOnTargetHit   = false;   // Bullets can Ricochet off of enemies
    private bool ExplodeAfterTime  = false;   // Bullet will auto Explode after an amount of time
    private bool IgniteEnemies     = false;   // Sets Enemies on Fire
    private bool SelfDamage        = false;   // Can Deal Damage to Yourself

    [Header("Properties")]
    private float Damage;                     // Damage on Hit
    private float MinDamage;                  // The Damage Applied at Max Falloff Distance
    private float Knockback;                  // The Force Applied to the Target from 1 Bullet
    private float FalloffTime;                // The Time it takes to go from Damage to MinDamage
    private int   RicochetCount;              // The Number of Times it Bounces before Destroying
    private float RicochetMultiplier;         // The Damage Multiplier Per Ricochet
    private int   PenetrateCount;             // The Amount of Targets it Pierces Through
    private float Gravity;                    // The Gravity Force of the Bullet
    private float LifeSpan;                   // The Time it takes for the Projectile Dies

    [Header("Explosion Properties")]
    private float ExplosionSize;              // The Size of the Explosion
    private float SplashDamage;               // The Max Damage Recieved from being in the Explosion
    private float ExplosionKnockback;         // The Force Applied to the Target away from the Explosion

    [Header("States")]
    private bool Impacted = false;

    [Header("Info")]
    public float _age;
    public float finalDamage;


    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        if(!Impacted) rb.velocity += Vector3.down * Gravity/150;

        if(FalloffTime > 0 && !Impacted)
        {
            _age += Time.deltaTime;  // Increment age each fixed frame

            float falloffFactor = Mathf.Clamp01(Mathf.SmoothStep(0, 1, _age / FalloffTime));
            finalDamage = Mathf.Lerp(Damage, MinDamage, falloffFactor);
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        // Knockback
        if (collision.rigidbody != null) collision.rigidbody.velocity += (collision.relativeVelocity.normalized*-1 * Knockback)/3;

        TargetPoint targetPoint = collision.gameObject.GetComponent<TargetPoint>();
        if(targetPoint != null) targetPoint.OnHit(finalDamage, transform.position);

        if(RicochetCount < 1)
        {
            if(targetPoint != null || Sticky)
            {
                Impacted = true;

                rb.collisionDetectionMode = CollisionDetectionMode.Discrete;
                rb.isKinematic = true;
                transform.parent = collision.transform;
                GetComponent<Collider>().enabled = false;
            }
        }

        if(DestroyOnImpact) Destroy(gameObject, 0.4f);
    }




    public void AssignOrigin(Gun gun)
    {
        OriginGun = gun;

        //Spaghetti Ass Code Lmao
        Sticky           = gun.ProjectileSticky;
        DestroyOnImpact  = gun.DestroyOnImpact;
        ExploadOnImpact  = gun.ExploadOnImpact;
        RicoOnTargetHit  = gun.RicoOnTargetHit;
        ExplodeAfterTime = gun.ExplodeAfterTime;
        IgniteEnemies    = gun.IgniteEnemies;
        SelfDamage       = gun.SelfDamage;

        Damage             = gun.Damage;
        MinDamage          = gun.MinDamage;
        Knockback          = gun.Knockback;
        FalloffTime        = gun.FalloffTime;
        RicochetCount      = gun.RicochetCount;
        RicochetMultiplier = gun.RicochetMultiplier;
        PenetrateCount     = gun.PenetrateCount;
        Gravity            = gun.ProjectileGravity;
        LifeSpan           = gun.ProjectileLifeSpan;

        ExplosionSize      = gun.ExplosionSize;
        SplashDamage       = gun.SplashDamage;
        ExplosionKnockback = gun.ExplosionKnockback;

        finalDamage = Damage;
        if(LifeSpan > 0) Destroy(gameObject, LifeSpan);
    }
}
