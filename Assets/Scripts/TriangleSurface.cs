using System.Collections.Generic;
using KT;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
// TASK 2.4 - Reading in data and drawing
public class TriangleSurface : MonoBehaviour {
    [SerializeField] private TextAsset verticesData;
    [SerializeField] private TextAsset indicesData;

    [SerializeField] private bool bDrawDebug = false;

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
        
        
        // Make sure to reset data when starting, as we dont reload our domain when we start the game.
        vertices = new();
        indices = new();
        ReadData(ref vertices, ref indices);
        
        // Create mesh from Data
        Mesh mesh = new Mesh();
        mesh.vertices = vertices.ToArray();
        mesh.triangles = indices.ToArray();
        
        // Make sure Normals are in order
        mesh.RecalculateNormals();
        
        // Set the mesh in Unity on connected GameObject
        GetComponent<MeshFilter>().mesh = mesh;
    }

    /// <summary>
    /// Helper function to get the number of quads per side.
    /// The calculation program always gives the vertices in order of x coordinate ascending, so we can use this to find the number of quads per side.
    /// </summary>
    /// <returns></returns>
    public int GetNumQuadsPerSide() {
        int quadsPerSide = 0;
        while (vertices[quadsPerSide+1].x > vertices[quadsPerSide].x) { // The calculation program always gives the vertices in order of x coordinate ascending, so we can use this to find the number of quads per side.
            quadsPerSide++;
        }

        return quadsPerSide;
    }
    
    /// <summary>
    /// Using QuadsPerSide to find the triangle we are in. If outside triangulation area, return null.
    /// </summary>
    /// <param name="position"></param>
    /// <returns></returns>
    public CollisionTriangle? GetTriangleFromPosition(Vector3 position) {
        int quadsPerSide = GetNumQuadsPerSide(); // Is Valid for both x and z directions as they have the same amount of quads

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
    
    /// <summary>
    /// Return all triangles
    /// If program thinks we have alleready initialzed vertices and indices dont read them in again.
    /// </summary>
    /// <param name="bForceUpdate">Force to readItems again. Useful for code that only runs EditTime</param>
    /// <returns></returns>
    public List<CollisionTriangle> GetTriangles(bool bForceUpdate = false) {
        if (bForceUpdate || vertices.Count == 0 || indices.Count == 0) {
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
        if (!bDrawDebug) 
            return;
        
        List<CollisionTriangle> tris = GetTriangles(true);
        for (int i = 0; i < tris.Count; i++) {
            tris[i].DebugDraw(UnityEngine.Color.red);
        }
    }

    /// <summary>
    /// Reads in vertices and indices from text files.
    /// </summary>
    /// <param name="verts"></param>
    /// <param name="indices"></param>
    void ReadData(ref List<Vector3> verts, ref List<int> indices) {
        var vertStringData = verticesData.text.Split('\n');
        int numVerts = int.Parse(vertStringData[0]);
        
        for (int i = 1; i < numVerts + 1; i++) {
            var compData = vertStringData[i].Split(' ');
            verts.Add(new Vector3(float.Parse(compData[0]),float.Parse(compData[1]), float.Parse(compData[2]) ));
        }
        
        var indicesStringData =indicesData.text.Split('\n');
        int NumIndices = int.Parse(indicesStringData[0]);
        
        for (int i = 1; i < NumIndices + 1; i++) {
            var compData = indicesStringData[i].Split(' ');
            indices.Add(int.Parse(compData[0]));
            indices.Add(int.Parse(compData[1]));
            indices.Add(int.Parse(compData[2]));
        }
    }
}