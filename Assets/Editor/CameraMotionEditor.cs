using System.IO;
using UnityEditor;
using UnityEngine;

namespace Editor
{
    [CustomEditor(typeof(CameraMotion))]
    public class CameraMotionEditor : UnityEditor.Editor
    {
        private Camera _camera;

        private Camera CameraObj
        {
            get
            {
                if (_camera == null)
                {
                    _camera = ((CameraMotion)target).GetComponent<Camera>();
                }
                return _camera;
            }
        }
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            var ctw = CameraObj.cameraToWorldMatrix;
            var cip = CameraObj.projectionMatrix.inverse;
            GUILayout.Label ("Camera To World: ");
            GUILayout.BeginHorizontal();
            GUILayout.TextArea($"{ctw.m00:0.000}");
            GUILayout.TextArea($"{ctw.m10:0.000}");
            GUILayout.TextArea($"{ctw.m20:0.000}");
            GUILayout.TextArea($"{ctw.m30:0.000}");
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            GUILayout.TextArea($"{ctw.m01:0.000}");
            GUILayout.TextArea($"{ctw.m11:0.000}");
            GUILayout.TextArea($"{ctw.m21:0.000}");
            GUILayout.TextArea($"{ctw.m31:0.000}");
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            GUILayout.TextArea($"{ctw.m02:0.000}");
            GUILayout.TextArea($"{ctw.m12:0.000}");
            GUILayout.TextArea($"{ctw.m22:0.000}");
            GUILayout.TextArea($"{ctw.m32:0.000}");
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            GUILayout.TextArea($"{ctw.m03:0.000}");
            GUILayout.TextArea($"{ctw.m13:0.000}");
            GUILayout.TextArea($"{ctw.m23:0.000}");
            GUILayout.TextArea($"{ctw.m33:0.000}");
            GUILayout.EndHorizontal();
            GUILayout.Space(10);
            GUILayout.Label ("Inverse Projection: ");
            GUILayout.BeginHorizontal();
            GUILayout.TextArea($"{cip.m00:0.000}");
            GUILayout.TextArea($"{cip.m10:0.000}");
            GUILayout.TextArea($"{cip.m20:0.000}");
            GUILayout.TextArea($"{cip.m30:0.000}");
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            GUILayout.TextArea($"{cip.m01:0.000}");
            GUILayout.TextArea($"{cip.m11:0.000}");
            GUILayout.TextArea($"{cip.m21:0.000}");
            GUILayout.TextArea($"{cip.m31:0.000}");
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            GUILayout.TextArea($"{cip.m02:0.000}");
            GUILayout.TextArea($"{cip.m12:0.000}");
            GUILayout.TextArea($"{cip.m22:0.000}");
            GUILayout.TextArea($"{cip.m32:0.000}");
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            GUILayout.TextArea($"{cip.m03:0.000}");
            GUILayout.TextArea($"{cip.m13:0.000}");
            GUILayout.TextArea($"{cip.m23:0.000}");
            GUILayout.TextArea($"{cip.m33:0.000}");
            GUILayout.EndHorizontal();

        }
    }
}