using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSFX : MonoBehaviour
{
    public PlayerMovement playerController;
    public GroundCheck groundCheck; 

    public int VolumeDivide = 60;
    public float FallingVelocity;

    [Header("Physics Sounds")]
    public AudioSource Roll;
    public AudioSource JumpAudio;
    public AudioSource LandAudio;

    [Header("Death")]
    public AudioSource Death;


    void FixedUpdate()
    {
        if(playerController.Grounded)
        {
            Roll.volume = playerController.rb.velocity.magnitude/100;
        }
        else
        {
            LandAudio.volume = playerController.rb.velocity.y*-1 / VolumeDivide;
            Roll.volume = 0;
        }
    }
}
