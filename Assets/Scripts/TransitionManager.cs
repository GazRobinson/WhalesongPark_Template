using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class TransitionManager : MonoBehaviour
{
    [SerializeField]
    GameManager gameManager;

    [SerializeField]
    ParticleSystem[] bubbleTransition;

    [SerializeField]
    ParticleSystem fishTransition;
    [SerializeField]
    Animator shutterTransition;

    [SerializeField]
    int transitionCount;
    [SerializeField]
    float transitionLength = 3.0f;

    [SerializeField]
    GameObject[] transitionDisplays;

    Material[] transitionDisplayMaterial;

    public AudioClip bubblesClip;
    public AudioClip shutterClip;

    private void Start()
    {
        transitionDisplayMaterial = new Material[4];
        for (int i = 0; i < 4; i++) 
        {
            Image transitionImage = transitionDisplays[i].GetComponent<Image>();
            transitionDisplayMaterial[i] = transitionImage.material;
            transitionDisplayMaterial[i].color = PlayerUtilities.GetPlayerColor(i);
        }
    }

    // Play bubble transition calling switch callback halfway through animation
    public void PlayBubbleTransition(int playerIndex, Action callback = null) 
    {
        if (playerIndex < bubbleTransition.Length) 
        {
            bubbleTransition[playerIndex].Play();
            StartCoroutine(DelayForTime(1.0f, callback));
        }
    }

    // Play a random transition from the set and call switch call-back function half way through 
    public void PlayRandomTransition(Action switchCallback)
    {
        for (int i = 0; i < PlayerUtilities.GetTotalPlayerCount(); i++) 
        {
            if (PlayerUtilities.GetPlayerState(i) == Player.PlayerState.ACTIVE)
            {
                SetTransitionDisplayVisiblity(i, true);
            }
            else
            {
                SetTransitionDisplayVisiblity(i, false);
            }
        }

        int randomIndex = UnityEngine.Random.Range(0, transitionCount);
        float halfLifetime = transitionLength / 2.0f;

        Debug.Log("Play transition with index: " + randomIndex);

        switch (randomIndex)
        {
            case 0:
                fishTransition.Play();
                PlayerAudioManager.PlayGlobalOneShot(bubblesClip);
                break;
            case 1:
                shutterTransition.SetTrigger("PlayTransition");
                PlayerAudioManager.PlayGlobalOneShot(shutterClip);
                break;
        }

        StartCoroutine(DelayForTime(halfLifetime, switchCallback));

        StartCoroutine(DelayForTime(halfLifetime * 10.0f, () =>
        {
            for (int i = 0; i < PlayerUtilities.GetTotalPlayerCount(); i++)
            {
                SetTransitionDisplayVisiblity(i, false);
            }
        }));
    }

    public void SetTransitionDisplayVisiblity(int playerIndex, bool visible) 
    {
        if (playerIndex < transitionDisplays.Length) 
        {
            transitionDisplays[playerIndex].SetActive(visible);
        }
    }

    // Utility function to delay for a custom time before executing a callback function
    IEnumerator DelayForTime(float delay, Action callback = null)
    {
        yield return new WaitForSeconds(delay);
        if (callback != null)
        {
            callback();
        }
    }
}
