using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteSpin : MonoBehaviour
{
    [SerializeField] private float rotationSpeed = 10.0f;
    private RectTransform rectTransform;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    private void Update()
    {
        rectTransform.Rotate(Vector3.forward, rotationSpeed * Time.deltaTime);
    }
}
