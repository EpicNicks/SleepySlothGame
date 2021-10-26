using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayNarrativeAudio : MonoBehaviour
{

    public AudioSource audio;
    private bool isPlayed = false;

    void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            if (isPlayed == false)
            {
                Debug.Log("PlayNarration");
                audio.Play();
                isPlayed = true;
            }

            //Time.timeScale = 0f;
        }
    }
}
