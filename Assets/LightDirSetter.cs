using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightDirSetter : MonoBehaviour
{
    void SetShaderVal()
    {
        Vector3 ForwardVec = transform.forward;

        Debug.Log("Setting Shader Val to: " + ForwardVec);

        Shader.SetGlobalVector("_LightDir", new Vector4(ForwardVec.x, ForwardVec.y, ForwardVec.z,1.0f));
    }

    void Start()
    {
        SetShaderVal();    
    }

#if UNITY_EDITOR
    private void Update()
    {
        SetShaderVal();
    }
#endif
}
