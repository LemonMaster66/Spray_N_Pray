using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSFX : MonoBehaviour
{
    public GameObject AudioPrefab;
    [Space(8)]
    public float TimeBetweenSteps;

    [Header("Physics Sounds")]
    public AudioClip[] Step;
    public AudioClip Jump;
    public AudioClip[] Land;
    public AudioClip Slam;
    public AudioClip Dash;
    public AudioClip LongJump;
    public AudioClip Slide;
    public AudioClip SlideJump;
    public AudioClip FastFalling;

    [Header("Info")]
    public float stepTimer;


    private PlayerMovement playerController;
    private GroundCheck groundCheck;
    private WallCheck wallCheck;


    void Awake()
    {
        playerController = FindAnyObjectByType<PlayerMovement>();
        groundCheck = GetComponentInChildren<GroundCheck>();
        wallCheck   = GetComponentInChildren<WallCheck>();
    }


    void FixedUpdate()
    {
        if(playerController.MovementX != 0 || playerController.MovementY != 0)
        {
            if(playerController.Grounded) stepTimer -= Time.deltaTime;
        }
        if(stepTimer < 0)
        {
            PlayRandomSound(Step, 1, 1, 0.2f);
            stepTimer = TimeBetweenSteps;
        }

        stepTimer = (float)Math.Round(stepTimer, 2);
    }



    public void PlaySound(AudioClip audioClip, float Pitch, float Volume, float PitchVariation)
    {
        GameObject AudioObj = Instantiate(AudioPrefab, playerController.transform, this.transform);
        AudioSource audioSource = AudioObj.GetComponent<AudioSource>();

        audioSource.clip   =  audioClip;
        audioSource.volume =  Volume;
        audioSource.pitch  =  Pitch + UnityEngine.Random.Range(-PitchVariation, PitchVariation);

        audioSource.Play();
        Destroy(AudioObj, audioClip.length);
    }


    public void PlayRandomSound(AudioClip[] audioClip, float Pitch, float Volume, float PitchVariation)
    {
        GameObject AudioObj = Instantiate(AudioPrefab, playerController.transform.position, Quaternion.identity, this.transform);
        AudioSource audioSource = AudioObj.GetComponent<AudioSource>();

        AudioClip RandomClip = audioClip[UnityEngine.Random.Range(0, audioClip.Length-1)];
        audioSource.clip     = RandomClip;
        audioSource.volume   = Volume;
        audioSource.pitch    = Pitch + UnityEngine.Random.Range(-PitchVariation, PitchVariation);

        audioSource.Play();
        Destroy(AudioObj, RandomClip.length);
    }
}

