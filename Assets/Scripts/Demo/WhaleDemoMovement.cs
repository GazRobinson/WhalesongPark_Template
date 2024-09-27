using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WhaleDemoMovement : MonoBehaviour
{

    public float speed = 1.0f;
    private void Update()
    {
        transform.Translate(Vector2.right * speed * Time.deltaTime);
    }
}
