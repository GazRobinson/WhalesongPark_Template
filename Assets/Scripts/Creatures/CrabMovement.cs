using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrabMovement : MonoBehaviour
{
    [SerializeField]
    GameObject crabSprite;
    [SerializeField]
    Transform pointA;
    [SerializeField]
    Transform pointB;

    [SerializeField]
    float moveSpeed = 1.0f;

    Animator crabAnimator;

    int moveDirection = 1;

    float lerpValue = 0.0f;

    bool pausedMovement = false;

    private void Awake()
    {
        crabAnimator = crabSprite.GetComponent<Animator>();
    }

    private void Update()
    {
        if (!pausedMovement)
        {
            // Add to the current lerp value with crab move speed
            lerpValue += moveDirection * moveSpeed * Time.deltaTime;
            if (lerpValue >= 1.0f)
                moveDirection = -1;
            else if (lerpValue <= 0.0f)
                moveDirection = 1;

            lerpValue = Mathf.Clamp(lerpValue, 0.0f, 1.0f);
            //  Calculate position from lerp and apply to sprite gameobject
            Vector2 position = Vector2.Lerp(pointA.position, pointB.position, lerpValue);
            crabSprite.transform.position = position;
        }

        // Change animation blend parameter using move direction
        if (pausedMovement) 
        {
            crabAnimator.SetFloat("WalkDirection", 0.0f);
        }
        else
        {
            crabAnimator.SetFloat("WalkDirection", moveDirection);
        }
    } 

    // Start crab movement pause coroutines
    private void OnEnable()
    {
        StartCoroutine(DelayBetweenMovementPauses());
    }

    // Disable crab movement coroutines when script disabled
    private void OnDisable()
    {
        StopAllCoroutines();
    }

    IEnumerator PauseMovementForTime() 
    {
        // Pause movement for random time in range
        pausedMovement = true;
        float randomPauseTime = Random.Range(2.0f, 5.0f);
        yield return new WaitForSeconds(randomPauseTime);

        // Randomly change the move direction of the crab after pausing 25% of the time
        float changeDirectionChance = Random.Range(0, 100.0f);
        if (changeDirectionChance >= 75.0f) 
        {
            if ((moveDirection == 1 && lerpValue <= 0.75f)
                || (moveDirection == -1 && lerpValue >= 0.25f))
            {               
                moveDirection *= -1;
            }
        }

        pausedMovement = false;
        StartCoroutine(DelayBetweenMovementPauses());
    }

    // Delay between movement pauses for a random time in range
    IEnumerator DelayBetweenMovementPauses() 
    {
        float randomDelayTime = Random.Range(2.0f, 5.0f);
        yield return new WaitForSeconds(randomDelayTime);
        StartCoroutine(PauseMovementForTime());
    }
}
