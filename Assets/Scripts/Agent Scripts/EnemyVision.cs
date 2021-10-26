using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AICore))]
public class EnemyVision : MonoBehaviour
{
    private AICore aiCore;

    [SerializeField]
    private float viewRadius;
    [SerializeField]
    [Range(0, 360)]
    private float viewAngle;

    [SerializeField]
    private LayerMask targetMask;
    [SerializeField]
    private LayerMask obstacleMask;

    private void Awake()
    {
        aiCore = GetComponent<AICore>();
    }


    void Update()
    {

    }
}
