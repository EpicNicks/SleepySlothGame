using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InitalNarrativePiece : MonoBehaviour
{

    public AudioSource audio;
    private bool isPlayed = false;
    public GameObject canvas;


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
                audio.Play();
                canvas.SetActive(true);
                isPlayed = true;
            }

            //Time.timeScale = 0f;
        }
    }
}
