using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class InputDebugger : MonoBehaviour
{
    public Text[] texts;
    public SingleComPortController comtroller;
    string s = "Waiting for input";
    // Update is called once per frame
    void Update()
    {
        if(comtroller != null)
        {
            s = "Buttons:\n" + comtroller.ButtonTester();
        }
        for (int i = 0; i < texts.Length; i++) {
            texts[i].text = s;
        }
    }
}
