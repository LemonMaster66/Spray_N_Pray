using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Gun : MonoBehaviour
{
    [Header("References")]
    public GameObject BulletPrefab;

    [Header("Types")]
    public bool Projectile;           // If the fired Bullet is a Projectile
    public bool Automatic;            // Hold Down the Mouse to Continue Firing
    public bool DestroyOnImpact;      // Bullets get Destroyed when Colliding
    public bool ExploadOnImpact;      // Bullets Expload when Colliding
    public bool CanParryBullet;       // Punch after Shooting to Parry Your Own Bullets
    public bool DamageFallOff;        // The Distance before the Bullets Lose All Damage

    [Header("Properties")]
    public float Damage;               // Damage on Hit
    public float Ammo;                 // Bullets Shot Before Reloading
    public float AttackSpeed;          // Time Between Shots
    public float ReloadSpeed;          // Time Taken to Reload
    public float MultiShot;            // Number Bullets Shot at the Same Time
    public float MultiShotInterval;    // Time Between Each MultiShot Bullet
    public float Spread;               // The Amount that Each Bullet Strays from the Target
    public float Knockback;            // The Force Applied to the Target from 1 Bullet

    [Header("Hitscan Properties")]
    public float Ricochete;            // The Number of Times it Bounces before Destroying
    public float MaxFalloffDist;       // The Distance it takes to Lose All Damage
    public int   Penetration;          // The Amount of Targets it Pierces Through

    [Header("Projectile Properties")]
    public float ProjectileSpeed;      // The Speed of the Bullet
    public float ProjectileBounce;     // The Bounciness Value of the Bullet
    public float ProjectileGravity;    // The Gravity Force of the Bullet
    public float ProjectileLifeSpan;   // The Time it takes for the Projectile Dies

    [Header("Explosion Properties")]
    public float ExplosionSize;        // The Size of the Explosion
    public float SplashDamage;         // The Max Damage Recieved from being in the Explosion Radius
    public float ExplosionKnockback;   // The Force Applied to the Target away from the Explosion

    [Header("States")]
    public bool CanShoot;
    public bool Reloading;

    #region Debug Values
        private GameObject GunTip;
        [HideInInspector] public float _damage;
        [HideInInspector] public float _ammo;
    #endregion




    void Awake()
    {
        GunTip = gameObject.transform.GetChild(0).gameObject;
        Debug.Log(GunTip);
    }


    public void OnFire(InputAction.CallbackContext context)
    {
        if(context.started)
        {
            Debug.Log("Fire");
        }
        else if(context.canceled)
        {

        }
    }

    public void OnAltFire(InputAction.CallbackContext context)
    {
        if(context.started)
        {
            Debug.Log("AltFire");
        }
        else if(context.canceled)
        {

        }
    }
}