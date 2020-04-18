using System.IO;
using UnityEditor;
using UnityEngine;
using UnityRayTracing;

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

        private static GUILayoutOption MatrixFieldWidth => GUILayout.Width(EditorGUIUtility.currentViewWidth * 0.15f);
        private static float SideBorderSpace => EditorGUIUtility.currentViewWidth * 0.125f;
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            var ctw = CameraObj.cameraToWorldMatrix;
            var cip = CameraObj.projectionMatrix.inverse;

            GUILayout.Label ("Camera To World: ");
            
            GUILayout.BeginHorizontal();
            GUILayout.Space(SideBorderSpace);
            GUILayout.TextArea($"{ctw.GetRow(0).x:0.000}", MatrixFieldWidth);
            GUILayout.TextArea($"{ctw.GetRow(0).y:0.000}", MatrixFieldWidth);
            GUILayout.TextArea($"{ctw.GetRow(0).z:0.000}", MatrixFieldWidth);
            GUILayout.TextArea($"{ctw.GetRow(0).w:0.000}", MatrixFieldWidth);
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            GUILayout.Space(SideBorderSpace);
            GUILayout.TextArea($"{ctw.GetRow(1).x:0.000}", MatrixFieldWidth);
            GUILayout.TextArea($"{ctw.GetRow(1).y:0.000}", MatrixFieldWidth);
            GUILayout.TextArea($"{ctw.GetRow(1).z:0.000}", MatrixFieldWidth);
            GUILayout.TextArea($"{ctw.GetRow(1).w:0.000}", MatrixFieldWidth);
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            GUILayout.Space(SideBorderSpace);
            GUILayout.TextArea($"{ctw.GetRow(2).x:0.000}", MatrixFieldWidth);
            GUILayout.TextArea($"{ctw.GetRow(2).y:0.000}", MatrixFieldWidth);
            GUILayout.TextArea($"{ctw.GetRow(2).z:0.000}", MatrixFieldWidth);
            GUILayout.TextArea($"{ctw.GetRow(2).w:0.000}", MatrixFieldWidth);
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            GUILayout.Space(SideBorderSpace);
            GUILayout.TextArea($"{ctw.GetRow(3).x:0.000}", MatrixFieldWidth);
            GUILayout.TextArea($"{ctw.GetRow(3).y:0.000}", MatrixFieldWidth);
            GUILayout.TextArea($"{ctw.GetRow(3).z:0.000}", MatrixFieldWidth);
            GUILayout.TextArea($"{ctw.GetRow(3).w:0.000}", MatrixFieldWidth);
            GUILayout.EndHorizontal();
            GUILayout.Label ("Inverse Projection: ");
            GUILayout.BeginHorizontal();
            GUILayout.Space(SideBorderSpace);
            GUILayout.TextArea($"{cip.GetRow(0).x:0.000}", MatrixFieldWidth);
            GUILayout.TextArea($"{cip.GetRow(0).y:0.000}", MatrixFieldWidth);
            GUILayout.TextArea($"{cip.GetRow(0).z:0.000}", MatrixFieldWidth);
            GUILayout.TextArea($"{cip.GetRow(0).w:0.000}", MatrixFieldWidth);
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            GUILayout.Space(SideBorderSpace);
            GUILayout.TextArea($"{cip.GetRow(1).x:0.000}", MatrixFieldWidth);
            GUILayout.TextArea($"{cip.GetRow(1).y:0.000}", MatrixFieldWidth);
            GUILayout.TextArea($"{cip.GetRow(1).z:0.000}", MatrixFieldWidth);
            GUILayout.TextArea($"{cip.GetRow(1).w:0.000}", MatrixFieldWidth);
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            GUILayout.Space(SideBorderSpace);
            GUILayout.TextArea($"{cip.GetRow(2).x:0.000}", MatrixFieldWidth);
            GUILayout.TextArea($"{cip.GetRow(2).y:0.000}", MatrixFieldWidth);
            GUILayout.TextArea($"{cip.GetRow(2).z:0.000}", MatrixFieldWidth);
            GUILayout.TextArea($"{cip.GetRow(2).w:0.000}", MatrixFieldWidth);
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            GUILayout.Space(SideBorderSpace);
            GUILayout.TextArea($"{cip.GetRow(3).x:0.000}", MatrixFieldWidth);
            GUILayout.TextArea($"{cip.GetRow(3).y:0.000}", MatrixFieldWidth);
            GUILayout.TextArea($"{cip.GetRow(3).z:0.000}", MatrixFieldWidth);
            GUILayout.TextArea($"{cip.GetRow(3).w:0.000}", MatrixFieldWidth);
            GUILayout.EndHorizontal();

        }
    }
}