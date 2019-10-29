using System;
using System.IO;
using UnityEngine;
using UnityEngine.Rendering;
using Random = System.Random;

public class RayTracingMain : MonoBehaviour
{
    // Start is called before the first frame update
    public ComputeShader rtxShader;
    public Texture skybox;
    //public bool monteCarlo;
    
    private RenderTexture _target;
    private Camera _camera;
    private uint _currentSample = 0;
    private Material _addMaterial;

    private void Awake()
    {
        _camera = GetComponent<Camera>();
    }

    private void Update()
    {
        if (!transform.hasChanged) return;
        _currentSample = 0;
        transform.hasChanged = false;
    }
    
    private void SetShaderParameters()
    {
        rtxShader.SetMatrix("_CameraToWorld", _camera.cameraToWorldMatrix);
        rtxShader.SetMatrix("_CameraInverseProjection", _camera.projectionMatrix.inverse);
    }

    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        SetShaderParameters();
        Render(destination);
    }
    
    private void Render(RenderTexture destination)
    {
        // Make sure we have a current render target
        InitRenderTexture();

        // Set the target and dispatch the compute shader
        rtxShader.SetTexture(0, "Result", _target);
        rtxShader.SetVector("_pixelOffset", UnityEngine.Random.insideUnitCircle);
        rtxShader.SetTexture(0, "_skybox", skybox);
        
        var threadGroupsX = Mathf.CeilToInt(Screen.width / 8.0f);
        var threadGroupsY = Mathf.CeilToInt(Screen.height / 8.0f);
        rtxShader.Dispatch(0, threadGroupsX, threadGroupsY, 1);

        // Blit the result texture to the screen
        if (_addMaterial == null)
            _addMaterial = new Material(Shader.Find("Hidden/AASampler"));
        _addMaterial.SetFloat("_Sample", _currentSample);
        Graphics.Blit(_target, destination, _addMaterial);
        _currentSample++;
    }

    private void InitRenderTexture()
    {
        if (_target != null && _target.width == Screen.width && _target.height == Screen.height) return;
        // Release render texture if we already have one
        if (_target != null)
            _target.Release();

        // Get a render target for Ray Tracing
        _target = new RenderTexture(Screen.width, Screen.height, 0,
            RenderTextureFormat.ARGBFloat, RenderTextureReadWrite.Linear) {enableRandomWrite = true};
        _target.Create();
    }
}
