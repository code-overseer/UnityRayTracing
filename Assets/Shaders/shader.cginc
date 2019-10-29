// Upgrade NOTE: replaced '_CameraToWorld' with 'unity_CameraToWorld'

// Upgrade NOTE: replaced '_CameraToWorld' with 'unity_CameraToWorld'

#if !defined(SHADER_H)
#define SHADER_H
static const int DIFF = 0;
static const int REFL = 1;
static const int TRANS = 2;
static const float PI = 3.14159265359f;
static const float EPSILON = 1e-5f;
static const float MAX_DIST = 1e+5f;
static const float KS = 0.1f;

struct Material
{
    float3 eps;
    float3 rho;
};

Material CreateMaterial(in float3 reflective, in float3 emissive)
{
    Material mat;
    mat.eps = emissive;
    mat.rho = reflective;
    return mat;
}

struct Ray 
{
    float3 origin;
    float3 dir;
};

struct Point
{
    float3 pos;
    Material mat;
};

struct RayHit
{
    float3 pos;
    float3 n;
    Material mat;
    float2 dist_type;
};

struct Plane 
{
    float3 pos;
    float3 n;
    Material mat;
    int type;
};


bool PlaneHit(in Plane pl, in Ray ray, inout RayHit hit) 
{
    float t = dot(pl.pos - ray.origin, pl.n) / dot(ray.dir, pl.n);
    if (t < 0 || t > hit.dist_type[0]) return false;
    hit.pos = ray.origin + t * ray.dir;
    hit.n = pl.n;
    hit.mat = pl.mat;
    hit.dist_type[0] = t;
    hit.dist_type[1] = pl.type; 
    return true;
}

struct Sphere 
{
    float4 centre;
    Material mat;
    int type;
};

bool SphereHit(in Sphere sph, in Ray ray, inout RayHit hit)
{
    float3 o_c = ray.origin - sph.centre.xyz;
    
    float4 q = float4(0,0,0,0);
    q[0] = dot(ray.dir, ray.dir);
    q[1] = 2 * dot(ray.dir, o_c);
    q[2] = dot(o_c, o_c) - sph.centre.w * sph.centre.w;
    q[3] = q[1] * q[1] - 4 * q[0] * q[2];
    
    if (q[3] < 0) return false;

    // Root of quadratic eqn
    float t = (-q[1] - sqrt(q[3]))/(2 * q[0]);
    if (t < 0) {
        t = (-q[1] + sqrt(q[3]))/(2 * q[0]);
        if (t < 0) return false;
    }

    if (t >= hit.dist_type[0]) return false;
    
    hit.pos = ray.origin + t * ray.dir;
    hit.n = normalize(hit.pos - sph.centre.xyz);
    hit.mat = sph.mat;
    hit.dist_type[0] = t;
    hit.dist_type[1] = sph.type;
    return true;
}

struct Box
{
    float4x4 pn;
    Material mat;
    int type;
};

bool BoxHit(in float3 p, in Box bx, in RayHit hit)
{
    return ( dot(p, p) < dot(bx.pn._m13_m23_m33, bx.pn._m13_m23_m33) );
}

bool BoxHit(in float3 p, in bool sign, in int idx, in Box bx)
{
    float3 n = (!sign - sign) * bx.pn[idx].xyz;
    float3 v = bx.pn[0].xyz + n * bx.pn[idx].w;
    return (dot(p, n) - dot(v, n) < 0); 
}

bool BoxHit(in Box bx, in Ray ray, inout RayHit hit) 
{
    float3x3 a;
    float t;
    bool sign = 1;
    bool output = 0;
    uint idx = 1;
    
    for (uint i = 0; i < 6; ++i)
    {
        a[0] = (!sign - sign) * bx.pn[idx].xyz;
        a[1] = bx.pn[0].xyz + a[0] * bx.pn[idx].w;
        t = dot(a[1] - ray.origin, a[0]) / dot(ray.dir, a[0]);
        a[2] = ray.origin + t * ray.dir;
        if ( t > 0 && t < hit.dist_type[0] && BoxHit(a[2] - bx.pn[0].xyz, bx, hit) && BoxHit(a[2], !sign, idx, bx) && 
            BoxHit(a[2], !sign, idx % 3 + 1, bx) && BoxHit(a[2], sign, idx % 3 + 1, bx) &&
            BoxHit(a[2], !sign, (idx + 1) % 3 + 1, bx) && BoxHit(a[2], sign, (idx + 1) % 3 + 1, bx) )
        {
            hit.pos = a[2];
            hit.n = a[0];
            hit.mat = bx.mat;
            hit.dist_type[0] = t;
            hit.dist_type[1] = bx.type;
            output = true;
        }
        sign = !sign;
        idx += sign;
    }
    return output;
}

struct Disc
{
    float3 pos;
    float4 n;
    Material mat;
    int type;
};

bool DiscHit(in Disc ds, in Ray ray, inout RayHit hit)
{
    float t = dot(ds.pos - ray.origin, ds.n.xyz) / dot(ray.dir, ds.n.xyz);
    float3 p = ray.origin + t * ray.dir - ds.pos;
    if (t < 0 || t > hit.dist_type[0] || dot(p, p) > ds.n.w * ds.n.w) return false;
    hit.pos = p;
    hit.n = ds.n.xyz;
    hit.mat = ds.mat;
    hit.dist_type[0] = t;
    hit.dist_type[1] = ds.type;
    return true;
}

struct State 
{
    RayHit hit;
    Ray ray;
    float3 kr_d_out;
};

float3 Black()
{
    return float3(0.0f, 0.0f, 0.0f);
}

float3 White() 
{
    return float3(1.0f, 1.0f, 1.0f);
}

Ray CreateRay(in float3 origin, in float3 dir) 
{
    Ray ray;
    ray.origin = origin;
    ray.dir = dir;
    return ray;
}

Ray CreateCameraRay(in float4x4 unity_CameraToWorld, in float4x4 _CameraInverseProjection, in float2 uv)
{
    // Transform the camera origin to world space
    float3 origin = mul(unity_CameraToWorld, float4(0.0f, 0.0f, 0.0f, 1.0f)).xyz;
    // Invert the perspective projection of the view-space position
    float3 dir = mul(_CameraInverseProjection, float4(uv, 0.0f, 1.0f)).xyz;
    // Transform the dir from camera to world space and normalize
    dir = mul(unity_CameraToWorld, float4(dir, 0.0f)).xyz;
    dir = normalize(dir);

    return CreateRay(origin, dir);
}

RayHit CreateRayHit()
{
    RayHit hit;
    hit.pos = Black();
    hit.n = Black();
    hit.mat = CreateMaterial(Black(), Black());
    hit.dist_type[0] = MAX_DIST;
    hit.dist_type[1] = DIFF;
    return hit;
}

float rnd()
{
    static int seed = 0;
	seed = int(fmod(float(seed)*1364.0+626.0, 509.0));
	return float(seed)/509.0;
}

float3 UniformHemisphere(in float3 normal) 
{
    
    float3 x;
    if (abs(normal.x) > abs(normal.y)) 
        x = normalize(float3(normal.z, 0, -normal.x)); 
    else 
        x = normalize(float3(0, -normal.z, normal.y));
     
    float3 z = cross(x, normal);
    float2 r = float2(rnd(), rnd());
    float2 a = float2(sqrt(1 - r.x * r.x), 2 * PI * r.y);
    float3 output = float3(a.x * cos(a.y), r.x, a.x * sin(a.y));
    output = float3(
        output.x * z.x + output.y * normal.x + output.z * x.x, 
        output.x * z.y + output.y * normal.y + output.z * x.y, 
        output.x * z.z + output.y * normal.z + output.z * x.z
    );
    
    return output;  

}

#endif