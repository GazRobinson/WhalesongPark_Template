using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFish : MonoBehaviour
{
    public System.Func<Bullet> GetBullet;
    public float FishSpeed = 2.0f;
    public int m_ScreenID = -1;
    private Vector2 inputDirection = Vector2.zero;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 moveDirection = inputDirection;
        moveDirection.y = 0; //We don't want to move up and down
        transform.position += (Vector3)moveDirection * FishSpeed * Time.deltaTime; // Time.deltaTime makes our movement consistent regardless of framerate
        transform.position = ScreenUtility.ClampToScreen(transform.position, m_ScreenID, 0.5f);
    }

    public void HandleDirectionalInput(Vector2 direction)
    {
        //Save the direciton to use later
        inputDirection = direction;
    }
    public void HandleButtonInput(int buttonID)
    {
        if (buttonID == 0)
        {
            Bullet bullet = GetBullet();
            if (bullet != null)
            {
                bullet.Fire(transform.position);
            }
        }
    }
}
