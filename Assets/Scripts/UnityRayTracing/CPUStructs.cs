using UnityEngine;
// ReSharper disable InconsistentNaming
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable NotAccessedField.Global

namespace UnityRayTracing
{
    public static class Strides
    {
        public const int material = 36;
        public const int plane = 24 + material;
        public const int sphere = 16 + material;
        public const int box = 16 * 4 + material;
        public const int disc = 12 + 16 + material;
        public const int quad = 16 + 16 + material;

    }
    public struct Material
    {
        public Vector3 emissive;
        public Vector3 reflective;
        public Vector3 rough_ior_metal;

        public Material(Color eta, Color rho, float r, float i, float m)
        {
            emissive = new Vector3(eta.r, eta.g, eta.b) * 1;
            reflective = new Vector3(rho.r, rho.g, rho.b);
            rough_ior_metal = new Vector3(r, i, m); 
        }
    };
    
    public struct Plane
    {
        public Vector3 pos;
        public Vector3 n;
        public Material mat;
        public Plane(Vector3 pos, Vector3 n, Material mat)
        {
            this.pos = pos;
            this.n = n;
            this.mat = mat;
        }
    };

    public struct Sphere
    {
        public Vector4 centre;
        public Material mat;

        public Sphere(Vector4 centre, Material mat)
        {
            this.centre = centre;
            this.mat = mat;
        }
    };

    public struct Box
    {
        public Matrix4x4 pos_n;
        public Material mat;

        public Box(Matrix4x4 posN, Material mat)
        {
            pos_n = posN;
            this.mat = mat;
        }
    };

    public struct Disc
    {
        public Vector3 pos;
        public Vector4 n;
        public Material mat;

        public Disc(Vector3 pos, Vector4 n, Material mat)
        {
            this.pos = pos;
            this.n = n;
            this.mat = mat;
        }
    };
    
    public struct Quad
    {
        public Vector4 pos;
        public Vector4 n;
        public Material mat;

        public Quad(Vector4 pos, Vector4 n, Material mat)
        {
            this.pos = pos;
            this.n = n;
            this.mat = mat;
        }
    };
    
    

}
