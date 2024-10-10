using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LandGenerator : MonoBehaviour
{
    Rigidbody rb;
    PlayerRig rig;

    [SerializeField] private GameObject landPrefab;
    [SerializeField] private GameObject bottomChecker;
    [SerializeField] private GameObject topChecker;

    private Vector3 staticPosition;
    private Vector3 dynamicPosition;
    private float positionsDistance = 0;

    [SerializeField] private float rotateRange = 45;
    bool isGeneratingLand = true;
    [SerializeField] LayerMask layerMask1;
    //DEBUG
    Vector3 sphereLocation;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rig = transform.parent.parent.GetComponent<PlayerRig>();
        staticPosition = bottomChecker.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        dynamicPosition = topChecker.transform.position;
        if (isGeneratingLand && CheckPosition()) GenerateNextLand();
        CheckForDestroy();
        if (positionsDistance == 0) positionsDistance = Vector3.Distance(staticPosition, dynamicPosition);
    }

    private void FixedUpdate()
    {
        ProcessMovement(rig.playerSpeed);
    }

    bool CheckPosition()
    {
        float staticZ = gameObject.transform.parent.InverseTransformPoint(staticPosition).z;
        float dynamicZ = gameObject.transform.parent.InverseTransformPoint(dynamicPosition).z;

        if (dynamicZ <= staticZ) return true;

        return false;
    }

    bool CheckGenerationValidity(Vector3 spawnPosition, float rotation)
    {
        //Get Theta
        float trueRotation = rotation % 360;
        if (trueRotation < 0) trueRotation += 360;
        //Get r
        float r = positionsDistance / 2;
        //Convert to radians
        trueRotation = trueRotation * MathF.PI / 180;
        //Convert from polar to cartesian
        float x = r*Mathf.Sin(trueRotation);
        float z = r*Mathf.Cos(trueRotation);

        //Add coordinates to spawn position to find where prospective land generation will end up.
        Vector3 nextStaticPosition = new Vector3(spawnPosition.x + x, spawnPosition.y, spawnPosition.z + z);
        Collider[] collisions = Physics.OverlapSphere(nextStaticPosition, 15, layerMask1, QueryTriggerInteraction.Collide);
        sphereLocation = nextStaticPosition;
        foreach (Collider collision in collisions)
        {
            if (collision.tag == "Land" && collision.transform.parent != gameObject.transform) return false;
        }
        return true;
    }
    void GenerateNextLand()
    {
        if (!isGeneratingLand) return;

        float randomOffset = UnityEngine.Random.Range(-rotateRange, rotateRange);
        Quaternion rotation = Quaternion.Euler(0, randomOffset, 0);

        if (!CheckGenerationValidity(dynamicPosition, randomOffset))
        {
            Debug.Log("Land gen failed");
            return;
        }
        isGeneratingLand = false;
        Instantiate(landPrefab, dynamicPosition, rotation, transform.parent);
    }

    private void ProcessMovement(float playerVelocity)
    {
        // transform the world forward into local space:
        Vector3 relative = new Vector3(0, 0, -playerVelocity);
        Vector3 worldVelocity = transform.parent.TransformDirection(relative);
        rb.velocity = worldVelocity;
    }

    private void CheckForDestroy()
    {
        //Check to see if passed the player's Z point, if so destroy object.
        if (transform.localPosition.z <= -5)
        {
            Destroy(gameObject);
        }
    }
    private void OnDrawGizmos()
    {
        //Gizmos.DrawSphere(sphereLocation, 15);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag == "Player")
        {
            if (rig.playerSpeed > 5)
            {
                if (rig.playerSpeed - 0.05f > 5)
                {
                    rig.playerSpeed -= 0.05f;
                }
                else
                {
                    rig.playerSpeed = 5;
                }
            }
        }
    }
}
