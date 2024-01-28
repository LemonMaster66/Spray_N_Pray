using UnityEngine;

public class GroundCheck : MonoBehaviour
{
    public PlayerMovement playerMovement;
    private Timers timers;

    public GameObject GroundObject;
    public bool Grounded;

    void Awake()
    {
        //Assign Components
        timers = GetComponentInParent<Timers>();
    }

    public bool CheckGround()
    {
        if(GroundObject != null) return true;
        else return false;
    }

    public void GroundState(bool state)
    {
        if(state)
        {
            Grounded = true;
            playerMovement.HasJumped = false;
            playerMovement.FastFalling = false;
            playerMovement.LongJumping = false;

            if(playerMovement.FastFalling) playerMovement.MaxSpeed = playerMovement._maxSpeed;
            if(!playerMovement.Sliding && !playerMovement.Dashing && !playerMovement.LongJumping)
            {
                playerMovement.MaxSpeed = playerMovement._maxSpeed;
            }

            //playerSFX.LandAudio.Play();
        }
        else timers.CoyoteTime = 0.3f;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == playerMovement.gameObject) return;
        playerMovement.SetGrounded(true);
        GroundObject = other.gameObject;

        if(timers.JumpBuffer  > 0 && !playerMovement.LongJumping) playerMovement.Jump();
        if(timers.SlideBuffer > 0) playerMovement.SlideState(true);

        timers.SlideJumpStorage = 0.2f;

        GroundState(true);
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject == playerMovement.gameObject) return;
        playerMovement.SetGrounded(false);
        GroundObject = null;
        Grounded = false;

        GroundState(false);
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject == playerMovement.gameObject) return;
        playerMovement.SetGrounded(true);
        GroundObject = other.gameObject;
        Grounded = true;
    }
}