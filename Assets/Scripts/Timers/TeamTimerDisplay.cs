using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TeamTimerDisplay : MonoBehaviour
{
    [SerializeField]
    GameManager gameManager;
    TMPro.TMP_Text timerText;

    float lastTimerValue = 0;

    private void Awake()
    {
        timerText = GetComponent<TMPro.TMP_Text>();           
    }


    private void Update()
    {
        if (gameManager != null) 
        {
            int timerValue = (int)Mathf.Ceil(gameManager.m_GameTimer.CurrentTime);
            timerText.SetText(timerValue.ToString());

            // if the timer value is increasing, change the color to green
            /*if (gameManager.timerIncreasing)
            {
                // alternate between the four player colors
                timerText.color = GameManager.PlayerColours[timerValue % 4];

            }
            else{
                timerText.color = Color.white;
            }*/
        }
    }
}
