using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LinearMovement : MonoBehaviour
{
    [SerializeField]
    GameObject target;
    [SerializeField]
    Transform pointA;
    [SerializeField]
    Transform pointB;
    [SerializeField]
    float movementSpeed = 1;
    [SerializeField]
    float initialDelay = 0;

    bool moving = false;

    float lerpValue = 0.0f;
    int moveDir = 1;

    [SerializeField]
    bool pingPong = false;
    [SerializeField]
    bool pauseOnReset = false;

    private void OnEnable()
    {
        moving = false;
        ResetMovement();
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
        if (moving) 
        {
            lerpValue += movementSpeed* moveDir * Time.deltaTime;
            if (pingPong)
            {
                if (lerpValue <= 0.0f)
                {
                    moveDir = 1;
                }
                else if (lerpValue >= 1.0f)
                {
                    moveDir = -1;
                }
            }
            else if (lerpValue >= 1.0f) 
            {
                lerpValue = 0.0f;
                if (pauseOnReset) 
                {
                    moving = false;
                    StartCoroutine(InitialDelayFunc());
                }
            }

            lerpValue = Mathf.Clamp(lerpValue, 0.0f, 1.0f);

            Vector3 position = Vector3.Lerp(pointA.position, pointB.position, lerpValue);
            target.transform.position = position;
        }
    }

    public void ResetMovement() 
    {
        lerpValue = 0.0f;
    }

    public void EnableMovement() 
    {
        moving = true;
    }

    public void DisableMovement() 
    {
        moving = false;
    }

    IEnumerator InitialDelayFunc() 
    {
        yield return new WaitForSeconds(initialDelay);

        EnableMovement();
    }
}
