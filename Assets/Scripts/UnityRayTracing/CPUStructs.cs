using UnityEngine;
// ReSharper disable InconsistentNaming
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable NotAccessedField.Global

namespace UnityRayTracing
{
    public struct Material
    {
        public Vector3 emissive;
        public Vector3 reflective;
        public Material(Vector3 eta, Vector3 rho)
        {
            emissive = eta;
            reflective = rho;
        }
        
        public Material(Color eta, Color rho)
        {
            emissive = new Vector3(eta.r, eta.g, eta.b);
            reflective = new Vector3(rho.r, rho.g, rho.b);
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
