using System.Collections.Generic;
using UnityEngine;

namespace UnityRayTracing
{
    using static Extension;
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
        public static List<Sphere> Spheres;
        public static List<Quad> Quads;
        public static List<Box> Boxes;
        public static List<Plane> Planes;
        public static List<Disc> Discs;

        private void MakeSphere()
        {
            if (type != ObjectType.Sphere) return;
            var t = transform;
            Spheres.Add(new Sphere(BuildVec4(t.position, t.localScale.x), 
                new Material(emissive, reflective)));
        }
        
        private void MakeBox()
        {
            if (type != ObjectType.Box) return;
            var t = transform;
            var pos = BuildVec4(t.position, 1);
            var scale = t.localScale;
            var x = BuildVec4(t.right, scale.x);
            var y = BuildVec4(t.up, scale.y);
            var z = BuildVec4(t.forward, scale.z);
            var box = new Matrix4x4(pos, x, y, z).transpose;

            Boxes.Add(new Box(box, new Material(emissive, reflective)));
        }
        
        private void MakePlane()
        {
            if (type != ObjectType.Plane) return;
            var t = transform;
            Planes.Add(new Plane(t.position, t.up, new Material(emissive, reflective)));
        }
        
        private void MakeQuad()
        {
            if (type != ObjectType.Quad) return;
            var t = transform;
            var scale = t.localScale;
            var q = new Quad(BuildVec4(t.position, scale.x),
                BuildVec4(t.forward, scale.y), 
                new Material(emissive, reflective));
            Quads.Add(new Quad());
        }

        private void MakeDisc()
        {
            if (type != ObjectType.Quad) return;
            var t = transform;
            var scale = t.localScale;
        }

        private static Vector4 BuildVec4(in Vector3 v, float w)
        {
            var output = Vector4.zero;
            output.Assign(v);
            output.w = w;
            return output;
        }

    }
}