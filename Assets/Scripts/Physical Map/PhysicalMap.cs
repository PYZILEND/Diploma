using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(MeshFilter), (typeof(MeshRenderer)))]
public class PhysicalMap : MonoBehaviour {

    Mesh mesh;
    MeshCollider meshCollider;
    List<Vector3> vertices;
    List<int> triangles;

    public Text cellLabelPrefab;
    Canvas mapCanvas;

    void Awake()
    {
        GetComponent<MeshFilter>().mesh = mesh = new Mesh();
        mesh.name = "MapMesh";
        meshCollider = gameObject.AddComponent<MeshCollider>();
        vertices = new List<Vector3>();
        triangles = new List<int>();

        mapCanvas = GetComponentInChildren<Canvas>();
    }

    public void CreateMap(int width, int height)
    {
        mesh.Clear();
        vertices.Clear();
        triangles.Clear();

        for (int x = 0; x < height; x++)
        {
            for (int z = 0; z < width; z++)
            {
                CreateCell(x, z);
            }
        }

        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.RecalculateNormals();
        meshCollider.sharedMesh = mesh;
    }

    void CreateCell(int x, int z)
    {
        Vector3 position;
        position.x = (x + z * 0.5f - z / 2) * (2f * HexMetrics.innerRadius);
        position.y = 0f;
        position.z = z * (1.5f * HexMetrics.outerRadius);

        TriangulateCell(position);

        HexCoordinates coordinates = HexCoordinates.fromOffsetCoordinates(x, z);
        Text label = Instantiate(cellLabelPrefab);
        label.rectTransform.SetParent(mapCanvas.transform, false);
        label.rectTransform.anchoredPosition = new Vector2(position.x, position.z);
        label.text = coordinates.X + "\n" + coordinates.Y + "\n" + coordinates.Z;
    }

    void TriangulateCell(Vector3 center)
    {
        for(int i = 0; i < HexMetrics.corners.Length - 1; i++)
        {
            AddTriangle(center, center + HexMetrics.corners[i], center + HexMetrics.corners[i + 1]);
        }
    }

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
