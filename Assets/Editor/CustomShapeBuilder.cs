using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Editor
{
    public class CustomShapeBuilder : EditorWindow
    {
        
        private static GUILayoutOption FloatBoxWidth => GUILayout.Width(EditorGUIUtility.currentViewWidth * 0.25f);
        private static Vector3 Pivot => SceneView.lastActiveSceneView.pivot;
        private static Material _defaultMaterial;

        private static Material GetMaterial
        {
            get
            {
                if (_defaultMaterial != null) return _defaultMaterial;
                var primitive = GameObject.CreatePrimitive(PrimitiveType.Plane);
                _defaultMaterial = primitive.GetComponent<MeshRenderer>().sharedMaterial;
                DestroyImmediate(primitive);
                return _defaultMaterial;
            }
        }
        
        [MenuItem("GameObject/3D Object/Disc")]
        private static void MakeDisc()
        {
            var go = new GameObject("Disc");
            go.transform.position = Pivot;

            var filter = go.AddComponent<MeshFilter>();
            filter.mesh = BuildDisc();
            var renderer = go.AddComponent<MeshRenderer>();
            renderer.sharedMaterial = GetMaterial;
        }

        private static Mesh BuildDisc()
        {
            const float radius = 0.5f;
            const int sides = 64;
            const int verts = sides + 1;
            const float _2PI = 2 * Mathf.PI;
            var v = 0;
            var vertices = new Vector3[verts];
            var normals = new Vector3[vertices.Length];
            var uvs = new Vector2[vertices.Length];
            var triangles = new int[3 * sides];

            vertices[v] = Vector3.zero;
            uvs[v++] = Vector2.one * 0.5f;
            while (v <= sides)
            {
                var angle = (float) v / sides * _2PI;
                var cosi = Mathf.Cos(angle);
                var sini = -Mathf.Sin(angle);
                vertices[v] = new Vector3( cosi * radius, 0.0f,  sini * radius);
                uvs[v] = new Vector2(cosi * 0.5f + 0.5f, sini * 0.5f + 0.5f);
                ++v;
            }

            for (v = 0; v <= sides; ++v)
            {
                normals[v] = Vector3.up;
            }
            
            var tri = 0;
            var i = 0;
            while (tri < sides - 1)
            {
                triangles[i] = tri + 1;
                triangles[i + 1] = tri + 2;
                triangles[i + 2] = 0;
                tri++;
                i += 3;
            }
            triangles[i] = tri + 1;
            triangles[i + 1] = 1;
            triangles[i + 2] = 0;
            var output = new Mesh {name = "Disc", vertices = vertices, triangles = triangles, uv = uvs, normals = normals};
            output.RecalculateBounds();
            return output;
        }
    }
}
