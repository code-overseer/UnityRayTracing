Shader "Custom/HitShader"
{
	Properties
	{
		_Color ("Color", Color) = (1,1,1,1)
		_MainTex ("Albedo (RGB)", 2D) = "white" {}
		_Roughness ("Roughness", Range(0,1)) = 0.3
		_Metallic ("Metallic", Range(0,1)) = 0.5
		_IoR ("Index of Refraction", Range(0,20)) = 1.3
		_EmissionStrength("Emission Strength", Float) = 1.0
		_Emission("Emission", Color) = (0,0,0,1)
	}
	SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200

        CGPROGRAM
        // Physically based Standard lighting model, and enable shadows on all light types
        #pragma surface surf Standard fullforwardshadows

        // Use shader model 3.0 target, to get nicer looking lighting
        #pragma target 3.0

        sampler2D _MainTex;

        struct Input
        {
            float2 uv_MainTex;
        };

        half _Glossiness;
        half _Metallic;
        fixed4 _Color;

        // Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
        // See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
        // #pragma instancing_options assumeuniformscaling
        UNITY_INSTANCING_BUFFER_START(Props)
            // put more per-instance properties here
        UNITY_INSTANCING_BUFFER_END(Props)

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            // Albedo comes from a texture tinted by color
            fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;
            o.Albedo = c.rgb;
            // Metallic and smoothness come from slider variables
            o.Metallic = _Metallic;
            o.Smoothness = _Glossiness;
            o.Alpha = c.a;
        }
        ENDCG
    }
	SubShader
	{
	    // DONT USE THIS PASS IF YOU HAVE HIGH SAMPLE/DEPTH
		Pass
		{
			Name "LambertianPass"
			HLSLPROGRAM

			#pragma raytracing OnRayHit
			#include "Utils.hlsl"
			#include "LambertUtils.cginc"
			float4 _Color;
			float4 _Emission;
			half _EmissionStrength;
			Texture2D<float4> _MainTex;
			SamplerState sampler_MainTex;

			[shader("closesthit")]
			void OnRayHit(inout RayPayload payload : SV_RayPayload, TriangleAttribute attribs : SV_IntersectionAttributes)
			{
				uint3 tri_idx = UnityRayTracingFetchTriangleIndices(PrimitiveIndex());
				float3 bary = GetBarycentrics(attribs);
				float3 normal = GetNormal(tri_idx, bary);
				float2 uv = GetUV(tri_idx, bary);
				bool backface = dot(normal, WorldRayDirection()) > 0;
				normal *= (!backface - backface);
                payload.depth = min(payload.depth, 3);
                payload.depth -= (payload.depth > 0);
                if (payload.depth > 0)
                {
                    rand(payload.seed);
                    RayPayload copy_load = payload;
                    RayDesc ray = ImportanceCosine(payload.seed, normal);
                    float4 accumulate = float4(0,0,0,0);
                    int count = min(SAMPLE_COUNT, 4);
                    for (int i = 0; i < count; ++i)
                    {
                        TraceRay(_DiffuseBVH, RAY_FLAG, INSTANCE_INCLUSION_MASK, RAY_CONTRIB_HITGROUP_IDX, GEOMETRY_STRIDE, MISS_SHADER, ray, copy_load);
                        accumulate += (_EmissionStrength * _Emission + copy_load.color * _Color) * _MainTex.SampleLevel(sampler_MainTex, uv, 0);
                        rand(payload.seed);
                        copy_load = payload;
                        ray = ImportanceCosine(copy_load.seed, normal);
                    }
                    payload.color = accumulate / SAMPLE_COUNT;
                }
				payload.color = (_EmissionStrength * _Emission + payload.color * _Color) * _MainTex.SampleLevel(sampler_MainTex, uv, 0);
			}

			ENDHLSL
		}
		
		Pass
		{
			Name "FastLambertian"
			HLSLPROGRAM

			#pragma raytracing OnRayHit
			#include "Utils.hlsl"
			#include "LambertUtils.cginc"
			float4 _Color;
			float4 _Emission;
			half _EmissionStrength;
			Texture2D<float4> _MainTex;
			SamplerState sampler_MainTex;

			[shader("closesthit")]
			void OnRayHit(inout RayPayload payload : SV_RayPayload, TriangleAttribute attribs : SV_IntersectionAttributes)
			{
				uint3 tri_idx = UnityRayTracingFetchTriangleIndices(PrimitiveIndex());
				float3 bary = GetBarycentrics(attribs);
				float3 normal = GetNormal(tri_idx, bary);
                float2 uv = GetUV(tri_idx, bary);
				bool backface = dot(normal, WorldRayDirection()) > 0;
				normal *= (!backface - backface);
                RayDesc ray = ImportanceCosine(payload.seed, normal);
                payload.depth = min(payload.depth, 3);
                payload.depth -= (payload.depth > 0);
                if (payload.depth > 0)
                {    
                    TraceRay(_DiffuseBVH, RAY_FLAG, INSTANCE_INCLUSION_MASK, RAY_CONTRIB_HITGROUP_IDX, GEOMETRY_STRIDE, MISS_SHADER, ray, payload);
                }
				payload.color = (_EmissionStrength * _Emission + payload.color * _Color) * _MainTex.SampleLevel(sampler_MainTex, uv, 0);
			}

			ENDHLSL
		}
	}
}
