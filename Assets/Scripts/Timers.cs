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

    [Header("Dash")]
    public float DashTime;

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
    private void DashFunction()
    {
        DashTime -= Time.deltaTime;
        if(DashTime <= 0.001f)
        {
            DashTime = 0;
            playerMovement.EndDash();
        }
        return;
    }


    void FixedUpdate()
    {
        //Auto Countdown
        if(BeegJumpStorage) BeegJumpStorageFunction();
        if(DashTime > 0) DashFunction();

        #region Fast Fall Logic
            if(playerMovement.FastFalling)                                       BeegJump += 0.3f;
            else if(playerMovement.Grounded && BeegJump > 0 && !BeegJumpStorage) BeegJump -= 1f;
            if(BeegJump <= 0f) BeegJump = 0;
        #endregion

        BeegJump = (float)Math.Round(BeegJump, 2);
        BeegJumpStorageTime = (float)Math.Round(BeegJumpStorageTime, 2);

        DashTime = (float)Math.Round(DashTime, 2);
    }

    void Awake()
    {
        //Assign Components
        playerMovement = GetComponent<PlayerMovement>();
    }
}