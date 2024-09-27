using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishBounce : MonoBehaviour
{
    public float maxBounceRadius = 0.25f;
    public bool caught = false;

    // Attributes for the sin wave used for the fish bouncing
    public float bounceAmplitude;
    public float bounceSpeed;
    public float startOffset;

    public Vector3 origianOffset;
    Animator animator;

    [SerializeField]
    NetMinigame netMinigame;

    private bool originFish = false;

    private void Awake()
    {
        animator = GetComponent<Animator>();

        // Randomly generate all sin wave bounce attributes
        bounceAmplitude = Random.Range(0.1f, maxBounceRadius);
        bounceSpeed = Random.Range(1f, 2f);
        //bounceSpeed = Random.Range(1.0f, 3.0f);
        startOffset = Random.Range(0.0f, 1.0f);

        origianOffset = transform.localPosition;
    }

    private void Start()
    {
        animator.speed = Random.Range(0.85f, 1.15f);
        netMinigame.MinigameLoaded.AddListener(ResetFish);
    }

    private void Update()
    {
        if (!caught)
        {
            // Calculate vertical bounce using a sin wave using gameplay time
            // TODO investigate if Time.time needs to be reset or can run for multiple days
            //float verticalBounce = bounceAmplitude * Mathf.Sin(Time.time * bounceSpeed + startOffset);
            //transform.localPosition = origianOffset + new Vector3(0.0f, verticalBounce, 0.0f);
        }
    }

    public void ResetFish() 
    {
        caught = false;
    }

    // Animator triggers to flip sprite via animation
    public void FlipLeft() 
    {
        animator.SetTrigger("FlipLeft");
    }

    public void FlipRight() 
    {
        animator.SetTrigger("FlipRight");
    }

    public void ResetAnimation() 
    {
        animator.SetTrigger("Reset");
    }

    public void SetOriginFish(bool b = true) 
    {
        originFish = b;
    }

    public bool IsOriginFish() 
    {
        return originFish;
    }
}
