using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioController : MonoBehaviour
{
    [SerializeField] AudioClip audioOutPlay;
    [SerializeField] AudioClip audioInPlay;
    [SerializeField] public AudioSource audioSourceBGM;
    [SerializeField] AudioSource audioSourceEffect;
    [SerializeField] AudioSource audioSourceEffectDone;

    public void PlayAudioStartGame()
    {
        audioSourceBGM.Stop();
        audioSourceBGM.clip = audioInPlay;
        audioSourceBGM.Play();
    }

    public void PlayAudioOut()
    {
        audioSourceBGM.Stop();
        audioSourceBGM.clip = audioOutPlay;
        audioSourceBGM.Play();
    }

    public void PlaySplat()
    {
        if(audioSourceEffect != null ) audioSourceEffect?.Play();
    }

    public void PlaySplatDone()
    {
        if (audioSourceEffectDone != null) audioSourceEffectDone?.Play();
    }
    public void PlayAddPoint(AudioClip clipE)
    {
        if (audioSourceEffectDone == null) return;
        audioSourceEffectDone.Stop();
        audioSourceEffectDone.clip = clipE;
        audioSourceEffectDone.Play();
    }

}
