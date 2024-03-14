using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shotgun : Gun
{
    [Header("Unique Properties")]
    public float AltCharge = 0;
    
    private int _multishot;

    public override void Awake()
    {
        base.Awake();
        _multishot = MultiShot;
    }

    public override void FixedUpdate()
    {
        base.FixedUpdate();
        
        if(playerMovement.WalkingCheck()) TargetAnimatorBlendTree = 1;
        else TargetAnimatorBlendTree = 0;
        animator.SetFloat("Blend", TargetAnimatorBlendTree, 0.1f, Time.deltaTime);
    }

    public override void ShootStart()
    {
        // Checks
        HoldingShoot = true;
        if (!CanShoot || AttackCooldown || MultiShotCooldown) return;

        animator.Play("Shotgun_Shoot", 0, 0.1f);
        playerSFX.PlaySound(playerSFX.Shotgun_Shoot, 1, 0.6f, 0.1f, false);
        cameraFX.CameraShake(4.5f+AltCharge*7, 0.45f, 0.35f);

        Charge(AltCharge);
        AltCharge = 0;

        base.ShootStart();

        
    }

    public override void AltShootStart()
    {
        base.AltShootStart();
        if (!CanShoot || AttackCooldown || MultiShotCooldown) return;

        if(AltCharge <= 3)
        {
            AltCharge++;
            Charge(AltCharge);
        }

        animator.Play("Shotgun_Rack", 0, 0.1f);
        playerSFX.PlaySound(playerSFX.Shotgun_Rack, 1, 0.9f, 0, false);
        playerSFX.PlaySound(playerSFX.Shotgun_Charge, 1+(AltCharge/5), 0.5f, 0, false);
    }

    void Charge(float ChargeCount)
    {
        if(ChargeCount == 0)
        {
            Damage = _damage;
            MultiShot = _multishot;
            Spread = 30;
            Knockback = 30;
        }
        if(ChargeCount == 1)
        {
            Damage = 40;
            MultiShot = 16;
            Spread = 45;
            Knockback = 45;
        }
        if(ChargeCount == 2)
        {
            Damage = 60;
            MultiShot = 24;
            Spread = 50;
            Knockback = 60;
        }
        if(ChargeCount >= 3)
        {
            Damage = 80;
            MultiShot = 30;
            Spread = 60;
            Knockback = 150;
        }
    }
}
