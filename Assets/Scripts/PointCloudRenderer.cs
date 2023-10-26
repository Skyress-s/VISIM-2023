using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

public class PointCloudRenderer : MonoBehaviour {
    [SerializeField] private TextAsset pointCloudData;
    [SerializeField] private Mesh _mesh;
    [SerializeField] private Material _material;
    GraphicsBuffer meshTriangles;
    GraphicsBuffer meshPositions;
    GraphicsBuffer vertexPositions;
    
    string basePath => Application.dataPath + "/Data/Hoydedata/";
    private readonly string modifiedFileName = "modified.txt";
    private readonly string originalFileName = "merged.txt";
    
    
    private int NumLinesInFile(TextAsset asset) {
        var list = asset.text.Split('\n');
        
        return list.Length;
    }

    [ContextMenu("CovertToModifiedFile")]
    private void ConvertToModifiedFile() {

        points = new();
        int numLines = 0;
        int a = 0;
        // var a = ReadAllLinesAsync(path + "test.txt", Encoding.UTF8).GetAwaiter().GetResult();
        // return;
        Vector3? first = null;
        using (StreamReader inputFile = new StreamReader(basePath + originalFileName)) {
            while (!inputFile.EndOfStream) {
                var strings = inputFile.ReadLine().Split(' ');
                if (strings.Length != 3) {
                    continue;
                }
                
                a++;
                if (a != 500)
                    continue;

                numLines++;
                a = 0;

                var p = new Vector3(float.Parse(strings[0]), float.Parse(strings[2]), float.Parse(strings[1]));
                Debug.Log(p);
                // p /= 100000f;
                if (first == null) {
                    first = p;
                }
                points.Add((Vector3)(p - first));
                // if (numLines == 1000000) {
                // break;
                // }
            }
        }
        
        if(File.Exists(basePath + modifiedFileName))
        {
            File.Delete(basePath + modifiedFileName);
        }
        // Write to file
        using (StreamWriter outputFile = new StreamWriter(basePath + modifiedFileName))
        {
            outputFile.WriteLine(numLines);
            foreach (var line in points) {
                outputFile.WriteLine(line[0] + " " + line[1] + " " + line[2]);
            }
        }
        
        AssetDatabase.SaveAssets();
        
        
    }
    public static async Task<string[]> ReadAllLinesAsync(string path, Encoding encoding)
    {
        var lines = new List<string>();

        // Open the FileStream with the same FileMode, FileAccess
        // and FileShare as a call to File.OpenText would've done.
        using (var stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read, 4096, FileOptions.Asynchronous | FileOptions.SequentialScan))
        using (var reader = new StreamReader(stream, encoding))
        {
            string line;
            while ((line = await reader.ReadLineAsync()) != null)
            {
                lines.Add(line);
            }
        }

        return lines.ToArray();
    }
    private List<Vector3> points = new();
    void Start()
    {
        points = new();
        // int a = 0;
        
        using (StreamReader inputFile = new StreamReader(basePath + modifiedFileName)) {
            while (!inputFile.EndOfStream) {
                var strings = inputFile.ReadLine().Split(' ');
                if (strings.Length != 3) 
                    continue;
               
                var p = new Vector3(float.Parse(strings[0]), float.Parse(strings[1]), float.Parse(strings[2]));
                Debug.Log(p);
                points.Add(p);
            }
        }
        
        SceneView.lastActiveSceneView.camera.transform.position = points[0];
        // note: remember to check "Read/Write" on the mesh asset to get access to the geometry data
        meshTriangles = new GraphicsBuffer(GraphicsBuffer.Target.Structured, _mesh.triangles.Length, sizeof(int));
        meshTriangles.SetData(_mesh.triangles);
        meshPositions = new GraphicsBuffer(GraphicsBuffer.Target.Structured, points.Count, 3 * sizeof(float));
        meshPositions.SetData(points.ToArray());
        vertexPositions = new GraphicsBuffer(GraphicsBuffer.Target.Structured, _mesh.vertices.Length, 3 * sizeof(float));
        vertexPositions.SetData(_mesh.vertices);
    }

    void OnDestroy()
    {
        meshTriangles?.Dispose();
        meshTriangles = null;
        meshPositions?.Dispose();
        meshPositions = null;
        vertexPositions?.Dispose();
        vertexPositions = null;
    }

    void Update()
    {
        RenderParams rp = new RenderParams(_material);
        rp.worldBounds = new Bounds(Vector3.zero, 10000*Vector3.one); // use tighter bounds
        rp.matProps = new MaterialPropertyBlock();
        rp.matProps.SetBuffer("_Triangles", meshTriangles);
        rp.matProps.SetBuffer("_Positions", meshPositions);
        rp.matProps.SetBuffer("_VertexPositions", vertexPositions);
        rp.matProps.SetInt("_StartIndex", (int)_mesh.GetIndexStart(0));
        rp.matProps.SetInt("_BaseVertexIndex", (int)_mesh.GetBaseVertex(0));
        rp.matProps.SetMatrix("_ObjectToWorld", Matrix4x4.Translate(new Vector3(-4.5f, 0, 0)));
        rp.matProps.SetFloat("_NumInstances", 10.0f);
        Graphics.RenderPrimitives(rp, MeshTopology.Triangles, (int)_mesh.GetIndexCount(0), points.Count);
    }
}