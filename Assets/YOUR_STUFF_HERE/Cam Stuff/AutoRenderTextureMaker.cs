using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AutoRenderTextureMaker : MonoBehaviour
{
    RenderTexture CamRenderTex;
    RenderTexture PostProcessTex;

    Camera CamRef;

    CameraRenderRequesting RenderRequestRef;

    [SerializeField]
    float ResScale = 1.0f; //Multiplier, 1.0, is a res of 640,1280

    public static float FinalResScale;

    [SerializeField]
    RawImage AscRawImage;

    [SerializeField]
    Material PostProcessMat;

    // Start is called before the first frame update
    void Start()
    {
        FinalResScale = ResScale;

        RenderRequestRef = CameraRenderRequesting.SelfRef;

        CamRef = GetComponent<Camera>();
        CamRef.depthTextureMode = DepthTextureMode.Depth;

        CamRenderTex = new RenderTexture((int)(640f * ResScale), (int)(1280f * ResScale), 16);
        PostProcessTex = new RenderTexture((int)(640f * ResScale), (int)(1280f * ResScale), 16);

        CamRef.targetTexture = CamRenderTex;

        Debug.Log("Adding Cam! " + RenderRequestRef == null);
        RenderRequestRef.AddCam(CamRef,CamRenderTex,PostProcessTex, AscRawImage, PostProcessMat);

        //CamRef.targetTexture = CamRenderTex;
        //AscRawImage.texture = CamRenderTex;

    }
}
