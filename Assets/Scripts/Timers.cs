using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Timers : MonoBehaviour
{
    private PlayerMovement playerMovement;

    public float CoyoteTime;

    // private void CoyoteTimeFunction()
    // {
    //     CoyoteTime -= Time.deltaTime;
    //     if(CoyoteTime <= 0f) CoyoteTime = 0;
    //     return;
    // }


    void FixedUpdate()
    {
        //if(CoyoteTime > 0) CoyoteTimeFunction();
    }

    void Awake()
    {
        //Assign Components
        playerMovement = GetComponent<PlayerMovement>();
    }
}