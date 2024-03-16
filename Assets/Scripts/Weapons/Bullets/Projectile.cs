using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    private Gun OriginGun;
    private Rigidbody rb;
    public List<Target> targetsHit = new List<Target>();

    [Header("Types")]
    private bool Sticky            = false;   // Bullets get Stuck to whatever they Collide with
    private bool DestroyOnImpact   = false;   // Bullets get Destroyed when Colliding
    private bool ExploadOnDestroy  = false;   // Bullets Expload when Colliding
    private bool RicoOnHit         = false;   // Bullets can Ricochet off of enemies
    private bool IgniteEnemies     = false;   // Sets Enemies on Fire
    private bool SelfDamage        = false;   // Can Deal Damage to Yourself

    [Header("Properties")]
    private float Damage;                     // Damage on Hit
    private float MinDamage;                  // The Damage Applied at Max Falloff Distance
    private float DamageFalloff;              // The Time it takes to go from Damage to MinDamage
    private float Knockback;                  // The Force Applied to the Target from 1 Bullet
    public  int   MultiShot;                  // Number Bullets Shot at the Same Time
    public  float MultiShotInterval;          // Time Between Each MultiShot Bullet
    private int   RicochetCount;              // The Number of Times it Bounces before Destroying
    private float RicochetMultiplier;         // The Damage Multiplier Per Ricochet
    private int   PierceCount;                // The Amount of Targets it Pierces Through
    private float Gravity;                    // The Gravity Force of the Bullet
    private float LifeSpan;                   // The Time it takes for the Projectile Dies

    [Header("Explosion Properties")]
    private float ExplosionSize;              // The Size of the Explosion
    private float ExplosionDamage;            // The Max Damage Recieved from being in the Explosion
    private float ExplosionKnockback;         // The Force Applied to the Target away from the Explosion

    [Header("States")]
    private bool Impacted = false;
    private bool Attatched = false;

    [Header("Info")]
    public float _age;
    public float finalDamage;
    public int   pierceRemaining;
    public int   ricoRemaining;
    public int   ricoCount;

    private PlayerSFX playerSFX;


    public virtual void Awake()
    {
        rb = GetComponent<Rigidbody>();
        playerSFX = FindAnyObjectByType<PlayerSFX>();
    }

    public virtual void FixedUpdate()
    {
        if(!Attatched) rb.velocity += Vector3.down * Gravity/150;

        _age += Time.deltaTime;

        if(DamageFalloff > 0 && !Impacted)
        {
            float falloffFactor = Mathf.Clamp01(Mathf.SmoothStep(0, 1, _age/2 / DamageFalloff));
            finalDamage = Mathf.Lerp(finalDamage, MinDamage, falloffFactor);
        }
        if(_age > LifeSpan && LifeSpan != 0) DestroyProjectile();
    }


    public virtual void OnCollisionEnter(Collision collision)
    {
        // Projectile Knockback
        if (collision.rigidbody != null) collision.rigidbody.velocity += (collision.relativeVelocity.normalized*-1 * Knockback)/3;

        // Hit Enemy
        TargetPoint targetPoint = collision.transform.GetComponent<TargetPoint>();
        if(targetPoint != null)
        {
            //Unique
            if (!targetsHit.Contains(targetPoint.target))
            {
                pierceRemaining--;
                targetsHit.Add(targetPoint.target);
                targetPoint.OnHit(finalDamage, transform.position);
            }
            if(pierceRemaining > 0) Pierce(collision);
            else
            {
                if(ricoRemaining > 0 && RicoOnHit)
                {
                    finalDamage *= RicochetMultiplier;
                    Ricochet(collision);
                }
                else Impact(collision);
            }
        }
        else
        {
            // Ricochet
            if(ricoRemaining > 0)
            {
                finalDamage *= RicochetMultiplier;
                Ricochet(collision);
            }
            else Impact(collision);
        }
    }

    public virtual void Ricochet(Collision collision)
    {
        rb.velocity = Vector3.Reflect(collision.relativeVelocity*-1, collision.contacts[0].normal);
        ricoRemaining--;
        finalDamage *= RicochetMultiplier;
    }

    public virtual void Pierce(Collision collision)
    {
        Vector3 StartPoint = transform.position + collision.relativeVelocity*-1 * 0.0075f;
        transform.position = StartPoint;
        rb.velocity = collision.relativeVelocity*-1f;
    }

    public virtual void Impact(Collision collision)
    {
        Impacted = true;

        if(Sticky)
        {
            Attatched = true;
            rb.collisionDetectionMode = CollisionDetectionMode.Discrete;
            rb.isKinematic = true;
            transform.parent = collision.transform;
            GetComponent<Collider>().enabled = false;
        }

        if(DestroyOnImpact) DestroyProjectile();
    }


    public virtual void DestroyProjectile()
    {
        if(!ExploadOnDestroy) Destroy(gameObject);
        else Explode();
    }

    public virtual void Explode()
    {
        Debug.Log("Boom");
        Destroy(gameObject);
    }




    public virtual void AssignOrigin(Gun gun)
    {
        OriginGun = gun;

        //Spaghetti Ass Code Lmao
        Sticky              = gun.ProjectileSticky;
        DestroyOnImpact     = gun.DestroyOnImpact;
        ExploadOnDestroy    = gun.ExploadOnDestroy;
        RicoOnHit           = gun.RicoOnHit;
        IgniteEnemies       = gun.IgniteEnemies;
        SelfDamage          = gun.SelfDamage;

        Damage              = gun.Damage;
        MinDamage           = gun.MinDamage;
        Knockback           = gun.Knockback;
        DamageFalloff       = gun.DamageFalloff;
        MultiShot           = gun.MultiShot;
        MultiShotInterval   = gun.MultiShotInterval;
        ricoRemaining       = gun.RicochetCount;
        RicochetMultiplier  = gun.RicochetMultiplier;
        PierceCount         = gun.PierceCount;
        Gravity             = gun.ProjectileGravity;
        LifeSpan            = gun.ProjectileLifeSpan;

        ExplosionSize       = gun.ExplosionSize;
        ExplosionDamage     = gun.ExplosionDamage;
        ExplosionKnockback  = gun.ExplosionKnockback;

        if(MultiShot > 1) 
        {
            Damage /= MultiShot;
            Knockback /= MultiShot;
        }
        finalDamage = Damage;
        pierceRemaining = PierceCount+1;
    }
}
