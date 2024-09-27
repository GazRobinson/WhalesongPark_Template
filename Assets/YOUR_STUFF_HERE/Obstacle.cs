using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Obstacle : MonoBehaviour
{
    Rigidbody rb;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void FixedUpdate()
    {
        ProcessMovement(1f);
    }

    private void ProcessMovement(float playerVelocity)
    {
        rb.velocity = new Vector3(0, 0, -playerVelocity);
    }

    private void OnCollisionEnter(Collision collision)
    {
        // Collision, player velocity reduced, score malice etc...
    }

    private void CheckForDestroy()
    {
        //Check to see if passed the player's Z point, if so destroy object.
    }
}
