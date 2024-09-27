using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialController : MonoBehaviour
{
    public System.Action OnTutorialComplete;

    [SerializeField] protected float TutorialDuration = 8.0f;
    private Animator m_Animator;
    private bool m_Initialised = false;
    private void Start()
    {
        Initialise();
    }

    private void Initialise()
    {
        if (!m_Initialised)
        {
            gameObject.SetActive(true);
            m_Animator = transform.GetComponentInChildren<Animator>();

            if (m_Animator)
            {
                TutorialDuration = m_Animator.GetCurrentAnimatorClipInfo(0)[0].clip.length;
            }

            CopyTutorial();
            m_Initialised = true;
        } 
    }
    private void CopyTutorial()
    {
        float xPos = m_Animator.GetComponent<RectTransform>().anchoredPosition.x;
        for (int i = 0; i < 3; i++)
        {
            GameObject tutorialCopy = Instantiate(m_Animator.gameObject, transform);
            RectTransform rt = tutorialCopy.GetComponent<RectTransform>();
            xPos += 336.0f; //Screen size over 4
            Vector3 pos = rt.anchoredPosition;
            pos.x = xPos;
            rt.anchoredPosition = pos;
            PlayerColoredSprite[] colours = tutorialCopy.GetComponentsInChildren<PlayerColoredSprite>();
            foreach (PlayerColoredSprite colour in colours)
            {
                colour.SetColour(i + 1);
            }
            tutorialCopy.GetComponentInChildren<BlockWave>().colour = PlayerUtilities.GetPlayerColor(i+1);
        }
    }
    public void Begin(System.Action callback = null)
    {
        Initialise();
        /*if (GameManager.SkipTutorials)
            TutorialDuration = 1.0f;*/

        OnTutorialComplete = callback;
        gameObject.SetActive(true);
        DoTutorial();
    }

    protected virtual void DoTutorial()
    {
        StartCoroutine(DelayForTime(TutorialDuration, () =>
        {
            CompleteTutorial();
        }));
    }

    protected IEnumerator DelayForTime(float delay, System.Action callback = null)
    {
        yield return new WaitForSeconds(delay);
        if (callback != null)
        {
            callback();
        }
    }

    protected void CompleteTutorial()
    {
        gameObject.SetActive(false);
        OnTutorialComplete();
    }
}
