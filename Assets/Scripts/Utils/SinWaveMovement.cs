using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SinWaveMovement : MonoBehaviour
{
    [SerializeField]
    float amplitude = 1.0f;
    [SerializeField]
    float frequency = 1.0f;
    [SerializeField]
    float offset = 0.0f;

    float yPosition;
    float originalYPosition;

    private void Start()
    {
        originalYPosition = transform.position.y;
    }

    private void Update()
    {
        // Apply an offset on the Y position with the custom sin wave
        yPosition = originalYPosition + amplitude * Mathf.Sin(Time.time * frequency + offset);
        Vector2 position = new Vector2(transform.position.x, yPosition);
        transform.position = position;
    }
}
