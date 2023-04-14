using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoatFloat : MonoBehaviour
{
    private float frequency;
    private float amplitude;
    private Vector3 startPos;

    void Start()
    {
        frequency = 1.5f;
        amplitude = 0.05f;
        startPos = transform.position;
    }


    void Update()
    {
        transform.position = startPos + Vector3.up * amplitude * Mathf.Sin(frequency * Time.time);
    }
}
