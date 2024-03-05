using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSFX : MonoBehaviour
{
    private PlayerMovement playerController;
    private GroundCheck groundCheck;
    private WallCheck wallCheck;

    public GameObject AudioPrefab;

    [Header("Physics Sounds")]
    public AudioClip Step;
    public AudioClip Jump;
    public AudioClip Land;
    public AudioClip Dash;
    public AudioClip LongJump;
    public AudioClip Slide;
    public AudioClip SlideJump;
    public AudioClip FastFalling;


    void Awake()
    {
        playerController = GetComponent<PlayerMovement>();
        groundCheck = GetComponentInChildren<GroundCheck>();
        wallCheck   = GetComponentInChildren<WallCheck>();
    }


    void FixedUpdate()
    {
        
    }



    public void PlaySound(AudioClip audioClip, float PitchVariation, float Volume = 1)
    {
        GameObject AudioObj = Instantiate(AudioPrefab, playerController.transform.position, Quaternion.identity, this.transform);
        AudioSource audioSource = AudioObj.GetComponent<AudioSource>();

        audioSource.clip   =  audioClip;
        audioSource.volume =  Volume;
        audioSource.pitch  += Random.Range(-PitchVariation, PitchVariation);

        Destroy(AudioObj, audioClip.length);
    }
    public void PlayRandomSound(AudioClip[] audioClip, float PitchVariation)
    {
        GameObject AudioObj = Instantiate(AudioPrefab, playerController.transform.position, Quaternion.identity, this.transform);
        AudioSource audioSource = AudioObj.GetComponent<AudioSource>();

        AudioClip RandomClip = audioClip[Random.Range(0, audioClip.Length-1)];
        audioSource.clip  = RandomClip;
        audioSource.pitch += Random.Range(-PitchVariation, PitchVariation);

        Destroy(AudioObj, RandomClip.length);
    }
}
