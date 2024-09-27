using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class TutorialStage
{
    public List<Animator> playerPanels;
}

public class StagedTutorialController : TutorialController
{
    [SerializeField] private List<TutorialStage> pages;

    [SerializeField] private AudioClip pageShowSound;

    protected override void DoTutorial()
    {
        for (int i = 0; i < pages.Count; i++)
        {
            for (int j = 0; j < pages[i].playerPanels.Count; j++)
            {
                //TODO: This feels like it would get out of sync
                pages[i].playerPanels[j].SetTrigger("Reset");
            }
        }
        StartCoroutine(PlayStages());
    }

    private IEnumerator PlayStages()
    {
        float t = TutorialDuration / pages.Count;

        for(int i=0; i < pages.Count; i++)
        {
            
            for (int j = 0; j < pages[i].playerPanels.Count; j++)
            {
                pages[i].playerPanels[j].gameObject.SetActive(true);
                pages[i].playerPanels[j].SetTrigger("Show");
                // play sound effect
                if (pageShowSound != null)
                PlayerAudioManager.PlayOneShot(j, pageShowSound, 1, 1 + ((float)i/10.0f));
                
            }
            yield return new WaitForSeconds(t);
        }
        
        CompleteTutorial();
    }
}
