using System.Collections;
using System.Collections.Generic;
using System.Drawing.Printing;
using UnityEngine;

public static class AudioClipExtensions
{
    //Channel index | Speaker
    //0	| Front left
    //1	| Front right
    //2	| Center
    //3	| Bass
    //4	| Rear left(5.1) | Surround left(7.1)
    //5	| Rear right(5.1) | Surround right(7.1)
    //6	| Rear left(7.1)
    //7	| Rear right(7.1)

    // Source

    //  http://thomasmountainborn.com/2016/06/10/sending-audio-directly-to-a-speaker-in-unity/
    public static Dictionary<int, AudioClip> speakerSpecificClips = new Dictionary<int, AudioClip>();

    public static AudioClip CreateSpeakerSpecificClip(this AudioClip originalClip, int amountOfChannels, int targetChannel)
    {
        if (speakerSpecificClips.ContainsKey(originalClip.GetInstanceID()))
        {
            return (speakerSpecificClips[originalClip.GetInstanceID()]);
        }
        else
        {
            // Create a new clip with the target amount of channels.
            AudioClip clip = AudioClip.Create(originalClip.name, originalClip.samples, amountOfChannels, originalClip.frequency, false);
            // Init audio arrays.
            float[] audioData = new float[originalClip.samples * amountOfChannels];
            float[] originalAudioData = new float[originalClip.samples * originalClip.channels];
            if (!originalClip.GetData(originalAudioData, 0))
                return null;
            // Fill in the audio from the original clip into the target channel. Samples are interleaved by channel (L0, R0, L1, R1, etc).
            int originalClipIndex = 0;
            for (int i = targetChannel; i < audioData.Length; i += amountOfChannels)
            {
                audioData[i] = originalAudioData[originalClipIndex];
                originalClipIndex += originalClip.channels;
            }
            if (!clip.SetData(audioData, 0))
                return null;

            speakerSpecificClips.Add(originalClip.GetInstanceID(), clip);
            return clip;
        }
    }
}

