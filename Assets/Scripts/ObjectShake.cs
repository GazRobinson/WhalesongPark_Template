using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectShake : MonoBehaviour
{
    Vector3 origin;
    public Quaternion originalRotation;
    private float shakeTimeRemaining, shakePower, shakeFadeTime, shakeRotation, rotationMultiplier, moveToOriginSpeed;

    private void Awake()
    {
        origin = transform.position;
    }

    private void LateUpdate()
    {
        if (shakeTimeRemaining > 0)
        {
            shakeTimeRemaining -= Time.deltaTime;
            float xAmount = UnityEngine.Random.Range(-1f, 1f) * shakePower;
            float zAmount = UnityEngine.Random.Range(-1f, 1f) * shakePower;

            transform.position = origin + new Vector3(xAmount, 0f, zAmount);

            shakePower = Mathf.MoveTowards(shakePower, 0f, shakeFadeTime * Time.deltaTime);

            shakeRotation = Mathf.MoveTowards(shakeRotation, 0f, shakeFadeTime * rotationMultiplier * Time.deltaTime);

        }
        else if (shakeTimeRemaining <= 0)
        {
            transform.position = Vector3.MoveTowards(transform.position, origin, Time.deltaTime * moveToOriginSpeed);
        }

        transform.rotation = Quaternion.Euler(originalRotation.x, 0, shakeRotation * UnityEngine.Random.Range(-1f, 1f));
    }

    public void StartShake(float length, float power, float rotationMulti, float moveToSpeed)
    {
        moveToOriginSpeed = moveToSpeed;
        rotationMultiplier = rotationMulti;
        shakeTimeRemaining = length;
        shakePower = power;

        shakeFadeTime = power / length;

        shakeRotation = power * rotationMultiplier;
    }

    public void StopShake() 
    {
        shakeTimeRemaining = 0.0f;
    }
}
