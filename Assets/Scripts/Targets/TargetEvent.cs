using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetEvent : TargetPoint
{
    [Space(20)]
    [Header("Unique Properties")]
    public GameObject TargetParent;
    public Target[] Targets;

    void Awake()
    {
        Targets = TargetParent.GetComponentsInChildren<BillboardTarget>();
    }

    public override void OnHit(float Damage, Vector3 HitPoint)
    {
        foreach(BillboardTarget target in Targets)
        {
            target.Revive();
        }
    }
}
