#if !defined(SHADER_H)
#define SHADER_H

static const int DIFF = 0;
static const int REFL = 1;
static const int TRANS = 2;
static const float IoR= 1.0f;
static const float PI = 3.14159265359f;
static const float INV_PI = 0.3183098862f;
static const float EPSILON = 1e-5f;
static const float MAX_DIST = 100000.0f;
static const float KS = 0.00f;


struct Material
{
    float3 eps;
    float3 rho;
    float3 rough_ior_metal;
};

Material CreateMaterial(in float3 reflective, in float3 emissive)
{
    Material mat;
    mat.eps = emissive;
    mat.rho = reflective;
    mat.rough_ior_metal = float3(0,0,0);
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
    float3x3 p_n_o;
    Material mat;
    float dist;
};

struct Plane 
{
    float3 pos;
    float3 n;
    Material mat;
};


bool PlaneHit(in Plane pl, in Ray ray, inout RayHit hit) 
{
    float t = dot(pl.pos - ray.origin, pl.n) / dot(ray.dir, pl.n);
    if (t < 0 || t > hit.dist) return false;
    hit.p_n_o[0] = ray.origin + t * ray.dir;
    hit.p_n_o[1] = pl.n;
    hit.mat = pl.mat;
    hit.dist = t; 
    return true;
}

struct Sphere 
{
    float4 centre;
    Material mat;
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

    if (t >= hit.dist) return false;
    
    hit.p_n_o[0] = ray.origin + t * ray.dir;
    hit.p_n_o[1] = normalize(hit.p_n_o[0] - sph.centre.xyz);
    hit.mat = sph.mat;
    hit.dist = t;
    return true;
}


struct Box
{
    float4x4 pn;
    Material mat;
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
        if ( t > 0 && t < hit.dist && BoxHit(a[2] - bx.pn[0].xyz, bx, hit) && BoxHit(a[2], !sign, idx, bx) && 
            BoxHit(a[2], !sign, idx % 3 + 1, bx) && BoxHit(a[2], sign, idx % 3 + 1, bx) &&
            BoxHit(a[2], !sign, (idx + 1) % 3 + 1, bx) && BoxHit(a[2], sign, (idx + 1) % 3 + 1, bx) )
        {
            hit.p_n_o[0] = a[2];
            hit.p_n_o[1] = a[0];
            hit.mat = bx.mat;
            hit.dist = t;
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
};

bool DiscHit(in Disc ds, in Ray ray, inout RayHit hit)
{
    float t = dot(ds.pos - ray.origin, ds.n.xyz) / dot(ray.dir, ds.n.xyz);
    float3 p = ray.origin + t * ray.dir - ds.pos;
    if (t < 0 || t > hit.dist || dot(p, p) > ds.n.w * ds.n.w) return false;
    hit.p_n_o[0] = p;
    hit.p_n_o[1] = ds.n.xyz;
    hit.mat = ds.mat;
    hit.dist = t;
    return true;
}

struct Quad
{
    float4 pos;
    float4 n;
    Material mat;
};

void CreateCoordinateSystem(in float3 n, out float3 x, out float3 z) 
{
    if (abs(n.x) >= abs(n.y)) 
        x = normalize(float3(n.z, 0, -n.x)); 
    else 
        x = normalize(float3(0, -n.z, n.y));
    z = normalize(cross(x, n));
}

bool QuadHit(in Quad qd, in Ray ray, inout RayHit hit)
{
    float t = dot(qd.pos.xyz - ray.origin, qd.n.xyz) / dot(ray.dir, qd.n.xyz);
    float3 p = ray.origin + t * ray.dir;
    float3 x, z; 
    CreateCoordinateSystem(qd.n.xyz, x, z);
    //x = x*cosi + z*sini;
    //z = x*cosi - z*sini; 
    float3 a =  qd.pos.xyz + x * qd.pos.w + z * qd.n.w;
    float3 ab = qd.pos.xyz + x * qd.pos.w - z * qd.n.w - a;
    float3 ad = qd.pos.xyz - x * qd.pos.w + z * qd.n.w - a;
    a = p - a;
    float2 dots = float2(dot(a,ab), dot(a,ad));
    if (t < 0 || t > hit.dist || 0 > dots.x || 0 > dots.y  || 
        dots.x > dot(ab, ab) || dots.y > dot(ad, ad)) return false;
    
    hit.p_n_o[0] = p;
    hit.p_n_o[1] = qd.n.xyz;
    hit.mat = qd.mat;
    hit.dist = t;
    return true;
}

struct State 
{
    RayHit hit;
    Ray ray;
    float3 kr_d_cosmax;
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

static float3 ViewPoint;
Ray CreateCameraRay(in float4x4 unity_CameraToWorld, in float4x4 _CameraInverseProjection, in float2 uv)
{
    // Transform the camera origin to world space
    ViewPoint = mul(unity_CameraToWorld, float4(0.0f, 0.0f, 0.0f, 1.0f)).xyz;
    // Invert the perspective projection of the view-space position
    float3 dir = mul(_CameraInverseProjection, float4(uv, 0.0f, 1.0f)).xyz;
    // Transform the dir from camera to world space and normalize
    dir = mul(unity_CameraToWorld, float4(dir, 0.0f)).xyz;
    dir = normalize(dir);

    return CreateRay(ViewPoint, dir);
}

RayHit CreateRayHit()
{
    RayHit hit;
    hit.p_n_o[0] = Black();
    hit.p_n_o[1] = Black();
    hit.p_n_o[2] = Black();
    hit.mat = CreateMaterial(Black(), Black());
    hit.dist = MAX_DIST;
    return hit;
}

State NewState(in float2 uv, in float4x4 ctw, in float4x4 cip) 
{
    State state;
    state.ray = CreateCameraRay(ctw, cip, uv);
    state.hit = CreateRayHit();
    state.kr_d_cosmax[0] = 1;
    state.kr_d_cosmax[1] = 0;
    state.kr_d_cosmax[2] = 1;
    return state;
}

float GeoAtt(in State state, in float3 h, in float cosi, in float coso) {
    float2 val;
    val.x = 2 * dot(state.hit.p_n_o[1], h) / dot(state.hit.p_n_o[2], h);
    val.y = val.x * coso;
    val.x *= cosi;
    
    return min(1, min(val.x, val.y));
}

float Beckmann(in State state, in float3 h, in float cosi, in float coso) {
    float m = state.hit.mat.rough_ior_metal[0];
    if (m < 0.5f * INV_PI) return (abs(cosi - coso) < EPSILON);
    m -= (m > 1);
    float cosa = dot(state.hit.p_n_o[1], h);
    return exp( (cosa*cosa - 1)/(cosa*cosa*m*m))*INV_PI / (m*m*pow(cosa,4));
}

float3 Fresnel(in State state, in float cosi) 
{
	float3 r0 = pow((IoR - state.hit.mat.rough_ior_metal[1])/(IoR + state.hit.mat.rough_ior_metal[1]), 2) * float3(1,1,1);
	r0 = lerp(r0, state.hit.mat.rho, state.hit.mat.rough_ior_metal[2]);
	return r0 + (1 - r0) * pow((1 - abs(cosi)), 5);
}

float3 CookTorrance(in State state, in float cosi, in float coso)
{
    float3 h =  normalize(state.hit.p_n_o[2] + state.ray.dir);
    return Fresnel(state, cosi) * Beckmann(state, h, cosi, coso) * GeoAtt(state, h, cosi, coso) / (4 * cosi * coso);
}

#endif