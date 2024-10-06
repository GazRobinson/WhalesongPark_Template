using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.Rendering;
using Unity.Properties;
using System;
using UnityEngine.UI;

public class CameraRenderRequesting : MonoBehaviour
{
    [NonSerialized]
    public Camera[] cameras;
    [NonSerialized]
    RenderTexture[] renderTextures;

    RenderTexture[] PostProcessTextures;

    [NonSerialized]
    Material[] PostProcessMats;

    Dictionary<Camera, int> CamToId;

    static int CamsAdded = 0;

    public static CameraRenderRequesting SelfRef;

    private void Awake()
    {
        Debug.Log("Set SelfRef");
        CamsAdded = 0;
        SelfRef = this;
    }

    public void AddCam(Camera CamIn, RenderTexture RTex1In, RenderTexture RTex2In, RawImage AscRawImg, Material PostProcessMat)
    {
        if (cameras == null)
        {
            cameras = new Camera[4];
            renderTextures = new RenderTexture[4];
            PostProcessTextures = new RenderTexture[4];
            PostProcessMats = new Material[4];
            CamToId = new Dictionary<Camera, int>();
        }

        cameras[CamsAdded] = CamIn;
        renderTextures[CamsAdded] = RTex1In;
        PostProcessTextures[CamsAdded] = RTex2In;
        PostProcessMats[CamsAdded] = PostProcessMat;
        CamToId.Add(CamIn,CamsAdded);

        AscRawImg.texture = RTex2In;

        CamsAdded++;

        if (CamsAdded == 4)
        {
            ReadyToRender();
        }
    }

    void ReadyToRender()
    {
        // Make sure all data is valid before you start the component
        if (cameras == null || cameras.Length == 0 || renderTextures == null || cameras.Length != renderTextures.Length)
        {
            Debug.LogError("Invalid setup");
            return;
        }

        // Start the asynchronous coroutine
        StartCoroutine(RenderSingleRequestNextFrame());

        // Call a method called OnEndContextRendering when a camera finishes rendering
        RenderPipelineManager.endContextRendering += OnEndContextRendering;
        RenderPipelineManager.endCameraRendering += OnCamEndRendering;
    }

    void OnCamEndRendering(ScriptableRenderContext context, Camera CCam)
    {
        if (CamToId.TryGetValue(CCam, out int i))
        {
            Graphics.Blit(renderTextures[i], PostProcessTextures[i], PostProcessMats[i]);
        }
    }

    void OnEndContextRendering(ScriptableRenderContext context, List<Camera> cameras)
    {
        // Create a log to show cameras have finished rendering
        //Debug.Log("All cameras have finished rendering.");
    }

    void OnDestroy()
    {
        // End the subscription to the callback
        RenderPipelineManager.endContextRendering -= OnEndContextRendering;
    }

    [SerializeField]
    bool CheckFrame = false;

    IEnumerator RenderSingleRequestNextFrame()
    {
        // Wait for the main camera to finish rendering
        yield return new WaitForEndOfFrame();

        // Enqueue one render request for each camera
        SendSingleRenderRequests();

        // Wait for the end of the frame
        yield return new WaitForEndOfFrame();

#if UNITY_EDITOR
        if (CheckFrame)
        {
            Debug.Break();
        }
        else
        {
            StartCoroutine(RenderSingleRequestNextFrame());
        }
#else
        StartCoroutine(RenderSingleRequestNextFrame());
#endif
    }

    void SendSingleRenderRequests()
    {
        //Debug.Log("Doing Single Rendering");

        for (int i = 0; i < cameras.Length; i++)
        {
            UniversalRenderPipeline.SingleCameraRequest request =
                new UniversalRenderPipeline.SingleCameraRequest();

            // Check if the active render pipeline supports the render request
            if (RenderPipeline.SupportsRenderRequest(cameras[i], request))
            {
                // Set the destination of the camera output to the matching RenderTexture
                request.destination = renderTextures[i];

                // Render the camera output to the RenderTexture synchronously
                RenderPipeline.SubmitRenderRequest(cameras[i], request);

                // At this point, the RenderTexture in renderTextures[i] contains the scene rendered from the point
                // of view of the Camera in cameras[i]
                //Graphics.Blit(renderTextures[i], PostProcessTextures[i], PostProcessMats[i]);
            }
        }
    }
}
