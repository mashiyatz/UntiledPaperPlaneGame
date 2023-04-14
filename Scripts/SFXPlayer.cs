using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SFXPlayer : MonoBehaviour
{
    public AudioClip startPlay;
    public AudioClip getStamp;
    public AudioClip getWind;
    private AudioSource audioSource;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void PlayStartSound()
    {
        audioSource.PlayOneShot(startPlay);
    }

    public void PlayStampSound()
    {
        audioSource.PlayOneShot(getStamp);
    }

    public void PlayWindSound()
    {
        audioSource.PlayOneShot(getWind);
    }
}
