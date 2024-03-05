using UnityEngine;

public class WallCheck : MonoBehaviour
{
    private PlayerMovement playerMovement;
    private PlayerSFX      playerSFX;
    private Timers         timers;
 
    public Collision WallCollision;
    public Collider SlipperyWalls;
    public bool AgainstWall;

    private float againstWallTimer;

    public int WallJumpsLeft = 3;

    void Awake()
    {
        //Assign Components
        playerMovement = GetComponentInParent<PlayerMovement>();
        timers         = GetComponentInParent<Timers>();
        playerSFX      = GetComponentInParent<PlayerSFX>();
    }

    void FixedUpdate()
    {
        if(AgainstWall && SlipperyWalls.material.dynamicFriction > 0)
        {
            SlipperyWalls.material.dynamicFriction -= Time.deltaTime/1.8f;
        }
        if(SlipperyWalls.material.dynamicFriction < 0.2)
        {
            SlipperyWalls.material.dynamicFriction = 0;
        }

        if(againstWallTimer > 0) againstWallTimer -= Time.deltaTime;
        if(againstWallTimer < 0)
        {
            againstWallTimer = 0;
            AgainstWall = false;
            playerMovement.SetAgainstWall(false);
        }
    }

    public void WallState(bool state)
    {
        if(state)
        {
            AgainstWall = true;
            playerMovement.LongJumping = false;
            playerMovement.SlideJumping = false;

            if(playerMovement.FastFalling) playerMovement.MaxSpeed = playerMovement._maxSpeed;
            if(!playerMovement.Sliding && !playerMovement.Dashing && !playerMovement.LongJumping)
            {
                playerMovement.MaxSpeed = playerMovement._maxSpeed;
            }
        }
        else timers.CoyoteTime = 0.3f;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject == playerMovement.gameObject) return;
        playerMovement.SetAgainstWall(true);
        WallCollision = collision;
        AgainstWall = true;

        if(WallJumpsLeft > 0 && !playerMovement.Grounded) SlipperyWalls.material.dynamicFriction = 0.6f;

        timers.SlideJumpStorage = 0.4f;

        if(timers.JumpBuffer  > 0) playerMovement.Jump();
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject == playerMovement.gameObject) return;
        
        againstWallTimer = 0.2f;
    }

    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject == playerMovement.gameObject) return;
        playerMovement.SetAgainstWall(true);
        WallCollision = collision;
        AgainstWall = true;
    }
}