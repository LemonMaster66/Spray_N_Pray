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
        cameraFX.CameraShake(4.5f+AltCharge*2, 0.45f, 0.35f);

        base.ShootStart();

        AltCharge = 0;
        Charge(AltCharge);
    }

    public override void AltShootStart()
    {
        base.AltShootStart();

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
        }
        if(ChargeCount == 1)
        {
            Damage = 40;
            MultiShot = 16;
        }
        if(ChargeCount == 2)
        {
            Damage = 60;
            MultiShot = 24;
        }
        if(ChargeCount >= 3)
        {
            Damage = 60;
            MultiShot = 24;
            //Explode
        }
    }
}
