using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]


public class InitalNarrativePiece : MonoBehaviour
{

    public AudioClip audio;
    private AudioSource audioSource;
    private bool isPlayed = false;
    public GameObject canvas;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        if ((Input.GetKeyDown("space")) || (Input.GetKeyDown(KeyCode.LeftShift)))
        {
            canvas.SetActive(false);
        }
    }

    void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            if (isPlayed == false)
            {
                Debug.Log("PlayNarration");
                audioSource.PlayOneShot(audio);
                canvas.SetActive(true);
                isPlayed = true;
            }

            //Time.timeScale = 0f;
        }
    }
}
