using System.Collections.Generic;
using KT;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class TriangleSurface : MonoBehaviour {
    [SerializeField] private TextAsset verticesData;
    [SerializeField] private TextAsset indicesData;

    private List<Vector3> vertices = new() ;
    private List<int> indices = new();
    
    // Decieded to use Singleton as i only want one triangle surface in the scene for this exam.
    public static TriangleSurface Instance { get; private set; }
    
    
    // Refrences
    private Renderer _renderer;
    private void Start() {
        // Simple Singleton pattern
        if (Instance == null) {
            Instance = this;
        }
        else {
            Destroy(this);
        }
        
        _renderer = GetComponent<Renderer>();
        
        
        
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

    public int GetQuadsPerSide() {
        int quadsPerSide = 0;
        while (vertices[quadsPerSide+1].x > vertices[quadsPerSide].x) { // The calculation program always gives the vertices in order of x coordinate ascending, so we can use this to find the number of quads per side.
            quadsPerSide++;
        }

        return quadsPerSide;
    }
    public CollisionTriangle? GetTriangleFromPosition(Vector3 position) {
        int quadsPerSide = GetQuadsPerSide(); // Is Valid for both x and z directions as they have the same amount of quads

        var bounds = _renderer.bounds;
        
        // X
        float lengthXSide = bounds.size.x;
        float positionLengthXSize = position.x + bounds.extents.x;
        float tx = positionLengthXSize / lengthXSide;
        int ix = (int)(tx * quadsPerSide);
        
        // Z
        float lengthZSide = bounds.size.z;
        float positionLengthZSize = position.z + bounds.extents.z;
        float tz = positionLengthZSize / lengthZSide;
        int iz = (int)(tz * quadsPerSide);
        
        int maxQuads = quadsPerSide * quadsPerSide;
        int quadNr = ix + iz * quadsPerSide;
        if (quadNr >= maxQuads || quadNr < 0) {
            return null;
        }
        // Draw Quad
        
        int startIndex = quadNr * 6;
        var x =vertices[indices[ startIndex]];
        var y = vertices[indices[startIndex + 1]];
        var z = vertices[indices[startIndex + 2]];

        CollisionTriangle tri = new CollisionTriangle(x, y, z);
        if (tri.InTriangle(position)) {
            return tri;
        }
        
        x =vertices[indices[ startIndex + 3]];
        y = vertices[indices[startIndex + 1 + 3]];
        z = vertices[indices[startIndex + 2 + 3]];


        tri = new CollisionTriangle(x, y, z);
        if (tri.InTriangle(position)) {
            return tri;
        }
        
        return null;
    }
    
    public List<CollisionTriangle> GetTriangles() {
        if (vertices.Count == 0 || indices.Count == 0) {
            vertices = new();
            indices = new();
            ReadData(ref vertices, ref indices);
        }

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
        return;
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