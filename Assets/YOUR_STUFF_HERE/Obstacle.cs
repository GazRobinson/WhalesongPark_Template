using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Obstacle : MonoBehaviour
{
    Rigidbody rb;
    PlayerRig rig;
    [SerializeField] private float SpeedReductionOnHit = 5;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rig = transform.parent.parent.GetComponent<PlayerRig>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void FixedUpdate()
    {
        ProcessMovement(rig.playerSpeed);
        CheckForDestroy();
    }

    private void ProcessMovement(float playerVelocity)
    {
        // transform the world forward into local space:
        Vector3 relative = new Vector3(0, 0, -playerVelocity);
        Vector3 worldVelocity = transform.TransformDirection(relative);
        rb.velocity = worldVelocity;
    }

    private void OnCollisionEnter(Collision collision)
    {
        // Collision, player velocity reduced, score malice etc...
        if (collision.gameObject.tag == "Player")
        {
            rig.playerSpeed -= SpeedReductionOnHit;
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Land" && gameObject.tag != "Land")
        {
            Destroy(gameObject);
        }
    }

    private void CheckForDestroy()
    {
        //Check to see if passed the player's Z point, if so destroy object.
        if(transform.localPosition.z <= -5)
        {
            Destroy(gameObject);
        }
    }
}
