using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JoiningTextAnimation : MonoBehaviour
{
    TMPro.TMP_Text joiningText;

    float animationTick = 1.0f;
    float tickTimer = 0.0f;

    int dotIndex = 0;

    private void Awake()
    {
        joiningText = GetComponent<TMPro.TMP_Text>();
    }

    private void Update()
    {
        tickTimer += Time.deltaTime;
        if (tickTimer >= animationTick) 
        {
            tickTimer = 0.0f;
            dotIndex++;
            if (dotIndex > 3) 
            {
                dotIndex = 0;
            }
        }

        string joiningString = "Joining";
        for (int i = 0; i < dotIndex; i++) 
        {
            joiningString += ".";
        }
        joiningText.SetText(joiningString);
    }
}
