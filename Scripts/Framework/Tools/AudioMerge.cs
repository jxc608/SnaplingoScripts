using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioMerge : MonoBehaviour {

    public static AudioClip SpliceAudioClips(List<AudioClip> list)
    {
        if (list == null || list.Count == 0)
            return null;

        int length = 0;
        for (int i = 0; i < list.Count; i++)
        {
            if (list[i] == null)
                continue;

            length += list[i].samples * list[i].channels;
        }

        float[] data = new float[length];
        length = 0;
        for (int i = 0; i < list.Count; i++)
        {
            if (list[i] == null)
                continue;

            float[] buffer = new float[list[i].samples * list[i].channels];
            list[i].GetData(buffer, 0);
            buffer.CopyTo(data, length);
            length += buffer.Length;
        }

        if (length == 0)
            return null;

        AudioClip result = AudioClip.Create("Combine", data.Length, 1, 44100, false);
        result.SetData(data, 0);

        return result;
    }

    public static AudioClip MergeTwoClip(AudioClip a, AudioClip b)
    {
        if (a == null || b == null)
            return null;

        int aLength = a.samples * a.channels;
        int bLength = b.samples * b.channels;

        int maxLength = Mathf.Max(aLength, bLength);

        float[] aSamples = new float[aLength];
        a.GetData(aSamples,0);
        float[] bSamples = new float[bLength];
        b.GetData(bSamples, 0);

        float[] result = new float[maxLength];
        for (int i = 0; i < maxLength; i++)
        {
            float aValue = 0, bValue = 0;
            if (i < aLength)
                aValue = aSamples[i];
            if (i < bLength)
                bValue = bSamples[i];

            //float samplef1 = aValue / 32768.0f;
            //float samplef2 = bValue / 32768.0f;
            float mixed = aValue + bValue;
            // reduce the volume a bit:
            //mixed *= 0.8f;
            // hard clipping
            //if (mixed > 1.0f) mixed = 1.0f;
            //if (mixed < -1.0f) mixed = -1.0f;
            result[i] = mixed;
        }

        AudioClip resultClip = AudioClip.Create("Merge", result.Length, 1, 44100, false);
        resultClip.SetData(result, 0);

        return resultClip;
    }
}
