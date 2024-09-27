using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LimitBounds : MonoBehaviour
{
    public float minX;
    public float maxX;

    public float minY;
    public float maxY;

    private void Update()
    {
        Vector3 position = transform.position;

        if (position.x < minX)
            position.x = minX;
        else if (position.x > maxX)
            position.x = maxX;

        if (position.y < minY)
            position.y = minY;
        else if (position.y > maxY)
            position.y = maxY;

        transform.position = position;
    }

    private void OnDrawGizmos()
    {
        Vector3 centre = new Vector3(
            (maxX - minX) / 2.0f + minX,
            (maxY - minY) / 2.0f + minY,
            0.0f);

        Vector3 bounds = new Vector3(
            maxX - minX,
            maxY - minY,
            0.0f);

        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(centre, bounds);
    }
}
