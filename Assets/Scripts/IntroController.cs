using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class IntroController : MonoBehaviour
{
    [SerializeField] private List<TutorialStage> pages;

    [SerializeField] private AudioClip pageShowSound;

    public System.Action OnIntroComplete;

    private int counterTime = 1;

    [SerializeField] private AudioClip counterSound;

    private int colourCounter = 0;

    public void Begin(System.Action callback = null)
    {
        gameObject.SetActive(true);
        OnIntroComplete = callback;

        for (int i = 0; i < pages.Count; i++)
        {
            for (int j = 0; j < pages[i].playerPanels.Count; j++)
            {
                //TODO: This feels like it would get out of sync
                pages[i].playerPanels[j].SetTrigger("Reset");
                pages[i].playerPanels[j].gameObject.SetActive(false);
            }
        }

        StartCoroutine(PlayStages());
    }

    public IEnumerator PlayStages(System.Action callback = null)
    {
        

        float t = 16 / pages.Count;

        for(int i=0; i < pages.Count; i++)
        {
            
            for (int j = 0; j < pages[i].playerPanels.Count; j++)
            {
                pages[i].playerPanels[j].gameObject.SetActive(true);
                pages[i].playerPanels[j].SetTrigger("Show");
                // play sound effect
                if (pageShowSound != null)
                PlayerAudioManager.PlayOneShot(j, pageShowSound, 1, 1 + ((float)i/50.0f));

                // if we find a text componenet in child, set the text colour to the player colour
                if (pages[i].playerPanels[j].gameObject.GetComponentInChildren<TextMeshProUGUI>() != null && i != 5 && i != 0 && i != 1){
                    pages[i].playerPanels[j].gameObject.GetComponentInChildren<TextMeshProUGUI>().color = UIManager.GetPlayerColor(j);
                }
                
            }
            yield return new WaitForSeconds(t);

            if (i == 0){
                for (int j = 0; j < pages[i].playerPanels.Count; j++)
                {
                    pages[0].playerPanels[j].SetTrigger("Hide");
                }
            }

            if (i == 3){
                for (int j = 0; j < pages[i].playerPanels.Count; j++)
                {
                    pages[3].playerPanels[j].SetTrigger("Hide");
                }
            }

            if (i == 4){
                for (int j = 0; j < pages[i].playerPanels.Count; j++)
                {
                    pages[1].playerPanels[j].SetTrigger("Hide");
                }
            }

            if (i == 5){
                StartCoroutine(IncreaseCounter());
            }
        }
        
        yield return new WaitForSeconds(5);
        CompleteIntro();
    }

    protected void CompleteIntro()
    {
        gameObject.SetActive(false);
        OnIntroComplete();
    }

    IEnumerator IncreaseCounter(){
        while (counterTime <= 60){
            for (int j = 0; j < pages[5].playerPanels.Count; j++)
            {
                if (pages[5].playerPanels[j].gameObject.activeInHierarchy == true){
                    pages[5].playerPanels[j].gameObject.GetComponentInChildren<TextMeshProUGUI>().text = counterTime.ToString();
                    pages[5].playerPanels[j].gameObject.GetComponentInChildren<TextMeshProUGUI>().color = UIManager.GetPlayerColor(counterTime % 4);
                }
                
                if (counterSound != null)
                PlayerAudioManager.PlayGlobalOneShot(counterSound);
                yield return new WaitForSeconds(0.01f);
            }
            counterTime++;
        }
        
        for (int j = 0; j < pages[5].playerPanels.Count; j++)
            {
                if (pages[5].playerPanels[j].gameObject.activeInHierarchy == true){
                    pages[5].playerPanels[j].gameObject.GetComponentInChildren<TextMeshProUGUI>().color = Color.white;
                }
            }
    }
}
