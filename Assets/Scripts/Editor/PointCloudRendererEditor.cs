    using UnityEditor;
    using UnityEngine;

    [CustomEditor(typeof(PointCloudRenderer))]
    public class PointCloudRendererEditor : Editor {
        public override void OnInspectorGUI() {
            base.OnInspectorGUI();
            if (GUILayout.Button("Bake Input file \"merged.txt\" to \"modified.txt\"")) {
                (target as PointCloudRenderer).ConvertToModifiedFile();
            }
        }
    }