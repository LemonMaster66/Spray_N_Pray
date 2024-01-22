using UnityEngine;

public class GroundCheck : MonoBehaviour
{
    public PlayerMovement playerMovement;
    public GameObject GroundObject;
    public bool Grounded;

    public GameObject DeathScreenForSomeReason;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == playerMovement.gameObject) return;
        playerMovement.SetGrounded(true);
        GroundObject = other.gameObject;

        Grounded = true;
        if(playerMovement.FastFalling) playerMovement.MaxSpeed = playerMovement._maxSpeed;
        playerMovement.FastFalling = false;
        if(playerMovement.HoldingCrouch) playerMovement.Sliding = true;

        //playerSFX.LandAudio.Play();
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject == playerMovement.gameObject) return;
        playerMovement.SetGrounded(false);
        GroundObject = null;
        Grounded = false;
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject == playerMovement.gameObject) return;
        playerMovement.SetGrounded(true);
        GroundObject = other.gameObject;
        Grounded = true;
    }
}