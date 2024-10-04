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

    void Update()
    {
        transform.position += Vector3.up * bikePower * Time.deltaTime; 
    }
    private void LateUpdate()
    {
        bikePower = Mathf.MoveTowards(bikePower, 0, bikePowerDecayRate);
        //Debug.Log(bikePower);
    }

    public void HandleButtonInput(int buttonID)
    {
        bikePower += (buttonPressPower / (bikePower / 12));
        bikePower = Mathf.Clamp(bikePower, 0, maxBikePower);
    }
}
