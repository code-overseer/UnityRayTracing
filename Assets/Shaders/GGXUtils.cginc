#ifndef GGXUTILS_CGINC
#define GGXUTILS_CGINC

#include "Utils.hlsl"

float chiGGX(float v)
{
    return v > 0 ? 1 : 0;
}

float GGX_PartialGeometryTerm(float3 v, float3 n, float3 h, float roughness)
{
    float VoH2 = saturate(dot(v, h));
    float chi = chiGGX(VoH2 / saturate(dot(v, n)));
    VoH2 = VoH2 * VoH2;
    float tan2 = (1 - VoH2) / VoH2;
    return (chi * 2) / (1 + sqrt(1 + roughness * roughness * tan2));
}

float3 GGX_sample(in float roughness, inout uint seed)
{
    float2 theta_phi = float2(rand(seed), rand(seed));
    theta_phi.x = atan(roughness * sqrt(theta_phi.x / (1 - theta_phi.x)));
    theta_phi.y = 2 * PI * theta_phi.y;
    float sinT = sin(theta_phi.x);
    float3 dir = float3(sinT * cos(theta_phi[1]), cos(theta_phi[0]), sinT * sin(theta_phi[1]));
    return normalize(dir);
}

float3 GGX_Specular(in float3 normal, in float3 view_dir, in float3 light_dir, in Material mat, in float sur_ior, out float3 k_spec, out float noh)
{
    float3 h = normalize(light_dir + view_dir);
    float cos_o = saturate(dot(normal, view_dir));
    float cos_i = saturate(dot(normal, light_dir));
    float sin_i = sqrt(1 - cos_i * cos_i);
    k_spec = Fresnel(sur_ior, mat.ior, (float3) mat.color, mat.metallic, saturate(dot(h, view_dir)));
    float k_geo = GGX_PartialGeometryTerm(view_dir, normal, h, mat.roughness) * GGX_PartialGeometryTerm(light_dir, normal, h, mat.roughness);
    noh = saturate(dot(h, normal));
    float denominator = saturate(4 * (cos_i * noh + 0.05)); // 0.05 prevents div by zero
    
    return k_spec * k_geo * sin_i / denominator;
}

float GGX_Distribution(in float3 n, in float3 h, float roughness)
{
    float NoH = dot(n, h);
    float alpha2 = roughness * roughness;
    float NoH2 = NoH * NoH;
    float den = NoH2 * alpha2 + (1 - NoH2);
    return (chiGGX(NoH) * alpha2) * INV_PI / (den * den);
}

RayDesc ImportanceGGX(float roughness, inout uint seed, in float3 normal)
{
    RayDesc ray; // DXR defined
    ray.Origin = WorldRayOrigin();
    ray.Direction = WorldRayDirection();
    ray.TMin = 0;
    ray.TMax = T_MAX;
    ray.Origin = ray.Origin + RayTCurrent() * ray.Direction + EPSILON * normal;
    ray.Direction = GGX_sample(roughness, seed);
    TransformToWorld(ray.Direction, normal);
    return ray;
}

#endif