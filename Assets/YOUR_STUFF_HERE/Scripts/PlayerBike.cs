using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBike : MonoBehaviour
{

    float bikePower = 0;
    public float maxBikePower = 5;
    public float buttonPressPower = 1f;
    public float bikePowerDecayRate = 0.01f;
    public int m_ScreenID = -1;

    // Start is called before the first frame update
    void Start()
    {
    
    }

    // Update is called once per frame
    void Update()
    {
            //Vector2 moveDirection = Vector2.up;
            //moveDirection.x = 0; //We don't want to move left and right
            transform.position += Vector3.up * bikePower * Time.deltaTime; // Time.deltaTime makes our movement consistent regardless of framerate
            //transform.position = ScreenUtility.ClampToScreen(transform.position, m_ScreenID, 0.5f);

       
    }
    private void LateUpdate()
    {
        bikePower = Mathf.MoveTowards(bikePower, 0, bikePowerDecayRate);
        Debug.Log(bikePower);
    }

    public void HandleButtonInput(int buttonID)
    {
        bikePower += (buttonPressPower / (bikePower / 12));
        bikePower = Mathf.Clamp(bikePower, 0, maxBikePower);
    }
}
