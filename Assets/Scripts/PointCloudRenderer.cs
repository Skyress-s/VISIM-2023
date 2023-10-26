using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class PointCloudRenderer : MonoBehaviour {
    [SerializeField] private TextAsset pointCloudData;
    [SerializeField] private Mesh _mesh;
    [SerializeField] private Material _material;
    GraphicsBuffer meshTriangles;
    GraphicsBuffer meshPositions;
    private int NumLinesInFile(TextAsset asset) {
        var list = asset.text.Split('\n');
        
        return list.Length;
    }

    [ContextMenu("Test")]
    private void Create() {

        string path = Application.dataPath + "/Data/Hoydedata/";
        List<string> lines = new();
        int numLines = 0;
        using (StreamReader inputFile = new StreamReader(path + "merged.txt")) {
            while (!inputFile.EndOfStream) {
                lines.Add(inputFile.ReadLine());
                numLines++;
            }
        }
        
        
        using (StreamWriter outputFile = new StreamWriter(path + "new.txt"))
        {
            outputFile.WriteLine(numLines);
            foreach (var line in lines) {
                outputFile.WriteLine(line);
            }
        }
        
        AssetDatabase.SaveAssets();
        
        
    }

    private List<Vector3> points = new();
    void Start()
    {
        // note: remember to check "Read/Write" on the mesh asset to get access to the geometry data
        meshTriangles = new GraphicsBuffer(GraphicsBuffer.Target.Structured, _mesh.triangles.Length, sizeof(int));
        meshTriangles.SetData(_mesh.triangles);
        meshPositions = new GraphicsBuffer(GraphicsBuffer.Target.Structured, _mesh.vertices.Length, 3 * sizeof(float));
        for (int i = 0; i < 10; i++) {
            points.Add(new Vector3(0, 1*i,0));
        }
        meshPositions.SetData(_mesh.vertices);
    }

    void OnDestroy()
    {
        meshTriangles?.Dispose();
        meshTriangles = null;
        meshPositions?.Dispose();
        meshPositions = null;
    }

    void Update()
    {
        RenderParams rp = new RenderParams(_material);
        rp.worldBounds = new Bounds(Vector3.zero, 10000*Vector3.one); // use tighter bounds
        rp.matProps = new MaterialPropertyBlock();
        rp.matProps.SetBuffer("_Triangles", meshTriangles);
        rp.matProps.SetBuffer("_Positions", meshPositions);
        rp.matProps.SetInt("_StartIndex", (int)_mesh.GetIndexStart(0));
        rp.matProps.SetInt("_BaseVertexIndex", (int)_mesh.GetBaseVertex(0));
        rp.matProps.SetMatrix("_ObjectToWorld", Matrix4x4.Translate(new Vector3(-4.5f, 0, 0)));
        rp.matProps.SetFloat("_NumInstances", 10.0f);
        Graphics.RenderPrimitives(rp, MeshTopology.Triangles, (int)_mesh.GetIndexCount(0), 3);
    }
}