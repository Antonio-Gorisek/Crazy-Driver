using UnityEngine;
using System.Collections;

public static class AudioManager
{
    public static AudioSource PlayFromResources(string soundFileName, float volume = 1.0f, float pitch = 1.0f, bool loop = false)
    {
        AudioClip soundClip = Resources.Load<AudioClip>($"Sound Effects/{soundFileName}");

        if (soundClip == null)
        {
            Debug.LogError($"Sound clip with name {soundFileName} not found in Resources folder.");
            return null;
        }

        GameObject soundGameObject = new GameObject(soundFileName);
        AudioSource soundSource = soundGameObject.AddComponent<AudioSource>();
        soundSource.clip = soundClip;
        soundSource.pitch = pitch;
        soundSource.volume = volume;
        soundSource.loop = loop;
        soundSource.Play();
        if (loop == false) { Object.Destroy(soundGameObject, soundClip.length); }
        Debug.Log($"Sound {soundFileName} has been played.");
        return soundSource;
    }

    public static void StopAudioClip(string soundFileName, float fadeDuration = 1.0f)
    {
        GameObject soundGameObject = GameObject.Find(soundFileName);
        if (soundGameObject == null)
        {
            Debug.LogError($"Sound clip with name {soundFileName} not found.");
            return;
        }

        AudioSource soundSource = soundGameObject.GetComponent<AudioSource>();
        if (soundSource != null)
        {
            soundGameObject.AddComponent<MonoBehaviourHelper>().StartCoroutine(FadeOutAndDestroy(soundSource, fadeDuration));
        }
    }

    private static IEnumerator FadeOutAndDestroy(AudioSource soundSource, float fadeDuration)
    {
        float startVolume = soundSource.volume;

        while (soundSource.volume > 0)
        {
            soundSource.volume -= startVolume * Time.deltaTime / fadeDuration;
            yield return null;
        }

        soundSource.Stop();
        Object.Destroy(soundSource.gameObject);
    }

    public static void PlayAudioClip(AudioClip soundClipFile, float volume = 1.0f, float pitch = 1.0f)
    {
        if (soundClipFile == null)
        {
            Debug.LogError($"Sound clip not found.");
            return;
        }

        GameObject soundGameObject = new GameObject(soundClipFile.name);
        AudioSource soundSource = soundGameObject.AddComponent<AudioSource>();
        soundSource.clip = soundClipFile;
        soundSource.pitch = pitch;
        soundSource.volume = volume;
        soundSource.Play();
        Debug.Log($"Sound {soundClipFile.name} has been played.");
        Object.Destroy(soundGameObject, soundClipFile.length);
    }
}

/// <summary>
///  Helper MonoBehaviour class to allow running coroutines in static class
/// </summary>
public class MonoBehaviourHelper : MonoBehaviour { }