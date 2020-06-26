#ifndef SPECULARUTILS_CGINC
#define SPECULARUTILS_CGINC

#include "Utils.hlsl"

float chiGGX(float v)
{
    return v > 0 ? 1 : 0;
}

float GGX_PartialGeometryTerm(in float3 v, in float3 n, in float3 h, float roughness)
{
    float VoH2 = saturate(dot(v, h));
    float chi = chiGGX(VoH2 / saturate(dot(v, n)));
    VoH2 = VoH2 * VoH2;
    float tan2 = (1 - VoH2) / VoH2;
    return (chi * 2) / (1 + sqrt(1 + roughness * roughness * tan2));
}

float Simple_Geometry(in float3 n, in float3 v, in float3 l, in float3 h)
{
    float n_h_v_h = 2 * dot(n,h) / dot(v,h);
    return min(1, min(n_h_v_h * dot(n,v), n_h_v_h * dot(n,l)));
}

float3 GGX_sample(in float roughness, inout uint seed)
{
    float2 theta_phi = float2(rand(seed), rand(seed));
    float a2 = roughness * roughness;
    theta_phi.x = a2 / (theta_phi.x * (a2 - 1) + 1);
    theta_phi.x = acos(sqrt(theta_phi.x));
    theta_phi.y = 2 * PI * theta_phi.y;
    float sinT = sin(theta_phi.x);
    float3 dir = float3(sinT * cos(theta_phi[1]), cos(theta_phi[0]), sinT * sin(theta_phi[1]));
    return normalize(dir);
}

float3 Beckmann_sample(in float roughness, inout uint seed)
{
    float2 theta_phi = float2(rand(seed), rand(seed));
    theta_phi.x = 1 / (1 - roughness * roughness * log(1 - theta_phi.x));
    theta_phi.x = acos(sqrt(theta_phi.x));
    theta_phi.y = 2 * PI * theta_phi.y;
    float sinT = sin(theta_phi.x);
    float3 dir = float3(sinT * cos(theta_phi[1]), cos(theta_phi[0]), sinT * sin(theta_phi[1]));
    return normalize(dir);
}

float GeometryTerm(in float3 normal, in float3 view, in float3 light, in float3 h, float roughness)
{
    if (roughness <= 1e-5)
    {
        return 4 * saturate(dot(normal, view) + 0.01);
    }
    if (roughness <= 0.2)
    {
        return Simple_Geometry(normal, view, light, h);
    }
    return GGX_PartialGeometryTerm(view, normal, h, roughness) * GGX_PartialGeometryTerm(light, normal, h, roughness); 
}

RayDesc ImportanceSpecular(inout uint seed, in float3 normal, in float roughness)
{
    RayDesc ray; // DXR defined
    ray.Origin = WorldRayOrigin();
    ray.Direction = WorldRayDirection();
    ray.TMin = 0;
    ray.TMax = T_MAX;
    ray.Origin = ray.Origin + RayTCurrent() * ray.Direction + EPSILON * normal;
    if (roughness < 1e-5)
    {
        ray.Direction = normalize(reflect(WorldRayDirection(), normal));
    }
    if (roughness <= 0.2)
    {
        ray.Direction = Beckmann_sample(roughness, seed);
    }
    else 
    {
        ray.Direction = GGX_sample(roughness, seed);
    }
    TransformToWorld(ray.Direction, normal);
    return ray;
}

float4 Specular(in float3 normal, in float3 view, in float3 light, in Material mat, inout float4 ks)
{
    float3 h = normalize(light + view);
    float cos_o = saturate(dot(normal, view));
    float k_geo = GeometryTerm(normal, view, light, h, mat.roughness);
    float denominator = 4 * saturate(cos_o + 0.01);
    ks = Fresnel(1.0, mat.ior, mat.color, mat.metallic, saturate(dot(h, view)));
    return saturate(ks * k_geo / denominator);
}

#endif