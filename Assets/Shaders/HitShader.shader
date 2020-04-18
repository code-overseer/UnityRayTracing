Shader "Custom/HitShader"
{
	Properties
	{
		_Color ("Color", Color) = (1,1,1,1)
		_MainTex ("Albedo (RGB)", 2D) = "white" {}
		_Roughness ("Roughness", Range(0,1)) = 0.3
		_Metallic ("Metallic", Range(0,1)) = 0.5
		_IoR ("Index of Refraction", Range(0,20)) = 1.3
		_Emission("Emission", Color) = (0,0,0,1)
	}
	SubShader
	{
		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float4 vertex : SV_POSITION;
			};

			float4 _Color;

			v2f vert(appdata v) 
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				return o; 
			}

			float4 frag(v2f i) : SV_Target { return _Color; }

			ENDCG
		}
	}
	SubShader
	{
		Pass
		{
			Name "DefaultRTPass"
			HLSLPROGRAM // Pass name must match that specified by SetShaderPass()

			#pragma raytracing OnRayHit
			#include "common.hlsl"
			float4 _Emission;
			float _Metallic;
			float _Roughness;
			float _IoR;
			float4 _Color;
			Texture2D<float4> _MainTex;
			SamplerState sampler_MainTex;

			[shader("closesthit")]
			void OnRayHit(inout RayPayload payload : SV_RayPayload, in BuiltInTriangleIntersectionAttributes attribs : SV_IntersectionAttributes)
			{
				uint3 tri_idx = UnityRayTracingFetchTriangleIndices(PrimitiveIndex());
				float3 bary = GetBarycentrics(attribs);
				float3 normal = GetNormal(tri_idx, bary);
				
				payload.color = saturate(dot(normal, float3(0, 0, -1))) * _Color;
			}
			ENDHLSL
		}
	}
}
