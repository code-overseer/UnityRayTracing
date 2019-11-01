using System.IO;
using UnityEditor;
using UnityEngine;
using UnityRayTracing;

namespace Editor
{
    [CustomEditor(typeof(RayTracingMain))]
    public class RayTracingMainEditor : UnityEditor.Editor
    {

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            if (!GUILayout.Button("Screenshot")) return;
            
            var count = Directory.GetFiles("Assets/Screenshots/").Length / 2 + 1;
            var path = Path.Combine(Directory.GetParent("Assets/Screenshots/").FullName, 
                $"screenshot{count:00}.png");
            ScreenCapture.CaptureScreenshot(path);
        }
    }
}
