using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager instance;

    [SerializeField] private AudioSource soundFXObject;

    private void Awake()
    {
        if( instance == null)
        {
            instance = this;
        }
    }

    /// <summary>
    /// will be choosent random when a list of sound is given
    /// </summary>
    /// <param name="audioClip"></param>
    public void PlaySoundFXClip([CanBeNull] AudioClip audioClip)
    {
        if (audioClip == null)
        {
            return;
        }
        
        AudioSource audioSource = Instantiate(soundFXObject, Vector3.zero, Quaternion.identity);

        audioSource.clip = audioClip;

        audioSource.Play();

        float clipLength = audioSource.clip.length;

        Destroy(audioSource.gameObject, clipLength);

    }
    /// <summary>
    /// will be chosen random when a list of sound is given
    /// </summary>
    /// <param name="audioClip"></param>
    public void PlaySoundFXClip(AudioClip[] audioClip)
    {
        PlaySoundFXClip(audioClip[Random.Range(0, audioClip.Length - 1)]);
    }
}
