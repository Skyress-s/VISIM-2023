    using UnityEditor;
    using UnityEngine;

    [CustomEditor(typeof(PointCloudRenderer))]
    public class PointCloudRendererEditor : Editor {
        public override void OnInspectorGUI() {
            base.OnInspectorGUI();
            if (GUILayout.Button("Calculate and Set num points in current file")) {
                
            }
        }
    }