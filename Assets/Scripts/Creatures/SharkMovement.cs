using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SharkMovement : MonoBehaviour
{
    [SerializeField] private float rayDistance;
    [SerializeField] private LayerMask collisionMask;

    ScreenWrapped screenWrapper;

    private Rigidbody2D rb;

    float horizontalInput = 0.0f;
    float verticalInput = 0.0f;

    public float moveForce = 4;
    public float waveFrequency = 1.0f;
    public float waveAmplitude = 0.5f;

    public float torque = 1.25f;

    Vector3 point;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        screenWrapper = GetComponentInParent<ScreenWrapped>();
    }

    private void Update()
    {
        Vector2 rightForward = (transform.up + transform.right * 0.25f).normalized;
        Debug.DrawRay(transform.position, rightForward * rayDistance);

        Vector2 leftForward = (transform.up + transform.right * -0.25f).normalized;
        Debug.DrawRay(transform.position, leftForward * rayDistance);

        Vector2 rightFlank = (transform.up + transform.right * 0.5f).normalized;
        Debug.DrawRay(transform.position, rightFlank * rayDistance);

        Vector2 leftFlank = (transform.up + transform.right * -0.5f).normalized;
        Debug.DrawRay(transform.position, leftFlank * rayDistance);

        Vector2 rightSide = (transform.up * 0.75f + transform.right).normalized;
        Debug.DrawRay(transform.position, rightSide * rayDistance * 0.5f);

        Vector2 leftSide = (transform.up * 0.75f + -transform.right).normalized;
        Debug.DrawRay(transform.position, leftSide * rayDistance * 0.5f);

        if ((Physics2D.Raycast(transform.position, rightForward, rayDistance, collisionMask) ||
            Physics2D.Raycast(transform.position, rightFlank, rayDistance, collisionMask) ||
            Physics2D.Raycast(transform.position, rightSide, rayDistance * 0.5f, collisionMask)) && screenWrapper.initialised)
        {
            horizontalInput = -1;
        }
        else if ((Physics2D.Raycast(transform.position, leftForward, rayDistance, collisionMask) ||
            Physics2D.Raycast(transform.position, leftFlank, rayDistance, collisionMask) ||
            Physics2D.Raycast(transform.position, leftSide, rayDistance * 0.5f, collisionMask)) && screenWrapper.initialised)
        {
            horizontalInput = 1;
        }
        else 
        {
            horizontalInput = waveAmplitude * Mathf.Sin(Time.time * waveFrequency);
        }

        Vector2 moveVector = new Vector2(0.0f, verticalInput);
        rb.AddForce(transform.up * 1.0f * moveForce);
        rb.AddTorque(-horizontalInput * 0.4f);
    }
}
