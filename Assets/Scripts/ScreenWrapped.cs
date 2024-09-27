using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenWrapped : MonoBehaviour
{
    Camera mainCamera;

    Vector2 screenTopRight;
    Vector2 screenBottomLeft;

    Bounds bounds;

    public int instanceCount = 1;

    public ScreenWrapped parentInstance;
    public ScreenWrapped spawnedDuplicateInstance;

    public Rigidbody2D controllerRB;

    public bool initialised = false;

    private void Awake()
    {
        instanceCount = 1;

        mainCamera = Camera.main;

        screenTopRight = mainCamera.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, 0));
        screenBottomLeft = mainCamera.ScreenToWorldPoint(new Vector3(0, 0, 0));
    }

    private void Update()
    {
        bounds.size = Vector3.zero;
        Collider2D[] colliders = GetComponentsInChildren<Collider2D>();
        foreach (Collider2D col in colliders)
        {
            bounds.Encapsulate(col.bounds);
        }

        if (instanceCount <= 1 && initialised)
        {
            if (bounds.max.x > screenTopRight.x)
            {
                // Right side of screen
                float distanceToEdge = screenTopRight.x - transform.position.x;
                Vector3 spawnPosition = new Vector3(screenBottomLeft.x - distanceToEdge, transform.position.y, 0.0f);
                GameObject duplicateShark = Instantiate(this.gameObject, spawnPosition, transform.rotation) as GameObject;
                duplicateShark.transform.parent = transform.parent;
                spawnedDuplicateInstance = duplicateShark.GetComponent<ScreenWrapped>();
                spawnedDuplicateInstance.controllerRB.velocity = controllerRB.velocity;
                spawnedDuplicateInstance.parentInstance = this;
                spawnedDuplicateInstance.instanceCount += 1;
                instanceCount += 1;
            }
            else if (bounds.min.x < screenBottomLeft.x)
            {
                // Left side of screen
                float distanceToEdge = transform.position.x - screenBottomLeft.x;
                Vector3 spawnPosition = new Vector3(screenTopRight.x + distanceToEdge, transform.position.y, 0.0f);
                GameObject duplicateShark = Instantiate(this.gameObject, spawnPosition, transform.rotation) as GameObject;
                duplicateShark.transform.parent = transform.parent;
                spawnedDuplicateInstance = duplicateShark.GetComponent<ScreenWrapped>();
                spawnedDuplicateInstance.controllerRB.velocity = controllerRB.velocity;
                spawnedDuplicateInstance.parentInstance = this;
                spawnedDuplicateInstance.instanceCount += 1;
                instanceCount += 1;
            }
            else if (bounds.max.y > screenTopRight.y)
            {
                // Top of screen
                float distanceToEdge = screenTopRight.y - transform.position.y;
                Vector3 spawnPosition = new Vector3(transform.position.x, screenBottomLeft.y - distanceToEdge, 0.0f);
                GameObject duplicateShark = Instantiate(this.gameObject, spawnPosition, transform.rotation) as GameObject;
                duplicateShark.transform.parent = transform.parent;
                spawnedDuplicateInstance = duplicateShark.GetComponent<ScreenWrapped>();
                spawnedDuplicateInstance.controllerRB.velocity = controllerRB.velocity;
                spawnedDuplicateInstance.parentInstance = this;
                spawnedDuplicateInstance.instanceCount += 1;
                instanceCount += 1;
            }
            else if (bounds.min.y < screenBottomLeft.y)
            {
                // Bottom of screen
                float distanceToEdge = transform.position.y - screenBottomLeft.y;
                Vector3 spawnPosition = new Vector3(transform.position.x, screenTopRight.y + distanceToEdge, 0.0f);
                GameObject duplicateShark = Instantiate(this.gameObject, spawnPosition, transform.rotation) as GameObject;
                duplicateShark.transform.parent = transform.parent;
                spawnedDuplicateInstance = duplicateShark.GetComponent<ScreenWrapped>();
                spawnedDuplicateInstance.controllerRB.velocity = controllerRB.velocity;
                spawnedDuplicateInstance.parentInstance = this;
                spawnedDuplicateInstance.instanceCount += 1;
                instanceCount += 1;
            }
        }

        if (initialised && (bounds.min.x > screenTopRight.x || bounds.max.x < screenBottomLeft.x ||
            bounds.min.y > screenTopRight.y || bounds.max.y < screenBottomLeft.y)) 
        {
            if (spawnedDuplicateInstance != null)
            {
                spawnedDuplicateInstance.instanceCount -= 1;
            }
            if (parentInstance != null) 
            {
                parentInstance.instanceCount -= 1;
            }
            GameObject.Destroy(this.gameObject);
        }

        if (bounds.min.x > screenBottomLeft.x && bounds.min.y > screenBottomLeft.y &&
            bounds.max.x < screenTopRight.x && bounds.max.y < screenTopRight.y) 
        {
            initialised = true;
        }
    }

    private void OnDrawGizmos()
    {
        if (bounds != null) 
        {
            Gizmos.color = Color.green;
            Gizmos.DrawSphere(bounds.min, 0.05f);
            Gizmos.DrawSphere(bounds.max, 0.05f);
        }
    }
}
