using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundData
{
    public float volume;
    public float pitch;
    public AudioClip clip;

    public SoundData()
    {
        pitch = 1;
        volume = 1;
    }
    public SoundData(AudioClip clip, float volume, float pitch)
    {
        this.clip = clip;
        this.pitch = pitch;
        this.volume = volume;
    }
    public void SetSoundData(AudioClip clip, float volume, float pitch)
    {
        this.clip = clip;
        this.pitch = pitch;
        this.volume = volume;
    }

    public float GetRandomPitch()
    {
        return Random.Range(pitch - (pitch * 0.01f), pitch + (pitch * 0.01f));
    }
}
