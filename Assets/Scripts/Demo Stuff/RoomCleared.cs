using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomCleared : MonoBehaviour
{
    public Animator Door;
    public Target[] targets;

    void Awake()
    {
        targets = GetComponentsInChildren<BillboardTarget>();
    }

    public void CompleteCheck()
    {
        foreach(Target target in targets)
        {
            if(!target.Dead) return;
        }
        Door.Play("DoorOpen");
        Door.gameObject.GetComponent<AudioSource>().Play();
    }
}
