using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunPickup : MonoBehaviour
{
    public int GunType;
    private GunManager gunManager;

    void Awake()
    {
        gunManager = FindAnyObjectByType<GunManager>();
    }

    void OnTriggerEnter(Collider collider)
    {
        gunManager.GunState(GunType, true);
        Destroy(gameObject);
    }
}
