using UnityEngine;

namespace UnityRayTracing
{
    public static class Extension
    {
        public static void Assign(this Vector4 vec4, Vector3 vec3)
        {
            vec4.x = vec3.x;
            vec4.y = vec3.y;
            vec4.z = vec3.z;
        }
    }
}