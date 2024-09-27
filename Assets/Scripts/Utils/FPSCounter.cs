using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FPSCounter : MonoBehaviour
{
    private List<float> times = new List<float>();
    private UnityEngine.UI.Text text;
    private void Start()
    {
        text = GetComponent<UnityEngine.UI.Text>();
    }
    // Update is called once per frame
    void Update()
    {
        times.Add( Time.deltaTime );
        if (times.Count > 3)
            times.RemoveAt(0);
        float t = 0.0f;
        for (int i = 0; i < times.Count; i++)
        {
            t+= times[i];
        }
        t /= times.Count;
        text.text = Mathf.RoundToInt(1.0f / t).ToString();
    }
}
