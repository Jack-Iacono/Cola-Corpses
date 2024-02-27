using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundSourceController : MonoBehaviour
{
    private AudioSource audioSrc;
    private bool soundPlaying = false;

    // Start is called before the first frame update
    public void Initialize()
    {
        audioSrc = GetComponent<AudioSource>();
    }

    public void PlaySound(Vector3 pos, SoundData data)
    {
        transform.position = pos;

        audioSrc.pitch = data.GetRandomPitch();

        audioSrc.PlayOneShot(data.clip, data.volume);
        soundPlaying = true;
    }

    private void Update()
    {
        if (soundPlaying)
        {
            if (!audioSrc.isPlaying)
            {
                soundPlaying = false;
                gameObject.SetActive(false);
            }
        } 
    }
}
