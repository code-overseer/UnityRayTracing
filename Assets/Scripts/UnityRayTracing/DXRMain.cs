using System;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Experimental.Rendering;
using Random = UnityEngine.Random;

namespace UnityRayTracing
{
	public class DXRMain : MonoBehaviour
	{
		public struct CameraUniforms
		{
			public float4x4 CameraToWorld;
			public float4x4 InverseProjection;
		};

		private RayTracingAccelerationStructure _rayTracingStructure;

		[SerializeField]
		private Texture _skybox;

		[SerializeField]
		private RayTracingShader _shader;

		[SerializeField]
		private string _shaderPass = "LambertianPass";

		private Camera _camera;
		private RenderTexture _target;
		private Material _aaMaterial;
		private int _currentSample = 0;
		private int _propertyToId;

		private void Awake()
		{
			_camera = GetComponent<Camera>();
			_aaMaterial = new Material(Shader.Find("Hidden/AASampler"));
		}

		private void CreateStructure()
		{
			if (_rayTracingStructure != null) return;
			var Settings = new RayTracingAccelerationStructure.RASSettings(
				RayTracingAccelerationStructure.ManagementMode.Automatic,
				RayTracingAccelerationStructure.RayTracingModeMask.Static,
				0xffff);
			_rayTracingStructure = new RayTracingAccelerationStructure(Settings);
			Debug.Log(_rayTracingStructure.GetSize());
		}

		private void Start()
		{
			_shader.SetShaderPass(_shaderPass);
			_propertyToId = Shader.PropertyToID("_Sample");
			CreateStructure();
		}
		
		private void Update()
		{
			if (!transform.hasChanged) return;
			_currentSample = 0;
			transform.hasChanged = false;
		}

		private void OnDisable()
		{ 
			_rayTracingStructure?.Release();
			_target?.Release();
			_rayTracingStructure = null;
			_target = null;
		}

		private void SetShaderParams()
		{
			_rayTracingStructure.Build();
			_shader.SetMatrix("_cameraToWorld", _camera.cameraToWorldMatrix);
			_shader.SetMatrix("_inverseProjection", _camera.projectionMatrix.inverse);
			_shader.SetTexture("_skybox", _skybox);
			_shader.SetTexture("RenderTarget", _target);
			_shader.SetInt("_seed", Random.Range(Int32.MinValue, Int32.MaxValue));
			_shader.SetAccelerationStructure(Shader.PropertyToID("_BVHStructure"), _rayTracingStructure);
		}

		private void OnRenderImage(RenderTexture source, RenderTexture destination)
		{
			InitRenderTexture();
			SetShaderParams();
			_shader.Dispatch("GenerateRays", Screen.width, Screen.height, 1, _camera);
			_currentSample++;
			_aaMaterial.SetFloat(_propertyToId, _currentSample);
			Graphics.Blit(_target, destination, _aaMaterial);
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
			_target = new RenderTexture(Screen.width, Screen.height, 1,
				RenderTextureFormat.ARGBFloat, RenderTextureReadWrite.Linear)
			{ enableRandomWrite = true };
			_target.Create();
		}
	}
}
