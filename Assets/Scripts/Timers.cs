using System;
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

    [Header("Slide")]
    public float SlideJumpPower = 0;
    public float SlideJumpStorage = 0;

    [Header("Quality of Life")]
    public float CoyoteTime;
    public float JumpBuffer;
    public float SlideBuffer;

    private void BeegJumpStorageFunction()
    {
        BeegJumpStorageTime -= Time.deltaTime;
        if(BeegJumpStorageTime < 0)
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
    private void CoyoteTimeFunction()
    {
        CoyoteTime -= Time.deltaTime;
        if(CoyoteTime < 0) CoyoteTime = 0;
        return;
    }
    private void JumpBufferFunction()
    {
        JumpBuffer -= Time.deltaTime;
        if(JumpBuffer < 0) JumpBuffer = 0;
        return;
    }
    private void SlideBufferFunction()
    {
        SlideBuffer -= Time.deltaTime;
        if(SlideBuffer < 0) SlideBuffer = 0;
        return;
    }
    private void SlideJumpStorageFunction()
    {
        SlideJumpStorage -= Time.deltaTime;
        if(SlideJumpStorage < 0) SlideJumpStorage = 0;
        return;
    }



    void FixedUpdate()
    {
        //Auto Countdown
        if(BeegJumpStorage)       BeegJumpStorageFunction();
        if(CoyoteTime > 0)        CoyoteTimeFunction();
        if(JumpBuffer > 0)        JumpBufferFunction();
        if(SlideJumpStorage > 0)  SlideJumpStorageFunction();
        if(SlideBuffer > 0)       SlideBufferFunction();
        if(DashTime > 0)          DashFunction();

        #region Fast Fall Logic
            if(playerMovement.FastFalling) BeegJump += 0.3f;
            else if(playerMovement.Grounded && BeegJump > 0 && !BeegJumpStorage) BeegJump = 0;
            if(BeegJump <= 0f) BeegJump = 0;
        #endregion

        #region Rounding Values
            BeegJumpStorageTime = (float)Math.Round(BeegJumpStorageTime, 2);
            BeegJump            = (float)Math.Round(BeegJump, 2);
            DashTime            = (float)Math.Round(DashTime, 2);
            CoyoteTime          = (float)Math.Round(CoyoteTime, 2);
            JumpBuffer          = (float)Math.Round(JumpBuffer, 2);
            SlideBuffer         = (float)Math.Round(SlideBuffer, 2);
            SlideJumpStorage    = (float)Math.Round(SlideJumpStorage, 2);
        #endregion
    }

    void Awake()
    {
        //Assign Components
        playerMovement = GetComponent<PlayerMovement>();
    }
}