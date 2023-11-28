using System;
using UnityEngine;

namespace KT.Utility {
    public class TimeManager : MonoBehaviour {
        private void OnGUI() {
            GUILayout.BeginHorizontal();
            Time.timeScale =  GUILayout.HorizontalSlider(Time.timeScale, 0.001f, 10f, GUILayout.MinWidth(100));
            if (GUILayout.Button("Reset")) {
                Time.timeScale = 1f;
            }
            GUILayout.EndHorizontal();
        }
    }
}