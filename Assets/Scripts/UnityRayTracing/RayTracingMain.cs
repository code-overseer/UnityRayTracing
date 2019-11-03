using System;
using System.IO;
using UnityEngine;
using UnityEngine.Assertions.Comparers;
using UnityEngine.Rendering;
using Random = System.Random;

namespace UnityRayTracing
{
    public class RayTracingMain : MonoBehaviour
    {
        // Start is called before the first frame update
        public ComputeShader rtxShader;
        public Texture skybox;
        
        private RenderTexture _target;
        private Camera _camera;
        private int _currentSample = 0;
        private UnityEngine.Material _addMaterial;
        private static readonly int Sample = Shader.PropertyToID("_Sample");
        private ComputeBuffer _planeBuffer;
        private ComputeBuffer _sphereBuffer;
        private ComputeBuffer _boxBuffer;
        private ComputeBuffer _quadBuffer;
        private ComputeBuffer _discBuffer;
        private ComputeBuffer _sizeBuffer;
        private void Awake()
        {
            _camera = GetComponent<Camera>();
            _addMaterial = new UnityEngine.Material(Shader.Find("Hidden/AASampler"));
        }
        

        private void OnEnable()
        {
            _currentSample = 0;
            if (SceneObject.Planes.Count > 0)
            {
                _planeBuffer = new ComputeBuffer(SceneObject.Planes.Count, Strides.plane);
                _planeBuffer.SetData(SceneObject.Planes);
            }
            if (SceneObject.Boxes.Count > 0)
            {
                _boxBuffer = new ComputeBuffer(SceneObject.Boxes.Count, Strides.box);
                _boxBuffer.SetData(SceneObject.Boxes);
            }
            if (SceneObject.Spheres.Count > 0)
            {
                _sphereBuffer = new ComputeBuffer(SceneObject.Spheres.Count, Strides.sphere);
                _sphereBuffer.SetData(SceneObject.Spheres);
            }

            if (SceneObject.Discs.Count > 0)
            {
                _discBuffer = new ComputeBuffer(SceneObject.Discs.Count, Strides.disc);
                _discBuffer.SetData(SceneObject.Discs);
            }

            if (SceneObject.Quads.Count > 0)
            {
                _quadBuffer = new ComputeBuffer(SceneObject.Quads.Count, Strides.quad);
                _quadBuffer .SetData(SceneObject.Quads);
            }
            
            var sizes = new []
            {
                SceneObject.Planes.Count,
                SceneObject.Boxes.Count,
                SceneObject.Spheres.Count,
                SceneObject.Discs.Count,
                SceneObject.Quads.Count,
            };
            _sizeBuffer = new ComputeBuffer(5, 4);
            _sizeBuffer.SetData(sizes);
            
        }
        
        private void OnDisable()
        {
            _planeBuffer?.Release();
            _boxBuffer?.Release();
            _sphereBuffer?.Release();
            _discBuffer?.Release();
            _quadBuffer?.Release();
            _sizeBuffer?.Release();
        }

        private void OnApplicationQuit()
        {
            _planeBuffer?.Release();
            _boxBuffer?.Release();
            _sphereBuffer?.Release();
            _discBuffer?.Release();
            _quadBuffer?.Release();
            _sizeBuffer?.Release();
        }

        private void Update()
        {
            if (!transform.hasChanged) return;
            _currentSample = 0;
            transform.hasChanged = false;
        }
        
        private void SetShaderParameters()
        {
            var kernel = rtxShader.FindKernel("CSMain");
            rtxShader.SetMatrix("_CameraToWorld", _camera.cameraToWorldMatrix);
            rtxShader.SetMatrix("_CameraInverseProjection", _camera.projectionMatrix.inverse);
            rtxShader.SetVector("_PixelOffset", new Vector2(UnityEngine.Random.value - 0.5f, 
                UnityEngine.Random.value - 0.5f));
            rtxShader.SetInt("_Seed", Mathf.FloorToInt(UnityEngine.Random.value * 500));
            if (_planeBuffer != null) rtxShader.SetBuffer(kernel, "_Planes", _planeBuffer);
            if (_boxBuffer != null) rtxShader.SetBuffer(kernel, "_Boxes", _boxBuffer);
            if (_sphereBuffer != null) rtxShader.SetBuffer(kernel, "_Spheres", _sphereBuffer);
            if (_discBuffer != null) rtxShader.SetBuffer(kernel, "_Discs", _discBuffer);
            if (_quadBuffer != null) rtxShader.SetBuffer(kernel, "_Quads", _quadBuffer);
            rtxShader.SetBuffer(kernel, "_Sizes", _sizeBuffer);
            
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

            var threadGroupsX = Mathf.CeilToInt(Screen.width / 8.0f);
            var threadGroupsY = Mathf.CeilToInt(Screen.height / 8.0f);
            rtxShader.Dispatch(0, threadGroupsX, threadGroupsY, 1);

            // Blit the result texture to the screen
            _addMaterial.SetFloat(Sample, _currentSample);
            Graphics.Blit(_target, destination, _addMaterial);
            _currentSample++;
        }

        private void InitRenderTexture()
        {
            if (_target != null && _target.width == Screen.width && _target.height == Screen.height) return;
            // Release render texture if we already have one
            if (_target != null)
            {
                _target.Release();
            }
            
            // Get a render target for Ray Tracing
            _target = new RenderTexture(Screen.width, Screen.height, 0,
                RenderTextureFormat.ARGBFloat, RenderTextureReadWrite.Linear) {enableRandomWrite = true};
            _target.Create();
        }
        
    }    
}

