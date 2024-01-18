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
    private int MaxHealth = 5;

    [Header("States")]
    public bool Grounded     = true;
    public bool Dead         = false;
    public bool Dashing      = false;
    public bool Sliding      = false;
    public bool FastFalling  = false;

    [Header("Debug Stats")]
    public Vector3     PlayerVelocity;
    private float      ForwardVelocityMagnitude;
    public float       VelocityMagnitudeXZ;
    [HideInInspector]  public Vector3 CamF;
    [HideInInspector]  public Vector3 CamR;
    [HideInInspector]  public Vector3 movement;
    [HideInInspector]  public float   movementX;
    [HideInInspector]  public float   movementY;

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
        Health = MaxHealth;
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

        if(Dead) return;
        

        movement = (CamF * movementY + CamR * movementX).normalized;
        rb.AddForce(movement * Speed);

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
    public void OnMove(InputAction.CallbackContext movementValue)
    {  
        Vector2 inputVector = movementValue.ReadValue<Vector2>();
        movementX = inputVector.x;
        movementY = inputVector.y;
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
        if(context.started)
        {
            if(Dead) return;

            //Debug.Log("Dash");
        }
    }

    public void OnCrouch(InputAction.CallbackContext context)
    {
        if(context.started && !Dead)
        {
            if(Dead) return;

            rb.AddForce(Vector3.up * JumpForce, ForceMode.VelocityChange);
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
