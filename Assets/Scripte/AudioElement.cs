using System;
using UnityEngine;
using UnityEngine.Audio;
using Random = UnityEngine.Random;

[Serializable]
public class AudioElement {
    public AudioClip[] AudioClips;
    public float Volume =0.75f;
    public bool UsRandomPitch;
    public float MinPitch =0.9f;
    public float MaxPitch =1.1f;
    
    public void PlayAsSFX() {
        if (AudioClips == null || AudioClips.Length == 0) return;
        AudioBus.OnPlayAudioElementSFX.Invoke(this);
    }

    public void PlaySFXOnGameObject(GameObject gameObject,AudioMixerGroup audioGroup  )
    {
        
        if (AudioClips == null || AudioClips.Length == 0) return;
        AudioClip audioClip = GetAudioClip();
        if (audioClip == null) return;
        AudioSource audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.outputAudioMixerGroup = audioGroup;
        audioSource.volume = Volume;
        audioSource.clip = audioClip;
        if (UsRandomPitch) {
            audioSource.pitch = GetPitch();
        }
        GameObject.Destroy(audioSource, audioClip.length+1);
        audioSource.Play();
    }

    public AudioClip GetAudioClip() {
        //return AudioClips[0];
        return AudioClips[Random.Range(0, AudioClips.Length - 1)];
    }


    public float GetPitch() =>  Random.Range(MinPitch, MaxPitch);
}