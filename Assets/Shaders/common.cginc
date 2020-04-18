#ifndef COMMON_CGINC
#define COMMON_CGINC

#include "UnityRaytracingMeshUtils.cginc"

#define PI 3.14159265359f
#define INV_PI 0.3183098862f
#define KS 0.01f
#define EPSILON 1e-4f
#define T_MAX 1e-4f

struct RayPayload
{
    float4 color;
    float ior;
    uint seed;
    uint depth;
};

struct Material
{
    float4 color;
    float4 emission;
    float metallic;
    float roughness;
    float ior;
};

struct TriangleAttribute
{
    float2 barycentric;
};

uint hash(in uint seed)
{
    seed = (seed ^ 61) ^ (seed >> 16);
    seed *= 9;
    seed = seed ^ (seed >> 4);
    seed *= 0x27d4eb2d;
    seed = seed ^ (seed >> 15);
    return seed;
}

float rand(inout uint seed)
{
    seed ^= (seed << 13);
    seed ^= (seed >> 17);
    seed ^= (seed << 5);
    return float(seed % 509) / 509.f;
}

RayPayload NewPayload(uint seed, uint depth)
{
    RayPayload payload;
    payload.color = float4(0, 0, 0, 1);
    payload.ior = 1.f;
    payload.seed = hash(seed);
    rand(payload.seed);
    payload.depth = depth;
    return payload;
}

void CreateCoordinateSystem(in float3 n, out float3 x, out float3 z)
{
    bool flag = abs(n.x) >= abs(n.y);
    x = normalize(float3(flag * n.z, !flag * -n.z, !flag * n.y - flag * n.x));
    z = normalize(cross(x, n));
}

void TransformToWorld(out float3 dir, in float3 ref_y)
{
    float3 x, z;
    CreateCoordinateSystem(ref_y, x, z);
    dir = normalize(float3(
        dir.x * z.x + dir.y * ref_y.x + dir.z * x.x,
        dir.x * z.y + dir.y * ref_y.y + dir.z * x.y,
        dir.x * z.z + dir.y * ref_y.z + dir.z * x.z
    ));
}

#define INTERPOLATE_ATTRIBUTE(att0,att1,att2,bary) \
(att0 * bary.x + att1 * bary.y + att2 * bary.z)

float3 GetBarycentrics(in TriangleAttribute attribs)
{
    return float3(1 - attribs.barycentric.x - attribs.barycentric.y, attribs.barycentric);
}

float3 GetNormal(in uint3 tri_idx, in float3 bary)
{
    float3 n0 = UnityRayTracingFetchVertexAttribute3(tri_idx.x, kVertexAttributeNormal);
    float3 n1 = UnityRayTracingFetchVertexAttribute3(tri_idx.y, kVertexAttributeNormal);
    float3 n2 = UnityRayTracingFetchVertexAttribute3(tri_idx.z, kVertexAttributeNormal);
    float3 n = mul(ObjectToWorld3x4(), float4(INTERPOLATE_ATTRIBUTE(n0, n1, n2, bary), 0));
    return normalize(n);
}

float2 GetUV(in uint3 tri_idx, in float3 bary)
{
    float2 uv0 = UnityRayTracingFetchVertexAttribute2(tri_idx.x, kVertexAttributeTexCoord0);
    float2 uv1 = UnityRayTracingFetchVertexAttribute2(tri_idx.y, kVertexAttributeTexCoord0);
    float2 uv2 = UnityRayTracingFetchVertexAttribute2(tri_idx.z, kVertexAttributeTexCoord0);

    return INTERPOLATE_ATTRIBUTE(uv0, uv1, uv2, bary);
}

float3 Fresnel(in float sur_IoR, in float hit_IoR, in float3 albedo, in float metallic, in float cosi)
{
    float3 r0 = abs((sur_IoR - hit_IoR) / (sur_IoR + hit_IoR));
    r0 = r0 * r0;
    r0 = lerp(r0, albedo, metallic);
    return r0 + (1 - r0) * pow((1 - abs(cosi)), 5);
}

#endif