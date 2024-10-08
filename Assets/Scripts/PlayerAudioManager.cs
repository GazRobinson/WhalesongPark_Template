using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Wrapper class for ease of use
public static class WhalesongAudio
{
    public static void PlayOneShot(int playerIndex, AudioClip clip, float volume = 1.0f, float pitch = 1.0f)
    {
        PlayerAudioManager.PlayOneShot(playerIndex, clip, volume, pitch);
    }

    public static void PlayGlobalOneShot(AudioClip clip, float volume = 1.0f, float pitch = 1.0f)
    {
        PlayerAudioManager.PlayGlobalOneShot(clip, volume, pitch);
    }
}

public class PlayerAudioManager : MonoBehaviour
{
    private static PlayerAudioManager instance;
    public AudioSource[] playerAudioSources;

    [SerializeField] private int totalAudioChannels = 8;
    // Speakers, front left, front right, rear left and rear right
    [SerializeField] private int[] playerAudioChannelIndex = { 0, 1, 2, 3 };

    private void Awake()
    {
        if(instance != null)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
    }

    public static void PlayOneShot(int playerIndex, AudioClip clip, float volume = 1.0f, float pitch = 1.0f)
    {
        if (playerIndex < instance.playerAudioSources.Length)
        {
            AudioClip channelDirectedClip = AudioClipExtensions.CreateSpeakerSpecificClip(clip, instance.totalAudioChannels, instance.playerAudioChannelIndex[playerIndex]);
            instance.playerAudioSources[playerIndex].pitch = pitch;
            instance.playerAudioSources[playerIndex].PlayOneShot(channelDirectedClip, volume);
        }
    }

    public static void PlayGlobalOneShot(AudioClip clip, float volume = 1.0f, float pitch = 1.0f)
    {
        for (int i = 0; i < 4; i++) 
        {
            if (instance.playerAudioSources[i] != null)
            {
                AudioClip channelDirectedClip = AudioClipExtensions.CreateSpeakerSpecificClip(clip, instance.totalAudioChannels, instance.playerAudioChannelIndex[i]);
                instance.playerAudioSources[i].pitch = pitch;
                instance.playerAudioSources[i].PlayOneShot(channelDirectedClip, volume);
            }
        }
    }
}
