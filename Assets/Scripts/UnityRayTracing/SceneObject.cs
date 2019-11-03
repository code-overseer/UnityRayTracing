using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.UIElements;

namespace UnityRayTracing
{
    public enum ObjectType
    {
        Sphere,
        Quad,
        Box,
        Plane,
        Disc
    }
    
    public class SceneObject : MonoBehaviour
    {
        public ObjectType type;
        public Color emissive;
        public Color reflective;
        [Range(0.0F, 1.0F)]
        public float roughness;
        [Range(1.0F, 6.0F)]
        public float indexOfReflection = 1;
        public bool metallic;
        public static List<Sphere> Spheres = new List<Sphere>{new Sphere()};
        public static List<Quad> Quads = new List<Quad>(){new Quad()};
        public static List<Box> Boxes = new List<Box>{new Box()};
        public static List<Plane> Planes = new List<Plane>{new Plane()};
        public static List<Disc> Discs = new List<Disc>{new Disc()};
        
        private void Awake()
        {
            MakeSphere();
            MakeBox();
            MakePlane();
            MakeQuad();
            MakeDisc();
            gameObject.GetComponent<MeshRenderer>().enabled = false;
        }
        
        private void MakeSphere()
        {
            if (type != ObjectType.Sphere) return;
            var m = System.Convert.ToSingle(metallic);
            var t = transform;
            Spheres.Add(new Sphere(BuildVec4(t.position, t.localScale.x * 0.5f), 
                new Material(emissive, reflective, roughness,indexOfReflection, m)));
        }
        
        private void MakeBox()
        {
            if (type != ObjectType.Box) return;
            var t = transform;
            var pos = BuildVec4(t.position, 1);
            var scale = t.localScale * 0.5f;
            var x = BuildVec4(t.right, scale.x);
            var y = BuildVec4(t.up, scale.y);
            var z = BuildVec4(t.forward, scale.z);
            var box = new Matrix4x4(pos, x, y, z).transpose;
            var m = System.Convert.ToSingle(metallic);
            Boxes.Add(new Box(box, new Material(emissive, reflective, roughness,indexOfReflection, m)));
        }
        
        private void MakePlane()
        {
            if (type != ObjectType.Plane) return;
            var t = transform;
            var m = System.Convert.ToSingle(metallic);
            Planes.Add(new Plane(t.position, t.up, new Material(emissive, reflective, roughness,indexOfReflection, m)));
        }
        
        private void MakeQuad()
        {
            if (type != ObjectType.Quad) return;
            var t = transform;
            var scale = t.localScale * 0.5f;
            var m = System.Convert.ToSingle(metallic);
            var q = new Quad(BuildVec4(t.position, scale.x),
                BuildVec4(t.forward, scale.y), 
                new Material(emissive, reflective, roughness,indexOfReflection, m));
            Quads.Add(q);
        }

        private void MakeDisc()
        {
            if (type != ObjectType.Disc) return;
            var t = transform;
            var scale = t.localScale * 0.5f;
            var m = System.Convert.ToSingle(metallic);
            var d = new Disc(t.position, BuildVec4(t.up, scale.x), new Material(emissive, reflective, roughness,indexOfReflection, m));
            Discs.Add(d);
        }

        private static Vector4 BuildVec4(in Vector3 v, float w)
        {
            var output = Vector4.zero;
            output.x = v.x;
            output.y = v.y;
            output.z = v.z;
            output.w = w;
            return output;
        }

    }
}