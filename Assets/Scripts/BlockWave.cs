using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BlockWave : MonoBehaviour
{
    public GameObject blockPrefab;
    public int blockCount = 40;

    RectTransform ownRect;
    public RectTransform[] blockTransforms;

    public float amplitude = 1.0f;
    public float frequency = 1.0f;
    public float offset = 1.0f;
    public float speed = 1.0f;

    public Color colour;

    private void Awake()
    {
        ownRect = GetComponent<RectTransform>();
        blockTransforms = new RectTransform[blockCount];
    }

    private void Start()
    {
        GenerateBlockArray();
    }

    private void GenerateBlockArray() 
    {
        for (int i = 0; i < blockCount; i++)
        {
            GameObject currentBlock = Instantiate(blockPrefab, transform) as GameObject;
            RectTransform currentRect = currentBlock.GetComponent<RectTransform>();

            Image blockImage = currentBlock.GetComponent<Image>();
            blockImage.color = colour;

            float blockWidth = ownRect.rect.width / (float)blockCount;
            Vector2 position = new Vector2((-168 + blockWidth / 2.0f) + i * blockWidth, -325.0f);
            currentRect.anchoredPosition = position;

            blockTransforms[i] = currentRect;
        }
    }

    private void Update()
    {
        for (int i = 0; i < blockTransforms.Length; i++)
        {
            float blockWidth = ownRect.rect.width / (float)blockCount;
            float timedOffset = offset + Time.time * speed;
            Vector2 position = new Vector2((-168 + blockWidth / 2.0f) + i * blockWidth, -325.0f + (amplitude * Mathf.Sin(((float)i * frequency) + timedOffset)));
            blockTransforms[i].anchoredPosition = position;
        }
    }
}
