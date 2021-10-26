using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(AudioSource))]

public class PlayNarrativeAudio : MonoBehaviour
{

    public AudioClip audio;
    private AudioSource audioSource;
    private bool isPlayed = false;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            if (isPlayed == false)
            {
                Debug.Log("PlayNarration");
                audioSource.PlayOneShot(audio);
                isPlayed = true;
            }

            //Time.timeScale = 0f;
        }
    }
}
