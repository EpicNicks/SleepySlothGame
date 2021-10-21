using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    [SerializeField]
    [Tooltip("The distance that audio generated at a point can be heard from")]
    private float audioRadius = 1.0f;
    private List<AICore> enemies = new List<AICore>{};

    void Start()
    {
        enemies = FindObjectsOfType<AICore>().ToList();
    }

    void Update()
    {
        
    }

    public void SignalSound(Vector3 position)
    {
        foreach (var enemy in enemies)
        {
            if (Vector3.Distance(enemy.transform.position, position) <= audioRadius)
            {
                enemy.Alert(position);
            }
        }
    }
}
