using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, viewRadius);
    }

    private void Awake()
    {
        aiCore = transform.parent.GetComponentInChildren<AICore>();
    }

    private void Update()
    {
        if (Vector3.Distance(aiCore.PlayerTransform.position, transform.position) < viewRadius)
        {
            if (Mathf.Abs(Vector3.Angle(aiCore.PlayerTransform.position, transform.position)) < viewAngle)
            {
                if (!(aiCore.PlayerTransform.GetComponent<PlayerController>().State is SnoreState))
                {
                    aiCore.Spotted();
                }
            }
        }
    }
}
