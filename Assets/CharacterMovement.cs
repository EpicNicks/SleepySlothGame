using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterMovement : MonoBehaviour
{
    [SerializeField] 
    float moveSpeed = 4;

    Vector3 forward, right;
    
    // Start is called before the first frame update
    void Start()
    {
        forward = Camera.main.transform.forward;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
