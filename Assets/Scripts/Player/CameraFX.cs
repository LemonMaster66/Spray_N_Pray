using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class CameraFX : MonoBehaviour
{
    public CinemachineVirtualCamera CMvc;
    private PlayerMovement playerMovement;
    [Space(10)]

    public float TargetDutch;
    private float BlendDutch;

    public float TargetFOV;
    private float BlendFOV;
    
    void Awake()
    {
        //Assign Components
        playerMovement = FindAnyObjectByType<PlayerMovement>();
    }


    void Update()
    {
        CMvc.m_Lens.Dutch       = Mathf.SmoothDamp(CMvc.m_Lens.Dutch, TargetDutch, ref BlendDutch, 0.1f);
        CMvc.m_Lens.FieldOfView = Mathf.SmoothDamp(CMvc.m_Lens.FieldOfView, TargetFOV, ref BlendFOV, 0.3f);

        TargetDutch = playerMovement.MovementX*-1.5f;
        TargetFOV = 65 + (playerMovement.ForwardVelocityMagnitude/2);
    }
}
