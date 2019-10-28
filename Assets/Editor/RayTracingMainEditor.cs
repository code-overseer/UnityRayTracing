using UnityEditor;
using UnityEngine;

namespace Editor
{
    [CustomEditor(typeof(RayTracingMain))]
    public class RayTracingMainEditor : UnityEditor.Editor
    {

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            if (!GUILayout.Button("Screenshot")) return;
            ScreenCapture.CaptureScreenshot("/Users/bryanwong/Downloads/screenshot.png");
//            var script = target as RayTracingMain;
//            if (script == null) return;
//            if (!script.Capture) script.Capture = true;
        }
    }
}
