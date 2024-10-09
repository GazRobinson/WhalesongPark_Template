using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dial_Ui_Setter : MonoBehaviour
{
    const float MinSpeed = 5;
    const float MaxSpeed = 20;

    float SpeedDif;

    private void Start()
    {
        SpeedDif = MaxSpeed - MinSpeed;
    }

    public void SetSpeed(float NewSpeed)
    {
        float NewZAngle = 180.0f * ((NewSpeed - MinSpeed) / SpeedDif);
        transform.rotation = Quaternion.Euler(new Vector3(0, 0, NewZAngle));
    }
}
