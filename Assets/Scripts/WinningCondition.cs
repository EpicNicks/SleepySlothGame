using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WinningCondition : MonoBehaviour
{
    public GameObject winningMenuUI;

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Debug.Log("WON");
            winningMenuUI.SetActive(true);
            //Time.timeScale = 0f;
        }
    }
}