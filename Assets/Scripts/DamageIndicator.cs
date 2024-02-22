using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DamageIndicator : MonoBehaviour
{
    public bool Decay;
    public bool BillboardRotate;

    public float Damage;
    public float DecayAfter;
    public float Speed = 12;
    public float Age;

    public Gradient gradient;
    private TextMeshPro _TMPro;
    private Transform cam;
    private Animator animator;

    void Awake()
    {
        _TMPro   = GetComponent<TextMeshPro>();
        animator = GetComponent<Animator>();
        cam      = Camera.main.transform;

        Vector3 randomDirection = UnityEngine.Random.insideUnitSphere;
        GetComponent<Rigidbody>().velocity = randomDirection * Speed;
    }

    void LateUpdate()
    {
        if(Decay)
        {
            Age += Time.deltaTime;
            if(Age >= DecayAfter) animator.Play("Fade");
        }

        if(BillboardRotate) transform.LookAt(transform.position + cam.forward);
    }


    public void DamageUpdate(float UpdatedDamage)
    {
        Damage = UpdatedDamage;
        Damage = (float)Math.Round(Damage, 2);

        _TMPro.text = Damage + "";
        Recolour();

        if(Age != 0 && Decay == true) animator.Play("Tick");
        gameObject.name = Damage + "";
    }

    public void Recolour()
    {
        _TMPro.color = gradient.Evaluate(Damage/100);
    }

    public void DestroyText()
    {
        Destroy(gameObject);
    }
}
