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

    [SerializeField] private float rotateRange = 45;
    bool isGeneratingLand = true;
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
        if (CheckPosition()) GenerateNextLand();
        CheckForDestroy();
    }

    private void FixedUpdate()
    {
        ProcessMovement(rig.playerSpeed);
    }

    bool CheckPosition()
    {
        float staticZ = gameObject.transform.parent.InverseTransformPoint(staticPosition).z;
        float dynamicZ = gameObject.transform.parent.InverseTransformPoint(dynamicPosition).z;
        Debug.Log(dynamicZ <= staticZ);

        if (dynamicZ <= staticZ) return true;

        return false;
    }

    void GenerateNextLand()
    {
        if (!isGeneratingLand) return;
        isGeneratingLand = false;

        float randomOffset = Random.Range(-rotateRange, rotateRange);
        Quaternion rotation = Quaternion.Euler(0, randomOffset, 0);

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
}
