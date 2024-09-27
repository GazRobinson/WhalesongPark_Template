using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectLifeCheck : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Debug.LogError("Object " + gameObject.name + " has been activated and is probably used by something!");
    }
}
