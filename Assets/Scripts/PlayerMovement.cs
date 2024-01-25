using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
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
    [HideInInspector]  public float   _speed;

    #region Script / Component Reference
        [HideInInspector] public Rigidbody  rb;
        [HideInInspector] public Transform  Camera;
        private Timers timers;
    #endregion


    void Awake()
    {
        //Assign Components
        rb      = GetComponent<Rigidbody>();
        timers  = GetComponent<Timers>();
        Camera  = GameObject.Find("Main Camera").transform;

        //Component Values
        rb.useGravity = false;

        //Property Values
        Health    = MaxHealth;
        _maxSpeed = MaxSpeed;
        _speed = Speed;
    }

    void FixedUpdate()
    {
        #region Physics Stuff
            //Gravity
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
        //**********************************
        #region Conditions
            if(Grounded) //Grounded
            {
                Speed = _speed;
                if(!Sliding)
                {
                    MaxSpeed = _maxSpeed;
                }
            }
            //Not Grounded
            else if(!Grounded) Speed = 60;

            //Sliding
            if(Sliding)
            {
                rb.velocity += Movement *8;
            }
        #endregion
        //**********************************

        if(!Sliding && !FastFalling)
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
            rb.AddForce(Vector3.up * (JumpForce + timers.BeegJump), ForceMode.VelocityChange);
            if(Sliding) MaxSpeed += 20;
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
        //Start Pressing Ctrl
        if(context.started && !Dead)
        {
            HoldingCrouch = true;

            //Fast Fall
            if(!Grounded && !FastFalling)
            {
                FastFalling = true;
                timers.BeegJumpStorage = true;
                timers.BeegJumpStorageTime = 0.3f;

                rb.velocity = new Vector3(0, rb.velocity.y, 0);
                rb.AddForce(Vector3.down * 85, ForceMode.VelocityChange);
            }
            //Slide
            else if(Grounded) SlideState(true);
        }
        //Stop Pressing Ctrl
        if(context.canceled && !Dead)
        {
            HoldingCrouch = false;
            if(Sliding) SlideState(false);
        }
    }

    public void SetGrounded(bool state) 
    {
        Grounded = state;
    }

    public void SlideState(bool state)
    {
        if(state)
        {
            Sliding = true;
            MaxSpeed = 30;
            GetComponent<Collider>().material.dynamicFriction = 0;

            transform.localScale += new Vector3(0, -1, 0);
            rb.position += new Vector3(0,-1,0);

            if(MovementX == 0 && MovementY == 0)
            {
                Movement = CamF;
            }
        }
        else
        {
            Sliding = false;
            GetComponent<Collider>().material.dynamicFriction = 9;

            transform.localScale += new Vector3(0,1,0);
            rb.position += new Vector3(0,1,0);
        }
    }

    //***********************************************************************
    //***********************************************************************
    //Other Functions

}
