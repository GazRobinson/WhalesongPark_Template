using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class SinWaveLineRenderer : MonoBehaviour
{
    public LineRenderer line;

    [SerializeField]
    int pointCount = 10;
    [SerializeField]
    float length = 10.0f;
    [SerializeField]
    float thickness = 1.0f;

    [SerializeField]
    float amplitude = 1.0f;
    [SerializeField]
    float frequency = 1.0f;
    [SerializeField]
    float offset = 0.0f;
    [SerializeField]
    float speed = 1.0f;
    [ColorUsage(true, true)]
    public Color lineColor;
    [SerializeField]
    int moveDirection = 1;


    private void Awake()
    {
        line = GetComponent<LineRenderer>();
    }

    private void Start()
    {
        GenerateLine();
    }

    private void Update()
    {
        float segmentLength = length / ((float)pointCount - 1.0f);
        float startingXPoint = -length / 2.0f;

        for (int i = 0; i < pointCount; i++)
        {
            float xPos = startingXPoint + (segmentLength * i);
            float yPos = amplitude * Mathf.Sin(xPos * frequency + offset + (Time.time * speed * moveDirection));

            Vector2 position = new Vector2(xPos, yPos);

            line.SetPosition(i, position);
        }
    }

    private void OnValidate()
    {
        GenerateLine();
    }

    private void GenerateLine()
    {
        line.positionCount = pointCount;
        line.startWidth = thickness;
        line.endWidth = thickness;
        line.startColor = lineColor;
        line.endColor = lineColor;

        float segmentLength = length / ((float)pointCount - 1.0f);
        float startingXPoint = -length / 2.0f;

        for (int i = 0; i < pointCount; i++)
        {
            float xPos = startingXPoint + (segmentLength * i);
            float yPos = amplitude * Mathf.Sin(xPos * frequency + offset + (Time.time * speed * moveDirection));

            Vector2 position = new Vector2(xPos, yPos);

            line.SetPosition(i, position);
        }
    }
}


