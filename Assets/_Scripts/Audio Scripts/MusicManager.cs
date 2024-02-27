using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MusicManager : MonoBehaviour
{
    public AudioClip arcadeMusic;
    public AudioClip menuMusic;

    private AudioSource audioSrc;

    private const float NORMAL_VOLUME = 0.04f;
    private const float MUTED_VOLUME = 0.02f;

    private Dictionary<string, AudioClip> musicDict = new Dictionary<string, AudioClip>();
    
    private void Start()
    {
        GameController.RegisterObserver(this);

        audioSrc = GetComponent<AudioSource>();
        audioSrc.loop = true;

        audioSrc.clip = GetMusic();
        audioSrc.volume = NORMAL_VOLUME;

        audioSrc.Play();
    }
    private void OnDestroy()
    {
        GameController.UnRegisterObserver(this);
    }

    private AudioClip GetMusic()
    {
        switch (SceneManager.GetActiveScene().buildIndex)
        {
            case 0:
            case 2:
                return menuMusic;
            case 1:
                return arcadeMusic;
            default:
                return null;
        }
    }
    private void ChangeMusic(AudioClip newClip)
    {
        if(audioSrc.clip != newClip)
        {
            audioSrc.clip = newClip;
            audioSrc.Play();
        }
    }
    public void GamePause(bool b)
    {
        if (b)
            audioSrc.volume = MUTED_VOLUME;
        else
            audioSrc.volume = NORMAL_VOLUME;
    }
}
