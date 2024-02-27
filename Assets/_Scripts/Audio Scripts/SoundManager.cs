using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    private static AudioSource audioSrc;
    private static bool isInitialized = false;

    [Header("Notification")]
    public AudioClip notifyClip;
    public AudioClip pickupClip;
    public AudioClip successClip;
    public AudioClip canOpenClip;

    [Header ("Can Throwing")]
    public AudioClip canThrowClip;
    public AudioClip canHitClip;

    [Header("Player Sounds")]
    public AudioClip hurtClip;
    public AudioClip jumpClip;

    [Header("Pickups")]
    public AudioClip coinClip;
    public AudioClip errorClip;

    [Header ("Easter Egg Sounds")]
    public AudioClip eePickupClip;
    public AudioClip eeHitClip;
    public AudioClip eeMoveClip;

    [Header ("Menu Sounds")]
    public AudioClip menuClickClip;
    public AudioClip menuHoverClip;

    public static SoundData notify { get; private set; } = new SoundData();
    public static SoundData pickup { get; private set; } = new SoundData();
    public static SoundData success { get; private set; } = new SoundData();
    public static SoundData canOpen { get; private set; } = new SoundData();
    public static SoundData canThrow { get; private set; } = new SoundData();
    public static SoundData canHit { get; private set; } = new SoundData();
    public static SoundData hurt { get; private set; } = new SoundData();
    public static SoundData jump { get; private set; } = new SoundData();
    public static SoundData coin { get; private set; } = new SoundData();
    public static SoundData error { get; private set; } = new SoundData();
    public static SoundData eePickup { get; private set; } = new SoundData();
    public static SoundData eeHit { get; private set; } = new SoundData();
    public static SoundData eeMove { get; private set; } = new SoundData();
    public static SoundData menuClick { get; private set; } = new SoundData();
    public static SoundData menuHover { get; private set; } = new SoundData();

    private void Start()
    {
        audioSrc = GetComponent<AudioSource>();

        if (!isInitialized)
        {
            // Initialize the SoundData
            notify.SetSoundData(canThrowClip, 0.6f, 0.95f);
            pickup.SetSoundData(pickupClip, 0.95f, 1);
            success.SetSoundData(successClip, 0.8f, 1f);
            canOpen.SetSoundData(canOpenClip, 0.7f, 0.95f);
            canThrow.SetSoundData(canThrowClip, 0.4f, 0.75f);
            canHit.SetSoundData(canHitClip, 0.01f, 0.7f);
            hurt.SetSoundData(hurtClip, 0.1f, 0.95f);
            jump.SetSoundData(jumpClip, 0.05f, 0.95f);
            coin.SetSoundData(coinClip, 0.55f, 1.1f);
            error.SetSoundData(errorClip, 0.6f, 0.95f);
            eePickup.SetSoundData(eePickupClip, 0.6f, 0.95f);
            eeHit.SetSoundData(eeHitClip, 0.05f, 0.95f);
            eeMove.SetSoundData(eeMoveClip, 0.075f, 0.95f);
            menuClick.SetSoundData(menuClickClip, 1f, 1);
            menuHover.SetSoundData(menuHoverClip, 0.95f, 1);

            isInitialized = true;
        }
    }

    public static void PlaySoundSource(Vector3 pos, SoundData sound)
    {
        GameObject soundSrc = ObjectPool.objPool.GetPooledObject("SoundSource");
        if (soundSrc)
        {
            soundSrc.SetActive(true);
            soundSrc.GetComponent<SoundSourceController>().PlaySound(pos, sound);
        }
    }

    public static void PlaySound(SoundData sound)
    {
        PlaySound(sound, audioSrc);
    }
    public static void PlaySound(SoundData sound, AudioSource source)
    {
        source.pitch = sound.pitch;
        source.PlayOneShot(sound.clip, sound.volume);
    }

    public static void PlayRandomSound(List<SoundData> sounds)
    {
        PlayRandomSound(sounds, audioSrc);
    }
    public static void PlayRandomSound(List<SoundData> sounds, AudioSource source)
    {
        int rand = Random.Range(0, sounds.Count);

        PlaySound(sounds[rand], source);
    }
}
