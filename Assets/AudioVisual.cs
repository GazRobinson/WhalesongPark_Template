using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioVisual : MonoBehaviour
{
    public GameManager gameManager;
    TMPro.TMP_Text text;

    private void Awake()
    {
        text = GetComponent<TMPro.TMP_Text>();
    }

    private void Update()
    {
        //text.SetText(gameManager.channelIndex.ToString());
    }
}
