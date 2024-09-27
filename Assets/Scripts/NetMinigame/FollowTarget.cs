using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowTarget : MonoBehaviour
{
    public NetMinigame minigameManager;

    public int playerIndex;

    public Transform playerFollowTarget;
    public float moveSpeed = 50.0f;
    public float fishMoveSpeed = 4;

    FishBounce[] fishControllers;

    bool facingRight = true;

    float horizontalInput = 0.0f;
    float verticalInput = 0.0f;

    Vector2 originalTargetPosition;

    private void Awake()
    {
        fishControllers = GetComponentsInChildren<FishBounce>();
        originalTargetPosition = playerFollowTarget.position;
        
    }

    private void Start()
    {
        // Bind minigame events
        minigameManager.MinigameLoaded.AddListener(OnMinigameLoadedStarted);
        minigameManager.MinigameStart.AddListener(OnMinigameLoadedStarted);
        minigameManager.MinigameFinished.AddListener(OnMinigameLoadedStarted);
    }

    // On minigame loaded or started, reset fish school positions
    private void OnMinigameLoadedStarted() 
    {
        //playerFollowTarget.position = originalTargetPosition;
        //transform.position = originalTargetPosition;
        for (int i = 0; i < fishControllers.Length; i++) 
        {
            if (fishControllers[i].gameObject != null) 
            {
                fishControllers[i].gameObject.SetActive(true);
                fishControllers[i].ResetAnimation();
            }
            fishControllers[i].transform.SetParent(transform.parent);
        }
        fishControllers[0].SetOriginFish();

        facingRight = true;
    }

    public void ProvideInput(Vector2 newInput)
    {
        horizontalInput = newInput.x;
        verticalInput = newInput.y;
    }

    private void Update()
    {
        Vector2 moveDir = (Vector2)(playerFollowTarget.position - transform.position);

        moveDir.Normalize();
        moveDir *= moveSpeed * Time.deltaTime;

        transform.position = Vector2.MoveTowards(transform.position, playerFollowTarget.position, moveSpeed * Time.deltaTime);

        // Play flip animations if input direction has changed
        if (moveDir.x < -0.02f && facingRight)
        {
            StartCoroutine(FlipLeft());
            
            
        }
        else if (moveDir.x > 0.02f && !facingRight)
        {
            StartCoroutine(FlipRight());
        }

        if (minigameManager.m_IsMinigameActive)
        {
            Vector2 moveVector = new Vector2(horizontalInput, verticalInput);
            playerFollowTarget.transform.Translate(moveVector * moveSpeed * Time.deltaTime);
            
            // have each fish follow the follow target with a slight delay and random offset
            for (int i = 0; i < fishControllers.Length; i++)
            {
                fishControllers[i].transform.position = Vector2.MoveTowards(fishControllers[i].transform.position, playerFollowTarget.position + fishControllers[i].origianOffset + new Vector3(fishControllers[i].bounceAmplitude, fishControllers[i].bounceAmplitude), fishControllers[i].bounceSpeed * Time.deltaTime);
            }
        }
    }

    IEnumerator FlipLeft() 
    {
        facingRight = false;
        for (int i = 0; i < fishControllers.Length; i++) 
        {
            fishControllers[i].FlipLeft();
            yield return new WaitForSeconds(0.01f);
        }


    }

    IEnumerator FlipRight()
    {
        facingRight = true;
        for (int i = 0; i < fishControllers.Length; i++)
        {
            fishControllers[i].FlipRight();
            yield return new WaitForSeconds(0.01f);
        }
    }

}
