using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateClouds : MonoBehaviour
{
    public float rotationSpeed;

    void Start()
    {
        
    }

    void Update()
    {
        transform.Rotate(rotationSpeed * Time.deltaTime * Vector3.up);
    }
}
