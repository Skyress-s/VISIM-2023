using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointCloudRenderer : MonoBehaviour {
    [SerializeField] private TextAsset pointCloudData;
    
    private int NumLinesInFile(TextAsset asset) {
        var list = asset.text.Split('\n');
        list.Length
    }
    
}
