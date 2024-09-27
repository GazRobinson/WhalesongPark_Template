using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoseStateController : MonoBehaviour
{
    [SerializeField]
    GameManager gameManager;

    [SerializeField]
    TMPro.TMP_Text[] minigameCountText;
    Animator[] minigameCountAnimators;

    [SerializeField]
    TMPro.TMP_Text[] timeCountText;
    Animator[] timeCountAnimators;

    [SerializeField]
    GameObject loseCanvas;

    float countingDelay = 0.2f;
    int minigameCount = 0;
    int timeCount = 0;

    [SerializeField] AudioClip scoreCounter;
    [SerializeField] AudioClip scoreDing;

    private void Awake()
    {
        minigameCountAnimators = new Animator[4];
        timeCountAnimators = new Animator[4];

        // Get references to scorecard animators
        for (int i = 0; i < minigameCountText.Length; i++)
        {
            minigameCountAnimators[i] = minigameCountText[i].gameObject.GetComponent<Animator>();
            timeCountAnimators[i] = timeCountText[i].gameObject.GetComponent<Animator>();
        }
    }

    private void Start()
    {
        loseCanvas.SetActive(false);
    }

    public void ShowLoseState(int minigamesCompleted) 
    {
        loseCanvas.SetActive(true);

        minigameCount = 0;
        timeCount = 0;

        for (int i = 0; i < minigameCountText.Length; i++)
        {
            minigameCountAnimators[i].SetTrigger("FadeIn");
        }

        StartCoroutine(CountDelay(minigamesCompleted));
    }

    IEnumerator CountDelay(int minigamesCompleted)
    {
        for (int j = 0; j < minigameCountText.Length; j++)
        {
            minigameCountText[j].SetText(minigameCount.ToString());
        }
        for (int i = 0; i < minigamesCompleted; i++) 
        {
            yield return new WaitForSeconds(countingDelay);
            minigameCount++;

            for (int j = 0; j < minigameCountText.Length; j++)
            {
                //Increment fish count value and update canvas text
                if (minigameCountText[j] != null)
                {
                    minigameCountText[j].SetText(minigameCount.ToString());
                    PlayerAudioManager.PlayOneShot(j, scoreCounter);
                }
            }
        }

        for (int j = 0; j < minigameCountAnimators.Length; j++)
        {
            minigameCountAnimators[j].SetTrigger("Pulse");
            PlayerAudioManager.PlayOneShot(j, scoreDing);
        }

        yield return new WaitForSeconds(3.0f);

        StartCoroutine(TimeCountDelay());
    }

    IEnumerator TimeCountDelay()
    {
        for (int i = 0; i < timeCountAnimators.Length; i++)
        {
            timeCountAnimators[i].SetTrigger("FadeIn");
        }

        for (int i = 0; i < gameManager.m_GameTimer.TotalTime; i++)
        {
            yield return new WaitForSeconds(countingDelay / 5.0f);
            timeCount++;

            for (int j = 0; j < timeCountText.Length; j++)
            {
                //Increment fish count value and update canvas text
                if (timeCountText[j] != null)
                {
                    timeCountText[j].SetText(timeCount.ToString() + "s");
                    PlayerAudioManager.PlayOneShot(j, scoreCounter);
                }
            }
        }

        for (int j = 0; j < timeCountAnimators.Length; j++)
        {
            timeCountAnimators[j].SetTrigger("Pulse");
            PlayerAudioManager.PlayOneShot(j, scoreDing);
        }

        yield return new WaitForSeconds(3.0f);

        gameManager.GoToCredits();
    }

    public void Deactivate()
    {
        loseCanvas.SetActive(false);
    }
}
