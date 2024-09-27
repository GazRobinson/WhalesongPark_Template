using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class GridSnapEditor : MonoBehaviour
{

    public float snapValue = 0.5f;

    void Update()
    {
        if (!Application.isPlaying)
        {
            if (snapValue != 0)
                transform.position = new Vector3(Mathf.Round(transform.position.x * (1 / snapValue)) / (1 / snapValue), transform.position.y, transform.position.z);

            if (snapValue != 0)
                transform.position = new Vector3(transform.position.x, Mathf.Round(transform.position.y * (1 / snapValue)) / (1 / snapValue), transform.position.z);

            if (snapValue != 0)
                transform.position = new Vector3(transform.position.x, transform.position.y, Mathf.Round(transform.position.z * (1 / snapValue)) / (1 / snapValue));
        }
    }
}
