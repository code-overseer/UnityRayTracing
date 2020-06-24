#ifndef LAMBERTUTILS_CGINC
#define LAMBERTUTILS_CGINC

#include "Utils.hlsl"

float3 Uniform_Sample(in float3 normal, inout uint seed)
{
    float3 x, z;
    CreateCoordinateSystem(normal, x, z);
    float2 r = float2(rand(seed), rand(seed));
    float2 a = float2(sqrt(1 - r.x * r.x), 2 * PI * r.y);
    
    return normalize(float3(a.x * cos(a.y), r.x, a.x * sin(a.y)));
}

float3 Cosine_Sample(in float3 normal, inout uint seed)
{
    float3 x, z;
    CreateCoordinateSystem(normal, x, z);
    float2 r = float2(rand(seed), rand(seed));
    float2 a = float2(sqrt(r.x), 2 * PI * r.y);
    
    return normalize(float3(a.x * cos(a.y), sqrt(1 - r.x), a.x * sin(a.y)));
}

RayDesc ImportanceCosine(inout uint seed, in float3 normal)
{
    RayDesc ray; // DXR defined
    ray.Origin = WorldRayOrigin();
    ray.Direction = WorldRayDirection();
    ray.TMin = 0;
    ray.TMax = T_MAX;
    ray.Origin = ray.Origin + RayTCurrent() * ray.Direction + EPSILON * normal;
    ray.Direction = Cosine_Sample(normal, seed);
    TransformToWorld(ray.Direction, normal);
    return ray;
}

RayDesc ImportanceUniform(inout uint seed, in float3 normal)
{
    RayDesc ray; // DXR defined
    ray.Origin = WorldRayOrigin();
    ray.Direction = WorldRayDirection();
    ray.TMin = 0;
    ray.TMax = T_MAX;
    ray.Origin = ray.Origin + RayTCurrent() * ray.Direction + EPSILON * normal;
    ray.Direction = Uniform_Sample(normal, seed);
    TransformToWorld(ray.Direction, normal);
    return ray;
}

#endif