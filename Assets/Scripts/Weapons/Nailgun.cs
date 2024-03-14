using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Nailgun : Gun
{
    [Header("Unique Properties")]
    public float AltCharge = 0;


    public override void FixedUpdate()
    {
        base.FixedUpdate();
        
        if(playerMovement.WalkingCheck()) TargetAnimatorBlendTree = 1;
        else TargetAnimatorBlendTree = 0;
        animator.SetFloat("Blend", TargetAnimatorBlendTree, 0.1f, Time.deltaTime);

        if(HoldingShoot && !HoldingAltShoot) AltCharge += Time.deltaTime;
        else AltCharge -= Time.deltaTime;

        if(HoldingAltShoot) AltCharge -= Time.deltaTime/2;

        if(AltCharge > 3) AltCharge = 3;
        if(AltCharge < 0) AltCharge = 0;
    }

    public override void ShootStart()
    {
        // Checks
        HoldingShoot = true;
        if (!CanShoot || AttackCooldown || MultiShotCooldown) return;

        animator.SetBool("Shooting", true);

        base.ShootStart();
    }

    public override void ExtraShootFunctions()
    {
        if(HoldingAltShoot && AltCharge > 0.1f)
        {
            playerSFX.PlayRandomSound(playerSFX.Nailgun_Shoot, 1, 0.75f, 0.1f, false);
            cameraFX.CameraShake(3f+AltCharge/2, 0.45f, 0.35f);
            animator.SetBool("Charge", true);
            Damage = 2.5f;
            MultiShot = 5;
            Spread = 25;
            AttackSpeed = 0;
        }
        else
        {
            playerSFX.PlayRandomSound(playerSFX.Nailgun_Shoot, 1, 2f, 0.25f, false);
            cameraFX.CameraShake(1.5f, 0.45f, 0.35f);
            animator.SetBool("Charge", false);
            Damage = 1f;
            MultiShot = 0;
            Spread = 15;
            AttackSpeed = 0.03f;
        }
    }

    public override void ShootEnd()
    {
        base.ShootEnd();

        animator.SetBool("Shooting", false);
        HoldingAltShoot = false;
    }

    public override void AltShootStart()
    {
        base.AltShootStart();

        if(AltCharge <= 0) return;

        animator.SetBool("Charge", true);
    }

    public override void AltShootEnd()
    {
        HoldingAltShoot = false;

        animator.SetBool("Charge", false);
    }
}
