using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoatMovement : MonoBehaviour
{
    [SerializeField]
    Transform pointA;
    [SerializeField]
    Transform pointB;
    [SerializeField]
    GameObject target;

    /* [SerializeField]
    float movementSpeed = 1; */
    [SerializeField]
    float initialDelay = 0;

    [SerializeField]
    bool duplicate = false;

    bool moving = false;

    float lerpValue = 0.0f;
    int moveDir = 1;

    int laps = 0;

    public NetMinigame minigameManager;

    [SerializeField]
    LineRenderer netRopeLine;
    [SerializeField]
    Transform boatRopePoint;
    [SerializeField]
    Transform netRopePoint;

    [SerializeField] AudioSource boatAudio;

    private float uiTimer;
    private float maxTime = 30.0f;

    [SerializeField] private List<Transform> screenPositions = new List<Transform>();

    int quadrant = 0;

    private void Awake()
    {
        // Bind functions to minigame controller event system
        minigameManager.MinigameLoaded.AddListener(OnMinigameLoaded);
        minigameManager.MinigameStart.AddListener(OnPlay);
        minigameManager.MinigameFinished.AddListener(OnMinigameComplete);
        //minigameManager.MinigamePause.AddListener(DisableMovement);
        //minigameManager.MinigameResume.AddListener(EnableMovement);

        if (duplicate) 
        {
            target.SetActive(false);
        }
    }

    private void OnMinigameLoaded()
    {
        // Reset lerp value and boats position to Point A
        lerpValue = 0.0f;
        Vector3 position = Vector3.Lerp(pointA.position, pointB.position, lerpValue);
        target.transform.position = position;
        moving = false;
        // Hide boat if duplicate at end of screen
        if (duplicate)
        {
            target.SetActive(false);
        }
    }

    private void OnMinigameComplete() 
    {
        // Reset lerp value and boats position to Point A
        lerpValue = 0.0f;
        Vector3 position = Vector3.Lerp(pointA.position, pointB.position, lerpValue);
        target.transform.position = position;
        moving = false;
        // Hide boat if duplicate at end of screen
        if (duplicate)
        {
            target.SetActive(false);
        }

        boatAudio.Stop();
    }

    public void OnPlay()
    {
        moving = false;
        lerpValue = 0.0f;
        uiTimer = maxTime;
        // If a delay value has been assigned pause movement until time passed
        if (initialDelay > 0)
        {
            StartCoroutine(InitialDelayFunc());
        }
        else
        {
            EnableMovement();
        }
    }

    private void Update()
    {
        UIManager.SetTimer(uiTimer / maxTime);
        uiTimer -= Time.deltaTime;

        if (moving)
        {
            if (netRopePoint.transform.position.x > screenPositions[quadrant].position.x)
            {
                quadrant++;
                if (quadrant > 3)
                {
                    quadrant = 0;
                }
            }

            // move the boat
            lerpValue += minigameManager.boatMovementSpeed * Time.deltaTime;            
            
            if (lerpValue >= 1.0f)
            {
                lerpValue = 0.0f;
                laps++;

                // Below is the code that makes the boat always do 2 loop and then end the minigame
                // This has been removed as it allows the boat to do infinite loops - and the game based on time

                // If exceeded two laps trigger end of minigame event
                //EndGame();
            }
            // Show duplicate ship after midway through first run
            if (lerpValue > 0.5f && duplicate)
            {
                target.SetActive(true);
            }
            // Update position
            Vector3 position = Vector3.Lerp(pointA.position, pointB.position, lerpValue);
            target.transform.position = position;

            if (netRopeLine != null)
            {
                // Update rope renderer sprite with new boat and net postions
                if (netRopeLine.positionCount == 2)
                {
                    netRopeLine.SetPosition(0, netRopePoint.position);
                    netRopeLine.SetPosition(1, boatRopePoint.position);
                }
            }
        }
        else 
        {
            if (netRopeLine != null)
            {
                // Update rope to hide when not moving
                if (netRopeLine.positionCount == 2)
                {
                    netRopeLine.SetPosition(0, Vector3.zero);
                    netRopeLine.SetPosition(1, Vector3.zero);
                }
            }
        }
    }

    private void EndGame()
    {
        if (laps >= 2 && !duplicate)
        {
            moving = false;
            StartCoroutine(PauseBeforeEndMinigame());
        }
    }

    public void EnableMovement()
    {
        Debug.Log("Enable boat movement");
        moving = true;
        boatAudio.Play();
    }

    public void DisableMovement()
    {
        moving = false;
        boatAudio.Stop();
    }

    // Delay for fixed time before beginning movement
    IEnumerator InitialDelayFunc()
    {
        yield return new WaitForSeconds(initialDelay);
        EnableMovement();
    }

    // Delay for fixed time before ending minigame and triggering win state
    IEnumerator PauseBeforeEndMinigame() 
    {
        yield return new WaitForSeconds(5.0f);
        //minigameManager.wonMinigame = true;
        minigameManager.OnMinigameFinished();
    }
}
