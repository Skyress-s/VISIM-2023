using System.Collections.Generic;
using KT;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class TriangleSurface : MonoBehaviour {
    [SerializeField] private TextAsset verticesData;
    [SerializeField] private TextAsset indicesData;

    private List<Vector3> vertices  ;
    private List<int> indices;
    private void Start() {
        vertices = new();
        indices = new();
        ReadData(ref vertices, ref indices);

        // for (int i = 0; i < indices.Count/3; i++) {
        // int temp = indices[i * 3 + 0];
        // indices[i * 3 + 0] = indices[i * 3 + 2];
        // indices[i * 3 + 2] = temp;
        // }
        
        Mesh mesh = new Mesh();
        mesh.vertices = vertices.ToArray();
        mesh.triangles = indices.ToArray();
        
        mesh.RecalculateNormals();
        GetComponent<MeshFilter>().mesh = mesh;
    }

    public List<CollisionTriangle> GetTriangles() {
        List<Vector3> vertices = new();
        List<int> indices = new();
        ReadData(ref vertices, ref indices);

        List<CollisionTriangle> triangles = new();
        for (int i = 0; i < indices.Count / 3; i++) {
            Vector3 x = vertices[indices[i*3]];
            Vector3 y = vertices[indices[i*3+1]];
            Vector3 z = vertices[indices[i*3+2]];
            CollisionTriangle triangle = new CollisionTriangle(x, y, z);
            triangles.Add(triangle);
        }

        return triangles;
    }
    private void OnDrawGizmos() {
        List<CollisionTriangle> tris = GetTriangles();
        for (int i = 0; i < tris.Count; i++) {
            tris[i].DebugDraw(UnityEngine.Color.red);
        }
    }

    void ReadData(ref List<Vector3> verts, ref List<int> indices) {
        var vertStringData = verticesData.text.Split('\n');
        int numVerts = int.Parse(vertStringData[0]);
        // Debug.Log($"Num Verts {numVerts}");
        
        for (int i = 1; i < numVerts + 1; i++) {
            var compData = vertStringData[i].Split(' ');
            verts.Add(new Vector3(float.Parse(compData[0]),float.Parse(compData[1]), float.Parse(compData[2]) ));
            // verts[verts.Count - 1] *= 0.01f;
        }
        
        var indicesStringData =indicesData.text.Split('\n');
        int NumIndices = int.Parse(indicesStringData[0]);
        
        // List<int> indices = new();
        for (int i = 1; i < NumIndices + 1; i++) {
            var compData = indicesStringData[i].Split(' ');
            indices.Add(int.Parse(compData[0]));
            indices.Add(int.Parse(compData[1]));
            indices.Add(int.Parse(compData[2]));
        }
    }

    // public int GetTriangleFromPosition(Vector3 position) {
    //     
    // }
    //
    // public 
}