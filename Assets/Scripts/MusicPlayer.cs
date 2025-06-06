using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicPlayer : MonoBehaviour
{

    [SerializeField] public AudioSource soundFXObject;
    [SerializeField] public AudioClip audioClip;

    public void OnEnable()
    {
        if (audioClip == null)
        {
            return;
        }
        AudioSource audioSource = Instantiate(soundFXObject, Vector3.zero, Quaternion.identity);
        audioSource.clip = audioClip;
        audioSource.loop = true;
        audioSource.Play();
    }
}

