﻿using System;
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
        public Transform[] planes;
        public Transform[] spheres;
        public Transform[] boxes;
        public Transform[] quads;
        public Transform[] discs;
        
        
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
        private void Awake()
        {
            _camera = GetComponent<Camera>();
            _addMaterial = new UnityEngine.Material(Shader.Find("Hidden/AASampler"));
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
            rtxShader.SetVector("_pixelOffset", new Vector2(UnityEngine.Random.Range(0.0f,0.5f), 
                UnityEngine.Random.Range(0.0f,0.5f)));
            rtxShader.SetTexture(0, "_skybox", skybox);
            rtxShader.SetInt("SEED", Mathf.FloorToInt(UnityEngine.Random.value * 100));

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
