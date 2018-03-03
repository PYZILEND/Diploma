using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

///<summary>
///Physical map takes care of the mesh, collider and rendering the map.
/// </summary>
[RequireComponent(typeof(MeshFilter), (typeof(MeshRenderer)))]
public class PhysicalMap : MonoBehaviour {

    Mesh mesh;
    MeshCollider meshCollider;

    //Data used to build mesh
    List<Vector3> vertices;
    List<int> triangles;   

    /// <summary>
    /// Initializes physical map fields.
    /// </summary>
    void Awake()
    {
        GetComponent<MeshFilter>().mesh = mesh = new Mesh();
        mesh.name = "MapMesh";
        meshCollider = gameObject.AddComponent<MeshCollider>();
        vertices = new List<Vector3>();
        triangles = new List<int>();
    }

    /// <summary>
    /// Creates a mesh to represent a map with said width and height.
    /// </summary>
    /// <param name="width">Width of the map in hexagons</param>
    /// <param name="height">Height of the map in hexagons</param>
    public void CreateMap(int width, int height)
    {
        //Clears old mesh data
        mesh.Clear();
        vertices.Clear();
        triangles.Clear();

        //Creating cells
        for (int z = 0; z < height; z++)
        {
            for (int x = 0; x < width; x++)
            {
                CreateCell(x, z);
            }
        }

        //Applyes generated data to mesh
        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.RecalculateNormals();
        meshCollider.sharedMesh = mesh;
    }

    /// <summary>
    /// Creates mesh for a cell at said offset coordinates.
    /// </summary>
    /// <param name="x">X offset coordinate</param>
    /// <param name="z">Z offset coordinate</param>
    void CreateCell(int x, int z)
    {
        //Calculating cell's center position
        Vector3 position;
        position.x = (x + z * 0.5f - z / 2) * (2f * HexMetrics.innerRadius);
        position.y = 0f;
        position.z = z * (1.5f * HexMetrics.outerRadius);

        TriangulateCell(position);
    }

    /// <summary>
    /// Creates triangles for each side of the hexagon
    /// </summary>
    /// <param name="center">Center of the hexagon</param>
    void TriangulateCell(Vector3 center)
    {
        for(int i = 0; i < HexMetrics.corners.Length - 1; i++)
        {
            AddTriangle(center, center + HexMetrics.corners[i], center + HexMetrics.corners[i + 1]);
        }
    }

    /// <summary>
    /// Adds triangle to the mesh
    /// </summary>
    /// <param name="v1">First vertex</param>
    /// <param name="v2">Second vertex</param>
    /// <param name="v3">Third vertex</param>
    void AddTriangle(Vector3 v1, Vector3 v2, Vector3 v3)
    {
        int verticesIndex = vertices.Count;
        vertices.Add(v1);
        vertices.Add(v2);
        vertices.Add(v3);
        triangles.Add(verticesIndex);
        triangles.Add(verticesIndex + 1);
        triangles.Add(verticesIndex + 2);
    }
}
