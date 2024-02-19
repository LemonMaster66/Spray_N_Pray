using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetPoint : MonoBehaviour
{
    private Target target;

    public float Multiplier = 1;

    void Awake()
    {
        target = GetComponentInParent<Target>();
    }

    public virtual void OnHit(float Damage)
    {
        float finalDamage = Damage * Multiplier;
        target.TakeDamage(finalDamage);
    }
}
