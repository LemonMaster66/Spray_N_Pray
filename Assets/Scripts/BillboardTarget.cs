using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BillboardTarget : Target
{
    private Animator animator;
    
    [Header("Custom Properties")]
    public float DeadDuration = 5;

    private float deadDurationTime;
    private bool Reviving = false;

    void Awake()
    {
        animator = GetComponent<Animator>();
        MaxHealth = Health;
    }

    public override void Update()
    {
        base.Update();

        if(deadDurationTime > 0) deadDurationTime-= Time.deltaTime;
        if(deadDurationTime <= 0.3f && !Reviving && Dead)
        {
            animator.Play("Revive", 0, 0f);
            Reviving = true;
        }
        if(deadDurationTime <= 0 && Reviving)
        {
            Reviving = false;
            Dead = false;
            Health = MaxHealth;
        }
    }

    public override void TakeDamage(float Damage)
    {
        base.TakeDamage(Damage);

        if(Dead) return;
        
        if     (TotalDamage <  MaxHealth/100*10)                                    animator.Play("HitSmall", 0, 0f);
        else if(TotalDamage >= MaxHealth/100*10  && TotalDamage < MaxHealth/100*25) animator.Play("HitMedium", 0, 0f);
        else if(TotalDamage >  MaxHealth/100*25 && TotalDamage < MaxHealth/100*50)  animator.Play("HitBig", 0, 0f);
        else if(TotalDamage >= MaxHealth/100*50)                                    animator.Play("HitOmega", 0, 0f);
    }

    public override void Die(float Damage)
    {
        Dead = true;
        deadDurationTime = DeadDuration;

        if     (Damage <  MaxHealth/100*15)                               animator.Play("KillSmall", 0, 0f);
        else if(Damage >= MaxHealth/100*15  && Damage < MaxHealth/100*45) animator.Play("KillMedium", 0, 0f);
        else if(Damage >= MaxHealth/100*45)                               animator.Play("KillBig", 0, 0f);
    }
}
