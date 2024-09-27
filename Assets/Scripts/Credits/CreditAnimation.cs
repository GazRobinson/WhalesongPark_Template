using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreditAnimation : MonoBehaviour
{
    public GameObject[] creditsPanels;
    Animator[] panelAnimsA;
    Animator[] panelAnimsB;
    Animator[] panelAnimsC;
    Animator[] panelAnimsD;

    GameManager gameManager;

    public GameObject creditDisplay;

    private void Awake()
    {
        gameManager = GameObject.FindObjectOfType<GameManager>();

        panelAnimsA = creditsPanels[0].GetComponentsInChildren<Animator>();
        panelAnimsB = creditsPanels[1].GetComponentsInChildren<Animator>();
        panelAnimsC = creditsPanels[2].GetComponentsInChildren<Animator>();
        panelAnimsD = creditsPanels[3].GetComponentsInChildren<Animator>();

        creditDisplay.SetActive(false);
    }

    public void ResetCredits() 
    {
        for (int i = 0; i < panelAnimsA.Length; i++)
        {
            panelAnimsA[i].SetTrigger("Reset");
        }
        for (int i = 0; i < panelAnimsB.Length; i++)
        {
            panelAnimsB[i].SetTrigger("Reset");
        }
        for (int i = 0; i < panelAnimsC.Length; i++)
        {
            panelAnimsC[i].SetTrigger("Reset");
        }
        for (int i = 0; i < panelAnimsD.Length; i++)
        {
            panelAnimsD[i].SetTrigger("Reset");
        }
    }

    public void PlayCredits() 
    {
        creditDisplay.SetActive(true);
        StartCoroutine(CreditAnimations());
    }

    public void HideCredits() 
    {
        ResetCredits();
        creditDisplay.SetActive(false);
    }

    IEnumerator CreditAnimations() 
    {
        yield return new WaitForSeconds(2.0f);
        for (int i = 0; i < panelAnimsA.Length; i++) 
        {
            panelAnimsA[i].SetTrigger("FadeIn");
        }
        yield return new WaitForSeconds(4.0f);
        for (int i = 0; i < panelAnimsA.Length; i++)
        {
            panelAnimsA[i].SetTrigger("FadeOut");
        }
        yield return new WaitForSeconds(1.0f);



        for (int i = 0; i < panelAnimsB.Length; i++)
        {
            panelAnimsB[i].SetTrigger("FadeIn");
        }
        yield return new WaitForSeconds(4.0f);
        for (int i = 0; i < panelAnimsB.Length; i++)
        {
            panelAnimsB[i].SetTrigger("FadeOut");
        }
        yield return new WaitForSeconds(1.0f);



        for (int i = 0; i < panelAnimsC.Length; i++)
        {
            panelAnimsC[i].SetTrigger("FadeIn");
        }
        yield return new WaitForSeconds(4.0f);
        for (int i = 0; i < panelAnimsC.Length; i++)
        {
            panelAnimsC[i].SetTrigger("FadeOut");
        }
        yield return new WaitForSeconds(1.0f);

        for (int i = 0; i < panelAnimsD.Length; i++)
        {
            panelAnimsD[i].SetTrigger("FadeIn");
        }
        yield return new WaitForSeconds(4.0f);
        for (int i = 0; i < panelAnimsD.Length; i++)
        {
            panelAnimsD[i].SetTrigger("FadeOut");
        }
        yield return new WaitForSeconds(1.0f);
    }
}
