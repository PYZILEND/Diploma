using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using UnityEditor;


public class WaterGeneration:MonoBehaviour {
    
    private MeshFilter filter;
    Mesh m;

    public GameObject mainCamera;

    void LateUpdate()
    {
        transform.position = mainCamera.transform.position;
    }

    public void CreateWater(int mapWidth, int mapHeight, float size,float height)
    {
        if (filter != null)
        {
            filter = gameObject.GetComponent<MeshFilter>();
        }
        else
        {
            filter = gameObject.AddComponent<MeshFilter>();

        }
        //filter.mesh = GenerateMesh(mapWidth,mapHeight, size,height);
        //StartCoroutine(GenerateMesh2(mapWidth, mapHeight, size, height));
        filter.mesh=GenerateMesh(mapWidth, mapHeight, size, height);

       // Split(filter);

        // AssetDatabase.CreateAsset(filter.mesh, "Assets/water.asset");
    }

    private Mesh GenerateMesh(int mapWidth, int mapHeight, float size, float height)
    {
        //WaitForSeconds wait = new WaitForSeconds(0.05f);
        m = new Mesh();
        m.name = "waterMesh";
        int gridWidth = mapWidth + 28;
        int gridHeight = mapHeight + 28;
        List<Vector3> verticies = new List<Vector3>();
       // List<Vector3> normals = new List<Vector3>();
        List<Vector2> uvs = new List<Vector2>();
        //List<Color> colors = new List<Color>();
        for (int i = 0; i < gridHeight + 1; i++)
        {
            for (int j = 0; j < gridWidth + 1; j++)
            {
                verticies.Add(new Vector3(
                    j*size - 14 * size,
                    //0,
                    Random.Range(-height, height),
                    i*size - 14 * size));
                uvs.Add(new Vector2(j / ((float)gridWidth + 1), i / ((float)gridHeight + 1)));
                //colors.Add(new Color(j / ((float)gridWidth + 1),0, i / ((float)gridHeight + 1)));
                //normals.Add(Vector3.up);
            }
        }
        List<int> triangles = new List<int>();
        for (int i = 0; i < verticies.Count-gridWidth-1; i++)
        {
                if ((i+1) % (gridWidth+1) == 0)
                {
                    continue;
                }
                triangles.AddRange(new List<int>()
                {
                    i+1,i,i+gridWidth+1,
                    i+1,i+gridWidth+1,i+gridWidth+2
                });
            
        }
        m.SetVertices(verticies);
        m.SetTriangles(triangles, 0);
        //m.RecalculateBounds();
        //m.SetNormals(normals);
        m.SetUVs(0, uvs);
        m.RecalculateNormals();
        //m.colors = colors.ToArray();
        //yield return null;
        return m;
    }

    private MeshFilter Split(MeshFilter mf)
    {
        Mesh mesh = mf.sharedMesh;//Change to sharedmesh? 
        Vector3[] oldVerts = mesh.vertices;
        Vector2[] oldUvs = mesh.uv;

        int[] triangles = mesh.triangles;
        Vector3[] vertices = new Vector3[triangles.Length];
        Vector2[] uvs = new Vector2[triangles.Length];
        for (int i = 0; i < triangles.Length; i++)
        {
            vertices[i] = oldVerts[triangles[i]];
            uvs[i] = oldUvs[triangles[i]];
            triangles[i] = i;
            mesh.vertices = vertices;
        }
        mesh.triangles = triangles;
       // mesh.RecalculateBounds();
        mesh.RecalculateNormals();
        mesh.uv = uvs;
        return mf;
    }

    public void SaveMesh(string mapName)
    { 
        UnityEditor.AssetDatabase.CreateAsset(filter.mesh,"Assets/Maps/"+mapName+"_water.asset");
    }
}
