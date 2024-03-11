using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetPoint : MonoBehaviour
{
    public Target target;

    public float Multiplier = 1;

    void Awake()
    {
        target = GetComponentInParent<Target>();
    }

    public virtual void OnHit(float Damage, Vector3 HitPoint)
    {
        float finalDamage = Damage * Multiplier;
        target.TakeDamage(finalDamage, HitPoint);
    }
}
