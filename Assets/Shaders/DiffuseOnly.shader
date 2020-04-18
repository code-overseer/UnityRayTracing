Shader "Custom/DiffuseOnly"
{
	Properties
	{
		_Color("Color", Color) = (1,1,1,1)
		_MainTex("Albedo (RGB)", 2D) = "white" {}
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
					Name "DiffuseRTPass"
					HLSLPROGRAM // Pass name must match that specified by SetShaderPass()

					#pragma raytracing OnRayHit

					float4 _Color;
					Texture2D<float4> _MainTex;
					SamplerState sampler_MainTex;
					#include "UnityRaytracingMeshUtils.cginc"
					#define INTERPOLATE_ATTRIBUTE(att0,att1,att2,bary) \
					(att0 * bary.x + att1 * bary.y + att2 * bary.z)

					struct RayPayload
					{
						float4 color;
						float ior;
						uint seed;
						uint depth;
					};


					[shader("closesthit")]
					void OnRayHit(inout RayPayload payload : SV_RayPayload, BuiltInTriangleIntersectionAttributes attribs : SV_IntersectionAttributes)
					{
						uint3 tri_idx = UnityRayTracingFetchTriangleIndices(PrimitiveIndex());

						float3 bary = float3(1 - attribs.barycentrics.x - attribs.barycentrics.y, attribs.barycentrics);
						float3 n0 = UnityRayTracingFetchVertexAttribute3(tri_idx.x, kVertexAttributeNormal);
						float3 n1 = UnityRayTracingFetchVertexAttribute3(tri_idx.y, kVertexAttributeNormal);
						float3 n2 = UnityRayTracingFetchVertexAttribute3(tri_idx.z, kVertexAttributeNormal);
						float3 n = mul(ObjectToWorld3x4(), float4(INTERPOLATE_ATTRIBUTE(n0, n1, n2, bary), 0));
						float3 normal = normalize(n);

						payload.color = float4(bary, 1);
					}

					ENDHLSL
				}
			}
}
