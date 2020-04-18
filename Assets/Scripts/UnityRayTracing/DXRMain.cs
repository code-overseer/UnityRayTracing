using UnityEngine;
using Unity.Mathematics;
using UnityEngine.Experimental.Rendering;

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
		private string _shaderPass = "DefaultRTPass";
		private Camera _camera;
		private RenderTexture _target;

		private void Awake()
		{
			_camera = GetComponent<Camera>();
		}

		private void CreateStructure()
		{
			if (_rayTracingStructure != null) return;
			var Settings = new RayTracingAccelerationStructure.RASSettings(
				RayTracingAccelerationStructure.ManagementMode.Automatic,
				RayTracingAccelerationStructure.RayTracingModeMask.DynamicTransform | RayTracingAccelerationStructure.RayTracingModeMask.Static,
				0xffff);
			_rayTracingStructure = new RayTracingAccelerationStructure(Settings);
		}
		void OnEnable()
		{
			_shader.SetShaderPass(_shaderPass);
			CreateStructure();
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
			_shader.SetAccelerationStructure(Shader.PropertyToID("accelerationStructure"), _rayTracingStructure);
		}

		private void OnRenderImage(RenderTexture source, RenderTexture destination)
		{
			InitRenderTexture();
			SetShaderParams();
			_shader.Dispatch("GenerateRays", Screen.width, Screen.height, 1, _camera);
			
			Graphics.Blit(_target, destination);
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
