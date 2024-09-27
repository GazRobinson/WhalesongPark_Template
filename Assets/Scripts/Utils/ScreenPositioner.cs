using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenPositioner : MonoBehaviour
{
    public Vector2 ScreenPos = Vector2.zero;
    public Vector2 Offset = Vector2.zero;
    // Start is called before the first frame update
    void Awake()
    {
        Vector3 pos = ScreenUtility.ViewToWorld(ScreenPos);
        transform.position = pos + (Vector3)Offset;
        Destroy(this);
    }

}
