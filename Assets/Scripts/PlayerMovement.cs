using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Physics")]
    public float Speed      = 50;
    public float MaxSpeed   = 80;
    public float JumpForce  = 8;
    public float Gravity    = 100;

    [Header("Properties")]
    public int  Health;
    private int MaxHealth = 100;

    [Header("States")]
    public bool Grounded     = true;
    public bool Dead         = false;
    public bool Dashing      = false;
    public bool Sliding      = false;
    public bool FastFalling  = false;

    #region Debug States
        [HideInInspector] public bool HoldingCrouch;
    #endregion

    [Header("Debug Stats")]
    public Vector3     PlayerVelocity;
    private float      ForwardVelocityMagnitude;
    public float       VelocityMagnitudeXZ;
    [HideInInspector]  public Vector3 CamF;
    [HideInInspector]  public Vector3 CamR;
    [HideInInspector]  public Vector3 Movement;
    [HideInInspector]  public float   MovementX;
    [HideInInspector]  public float   MovementY;
    [HideInInspector]  public float   _maxSpeed;

    #region Script / Component Reference
        [HideInInspector] public Rigidbody  rb;
        [HideInInspector] public Transform  Camera;
    #endregion


    void Awake()
    {
        //Assign Components
        rb      = GetComponent<Rigidbody>();
        Camera  = GameObject.Find("Main Camera").transform;

        //Component Values
        rb.useGravity = false;

        //Property Values
        Health    = MaxHealth;
        _maxSpeed = MaxSpeed;
    }

    void FixedUpdate()
    {
        #region Physics Stuff
            //Extra Gravity
            rb.AddForce(Physics.gravity * Gravity /10);

            //max speed
            if (rb.velocity.magnitude > MaxSpeed)
            {
                // Get the velocity direction
                Vector3 newVelocity = rb.velocity;
                newVelocity.y = 0f;
                newVelocity = Vector3.ClampMagnitude(newVelocity, MaxSpeed);
                newVelocity.y = rb.velocity.y;
                rb.velocity = newVelocity;
            }

            if(Grounded) Speed = 400;
            else         Speed = 60;
        #endregion
        //**********************************
        #region PerFrame Calculations
            CamF = Camera.forward;
            CamR = Camera.right;
            CamF.y = 0;
            CamR.y = 0;
            CamF = CamF.normalized;
            CamR = CamR.normalized;

            // Calculate the Forward velocity magnitude
            Vector3 ForwardVelocity = Vector3.Project(rb.velocity, CamF);
            ForwardVelocityMagnitude = ForwardVelocity.magnitude;
            ForwardVelocityMagnitude = (float)Math.Round(ForwardVelocityMagnitude, 2);

            VelocityMagnitudeXZ = new Vector3(rb.velocity.x, 0, rb.velocity.z).magnitude;
        #endregion

        if(Sliding)
        
        if(!Sliding && !Dead)
        {
            Movement = (CamF * MovementY + CamR * MovementX).normalized;
            rb.AddForce(Movement * Speed);
        }

        #region Debug Stats
            PlayerVelocity      = rb.velocity;
            PlayerVelocity.x    = (float)Math.Round(PlayerVelocity.x, 2);
            PlayerVelocity.y    = (float)Math.Round(PlayerVelocity.y, 2);
            PlayerVelocity.z    = (float)Math.Round(PlayerVelocity.z, 2);
            VelocityMagnitudeXZ = (float)Math.Round(VelocityMagnitudeXZ, 2);
        #endregion
    }

    //***********************************************************************
    //***********************************************************************
    //Movement Functions
    public void OnMove(InputAction.CallbackContext MovementValue)
    {  
        Vector2 inputVector = MovementValue.ReadValue<Vector2>();
        MovementX = inputVector.x;
        MovementY = inputVector.y;
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if(context.started && Grounded && !Dead)
        {
            rb.AddForce(Vector3.up * JumpForce, ForceMode.VelocityChange);
        }
    }

    public void OnDash(InputAction.CallbackContext context)
    {
        if(context.started && !Dead)
        {
            if(Dead) return;

            //Debug.Log("Dash");
        }
    }

    public void OnCrouch(InputAction.CallbackContext context)
    {
        if(context.started && !Dead)
        {
            HoldingCrouch = true;

            if(!Grounded && !FastFalling)
            {
                FastFalling = true;

                MaxSpeed = 0;

                rb.AddForce(Vector3.down * 85, ForceMode.VelocityChange);
            }
            else if(Grounded) Sliding = true;
        }
        if(context.canceled)
        {
            HoldingCrouch = false;
            Sliding = false;
        }
    }

    public void SetGrounded(bool state) 
    {
        Grounded = state;
    }

    //***********************************************************************
    //***********************************************************************
    //Other Functions

}
