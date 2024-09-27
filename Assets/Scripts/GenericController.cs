using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenericController : MonoBehaviour
{
    public float moveSpeed = 2.0f;
    Rigidbody2D rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        Vector2 moveDir = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        rb.velocity = moveDir * moveSpeed;
    }
}
