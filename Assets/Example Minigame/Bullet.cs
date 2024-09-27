using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float m_BulletSpeed = 10.0f;


    public void Fire(Vector2 position)
    {
        transform.position = position;
        gameObject.SetActive(true);
    }

    private void Update()
    {
        transform.position += Vector3.up * m_BulletSpeed * Time.deltaTime;

        if(!ScreenUtility.IsOnScreen(transform.position, -1))
        {
            Debug.Log("Bullet is off screen!");
            gameObject.SetActive(false);
        }
    }
}
