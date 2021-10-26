using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    private GameObject player;
    private List<AICore> enemies = new List<AICore>{};

    [SerializeField]
    private bool alertAllEnemiesOnSpotted = false;
    [SerializeField]
    private AudioClip undetectedMusic;
    [SerializeField]
    private AudioClip susMusic;
    [SerializeField]
    private AudioClip chasingMusic;

    [SerializeField]
    private AudioSource audioSource;

    public enum GameState
    {
        UNDETECTED,
        SUS,
        DETECTED,
        WIN,
        LOSE
    }
    private GameState gameState = GameState.UNDETECTED;

    private void Awake()
    {
        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
        }
    }

    void Start()
    {
        enemies = FindObjectsOfType<AICore>().ToList();
        player = FindObjectOfType<PlayerController>().gameObject;
    }

    private void ChangeMusic(AudioClip clip)
    {
        if (audioSource != null)
        {
            if (clip != audioSource.clip)
            {
                audioSource.Stop();
                audioSource.clip = clip;
                audioSource.Play();
            }
        }
    }

    public void UpdateGameState(GameState gameState)
    {
        if (this.gameState != gameState)
        {
            if (gameState == GameState.UNDETECTED)
            {
                ChangeMusic(undetectedMusic);
            }
            else if (gameState == GameState.SUS)
            {
                ChangeMusic(susMusic);
            }
            else if (gameState == GameState.DETECTED)
            {
                ChangeMusic(chasingMusic);
                if (alertAllEnemiesOnSpotted)
                {
                    enemies.ForEach(e => e.Spotted());
                }
            }
            else if (gameState == GameState.LOSE)
            {
                foreach (var enemy in enemies)
                {
                    Destroy(enemy);
                }
            }
            else if (gameState == GameState.WIN)
            {
                foreach (var enemy in enemies)
                {
                    Destroy(enemy);
                }
            }
            this.gameState = gameState;
        }
    }

    public void SignalSound(Vector3 position, float audioRadius)
    {
        foreach (var enemy in enemies)
        {
            if (enemy != null)
            {
                if (Vector3.Distance(enemy.transform.position, position) <= audioRadius)
                {
                    enemy.Alert(position);
                }
            }
        }
    }
}
