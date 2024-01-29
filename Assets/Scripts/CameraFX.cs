using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class CameraFX : MonoBehaviour
{
    public CinemachineVirtualCamera cinemachineVirtualCamera;
    private PlayerMovement playerMovement;
    [Space(10)]
    public float TargetDutch;
    public float Blend;
    
    void Awake()
    {
        //Assign Components
        playerMovement = FindAnyObjectByType<PlayerMovement>();
    }


    void Update()
    {
        cinemachineVirtualCamera.m_Lens.Dutch = Mathf.SmoothDamp(cinemachineVirtualCamera.m_Lens.Dutch, TargetDutch, ref Blend, 0.1f);

        TargetDutch = playerMovement.MovementX*-2;
    }
}
