using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class PlayerRig : MonoBehaviour
{
    // PLAYER
    public enum inputTypes
    {
        primaryFire,
        secondaryFire,
        Directional
    }
    public float playerSpeed;
    [SerializeField] private float playerAcceleration;

    // OBSTACLES
    //The distance obstacles will spawn from the player, in units, through local Z-axis.
    [SerializeField] private int ObstacleSpawnDistance = 30;
    GameObject obstacles;
    [SerializeField] private GameObject prefab1x1;
    [SerializeField] private GameObject prefabLand;
    float timeSinceLastSpawn = 0f;
    float timeSinceLastLand = 0f;
    [SerializeField] private float spawnRate;
    public LayerMask mask;

    void Start()
    {
        obstacles = transform.Find("Obstacles").gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        playerSpeed += playerAcceleration * Time.deltaTime;
        playerSpeed = Mathf.Clamp(playerSpeed, 5, Mathf.Infinity);
        ProcessObjectSpawning();
    }

    public void HandleInput(inputTypes type, Vector2 direction = new Vector2())
    {
        switch (type)
        {
            case inputTypes.primaryFire:
                ProcessMovement(new Vector3(1, 0, 0));
                break;
            case inputTypes.secondaryFire:
                ProcessMovement(new Vector3(-1, 0, 0));
                break;
            case inputTypes.Directional:

                break;
        }

    }

    private void ProcessMovement(Vector3 direction)
    {
        obstacles.transform.localPosition = obstacles.transform.localPosition + direction;
    }

    private void ProcessObjectSpawning()
    {
        if (timeSinceLastSpawn >= 1/spawnRate / playerSpeed)
        {
            //generate random offset
            int offset = Random.Range(-15, 15);
            offset *= 2;
            //get base position
            Vector3 spawnPos = transform.TransformPoint(new Vector3(offset, 0, ObstacleSpawnDistance));

            if (timeSinceLastLand >= 20/spawnRate / playerSpeed)
            {
                SpawnLand(spawnPos);
            }
            else SpawnSingleObject(spawnPos);

            timeSinceLastSpawn = 0;
        }
        timeSinceLastSpawn += Time.deltaTime;
        timeSinceLastLand += Time.deltaTime;
    }
    private void SpawnSingleObject(Vector3 spawnPos)
    {
        Instantiate(prefab1x1, spawnPos, transform.rotation, obstacles.transform);
    }
    private void SpawnLand(Vector3 spawnPos)
    {
        timeSinceLastLand = 0;
        //Convert to local space and apply landmass offset
        Vector3 leftPos = obstacles.transform.InverseTransformPoint(spawnPos) - new Vector3(-27.5f, 0, 0);
        Vector3 rightPos = obstacles.transform.InverseTransformPoint(spawnPos) - new Vector3(27.5f, 0, 0);
        //Convert back to world space
        leftPos = obstacles.transform.TransformPoint(leftPos);
        rightPos = obstacles.transform.TransformPoint(rightPos);

        Instantiate(prefabLand, leftPos, transform.rotation, obstacles.transform);
        Instantiate(prefabLand, rightPos, transform.rotation, obstacles.transform);
    }
}
