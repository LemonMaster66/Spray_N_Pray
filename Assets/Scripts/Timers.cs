using System;
using System.Data.SqlTypes;
using UnityEngine;

public class Timers : MonoBehaviour
{
    private PlayerMovement playerMovement;

    [Header("Jump Chaining")]
    public float BeegJump;
    public float BeegJumpStorageTime;
    public bool  BeegJumpStorage;

    private void BeegJumpStorageFunction()
    {
        BeegJumpStorageTime -= Time.deltaTime;
        if(BeegJumpStorageTime <= 0f)
        {
            BeegJumpStorageTime = 0;
            BeegJumpStorage = false;
        }
        return;
    }


    void FixedUpdate()
    {
        if(BeegJumpStorage) BeegJumpStorageFunction();

        if(playerMovement.FastFalling)                                       BeegJump += 0.3f;
        else if(playerMovement.Grounded && BeegJump > 0 && !BeegJumpStorage) BeegJump -= 1f;

        BeegJump = (float)Math.Round(BeegJump, 2);
        BeegJumpStorageTime = (float)Math.Round(BeegJumpStorageTime, 2);

        if(BeegJump <= 0f) BeegJump = 0;
    }

    void Awake()
    {
        //Assign Components
        playerMovement = GetComponent<PlayerMovement>();
    }
}