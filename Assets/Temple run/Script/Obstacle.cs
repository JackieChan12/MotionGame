using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class Obstacle : MonoBehaviour
{
    public AudioClip impactedSound;
    public AudioSource audioSource;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public virtual void Impacted()
    {
        if (audioSource != null && impactedSound != null)
        {
            audioSource.Stop();
            audioSource.loop = false;
            audioSource.clip = impactedSound;
            audioSource.Play();
        }
    }
}
