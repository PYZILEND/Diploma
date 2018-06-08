using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

public class CreateHexMap : MonoBehaviour
{
    static Mesh mesh;
    static Mesh meshColl;
    MeshCollider meshCollider;

    List<Vector3> vertices;
    List<int> triangles;
    List<Color> colors;
    //
    List<Vector3> verticesColl;
    List<int> trianglesColl;

    static Transform forestContainer;

    static Mesh[] border;
    public GameObject borderPrefab;
    List<GameObject> borders;
    List<Vector3>[] verticesBorders;
    List<int>[] trianglesBorders;
    List<Vector2>[] uvsBorders;

    public Texture2D noiseSource;
    
    public void CreateMap(int height, int width)
    {
        GetComponent<MeshFilter>().mesh = mesh = new Mesh();
        mesh.name = "MapMesh";
        vertices = new List<Vector3>();
        triangles = new List<int>();
        colors = new List<Color>();
        borders = new List<GameObject>(); 
        ClearForest();
    }

    public void RedoMapMesh(int height, int width)
    {
        vertices = new List<Vector3>();
        triangles = new List<int>();
        colors = new List<Color>();
        ClearForest();

        CreateMapMesh(height, width);
        MapMeshCollider(height, width);

    }

    public void ClearForest()
    {
        if (forestContainer)
        {
            Destroy(forestContainer.gameObject);
        }
        forestContainer = new GameObject("Forest Container").transform;
        forestContainer.SetParent(transform, false);
    }

    void CreateMapMesh(int height, int width)
    {
        mesh.Clear();
        vertices.Clear();
        triangles.Clear();
        colors.Clear();

        for (int i = 0; i < height; i++)
        {
            for (int j = 0; j < width; j++)
            {
                if (PropertiesKeeper.logicalMap.cells[j + i * width].terrain == TerrainType.Ocean)
                {
                    continue;
                }
                Vector3 offset = OffsetVector(j, i);
                CreateHex(offset, (j + i * width));
            }
        }
        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.colors = colors.ToArray();
        mesh.RecalculateNormals();
    }


    public static Vector3 OffsetVector(int offsetX, int offsetZ)
    {
        Vector3 offset = new Vector3();
        offset.x = offsetX * HexMetrics.innerRadius * 2 + (HexMetrics.innerRadius * (offsetZ % 2));
        offset.y = 1.5f;
        offset.z = offsetZ * HexMetrics.outerRadius * 1.5f;
        return offset;
    }

    void CreateHex(Vector3 offset, int ind)
    {
        
        switch ( PropertiesKeeper.logicalMap.cells[ind].terrain)
        {
            case TerrainType.Forest:
                if ( PropertiesKeeper.logicalMap.cells[ind].country == null || ( PropertiesKeeper.logicalMap.cells[ind].country != null&&  PropertiesKeeper.logicalMap.cells[ind].country.capital !=  PropertiesKeeper.logicalMap.cells[ind])) {
                    AddForest(offset, ind);
                }
                break;
            case TerrainType.Road:
                AddRoad(offset,ind);
                return;
            case TerrainType.Impassable:
                AddMountain(offset,ind,TerrainTypeExtentions.TypeToTileColor(TerrainType.Impassable));
                return;
            default: break;
        }
        TriangulateHex(offset, ind);
    }

    void TriangulateHex(Vector3 offset, int ind)
    {
        Color color = TerrainTypeExtentions.TypeToTileColor( PropertiesKeeper.logicalMap.cells[ind].terrain);

        for (HexDirection i = HexDirection.NE; i <= HexDirection.NW; i++)
        {
            AddTriangle(HexMetrics.GetFirstSolidCorner(i) + offset, HexMetrics.GetSecondSolidCorner(i) + offset, offset, color);
            TriangulateConnection(i, HexMetrics.GetFirstSolidCorner(i) + offset, HexMetrics.GetSecondSolidCorner(i) + offset, offset, color);

            AddShore(offset, ind, i, color);
        }
    }


    void TriangulateConnection(HexDirection direction, Vector3 v1, Vector3 v2, Vector3 offset, Color color)
    {
        Vector3 bridge = HexMetrics.GetBridge(direction);
        Vector3 v3 = v1 + bridge;
        Vector3 v4 = v2 + bridge;

        AddQuad(v1, v2, v3, v4, color);

        AddTriangle(v1, offset + HexMetrics.corners[(int)direction], v3,color);
        AddTriangle(v2, v4, offset + HexMetrics.corners[(int)direction+1], color);
    }

    public float SampleNoise(Vector3 position)
    {
        return noiseSource.GetPixelBilinear(position.x, position.z).r;
    }

    Vector3 Perturb(Vector3 position)
    {
        position.y += SampleNoise(position) * 2f - 0.9f;
        return position;
    }

    void AddTriangle(Vector3 v1, Vector3 v2, Vector3 v3, Color color)
    {
        vertices.Add(Perturb(v1));
        vertices.Add(Perturb(v2));
        vertices.Add(Perturb(v3));
        triangles.Add(vertices.Count - 3);
        triangles.Add(vertices.Count - 2);
        triangles.Add(vertices.Count - 1);
        colors.Add(color);
        colors.Add(color);
        colors.Add(color);
    }

    void AddQuad(Vector3 v1, Vector3 v2, Vector3 v3, Vector3 v4, Color color)
    {
        vertices.Add(Perturb(v1));
        vertices.Add(Perturb(v2));
        vertices.Add(Perturb(v3));
        vertices.Add(Perturb(v4));
        triangles.Add(vertices.Count - 4);
        triangles.Add(vertices.Count - 2);
        triangles.Add(vertices.Count - 3);
        triangles.Add(vertices.Count - 3);
        triangles.Add(vertices.Count - 2);
        triangles.Add(vertices.Count - 1);
        colors.Add(color);
        colors.Add(color);
        colors.Add(color);
        colors.Add(color);
    }

    void AddMountain(Vector3 offset, int ind,Color color)
    {
        for (HexDirection i = HexDirection.NE; i <= HexDirection.NW; i++)
        {
            Vector3 corner1 = offset + HexMetrics.GetFirstSolidCorner(i);
            Vector3 corner2 = offset + HexMetrics.GetSecondSolidCorner(i);

            Vector3 v1 = HexMetrics.GetFirstSolidCorner(i) * 0.5f +offset;
            v1.y = 6f;
            Vector3 v2 = HexMetrics.GetSecondSolidCorner(i) * 0.5f+offset;
            v2.y = 6f;

            Vector3 bridge = HexMetrics.GetBridge(i);
            AddTriangle(corner1, offset + HexMetrics.corners[(int)i], corner1 + bridge, color);
            AddTriangle(corner2, corner2 + bridge, offset + HexMetrics.corners[(int)i + 1], color);

            AddQuad(corner1, corner2, corner1+bridge, corner2+bridge, color);

            AddQuad(v1,v2,corner1,corner2,color);
            AddTriangle(v1, v2, new Vector3(offset.x, 14f, offset.z), new Color32(229,227,238,255));

            AddShore(offset, ind, i, color);
        }

    }

    void AddShore(Vector3 offset,int ind, HexDirection i, Color color)
    {
        if ( PropertiesKeeper.logicalMap.cells[ind].neighbors[(int)i] == null ||
                     ( PropertiesKeeper.logicalMap.cells[ind].neighbors[(int)i] != null &&  PropertiesKeeper.logicalMap.cells[ind].neighbors[(int)i].terrain == TerrainType.Ocean))
        {
            AddQuad(HexMetrics.GetFirstSolidCorner(i) + offset + HexMetrics.GetBridge(i),
                HexMetrics.GetSecondSolidCorner(i) + HexMetrics.GetBridge(i) + offset,
               HexMetrics.GetFirstSolidCorner(i) + HexMetrics.GetBridge(i) + offset + new Vector3(0, -5f, 0),
                HexMetrics.GetSecondSolidCorner(i) + HexMetrics.GetBridge(i) + offset + new Vector3(0, -5f, 0), color);

            AddQuad(HexMetrics.corners[(int)i] + offset,
                HexMetrics.GetFirstSolidCorner(i) + offset + HexMetrics.GetBridge(i),
                HexMetrics.corners[(int)i] + offset + new Vector3(0, -5f, 0),
                HexMetrics.GetFirstSolidCorner(i) + offset + HexMetrics.GetBridge(i) + new Vector3(0, -5f, 0),
                color);
            AddQuad(HexMetrics.GetSecondSolidCorner(i) + offset + HexMetrics.GetBridge(i),
                HexMetrics.corners[(int)i + 1] + offset,
                HexMetrics.GetSecondSolidCorner(i) + offset + HexMetrics.GetBridge(i) + new Vector3(0, -5f, 0),
                HexMetrics.corners[(int)i + 1] + offset + new Vector3(0, -5f, 0),
                color);
        }
    }

    void AddRoad(Vector3 offset, int ind)
    {
        for (HexDirection i = HexDirection.NE; i <= HexDirection.NW; i++)
        {
            AddRoadTriangle(offset, i, ind);

            AddShore(offset, ind, i, TerrainTypeExtentions.TypeToTileColor(TerrainType.Plain));
        }
    }

    void AddRoadTriangle(Vector3 offset,HexDirection dir, int ind)
    {
        //creating all needed vertices' vectors first
        //look at image "roadVertices.png" to see where each vertice is
        Vector3 v1 = offset + HexMetrics.GetFirstSolidCorner(dir) * 0.5f;
        v1.y = 1.5f;
        Vector3 v2 = offset + HexMetrics.GetSecondSolidCorner(dir) * 0.5f;
        v2.y = 1.5f;
        Vector3 v3 = offset + HexMetrics.GetRoadVertice(dir);
        Vector3 corner1 = offset + HexMetrics.GetFirstSolidCorner(dir);
        Vector3 corner2 = offset + HexMetrics.GetSecondSolidCorner(dir);
        Vector3 bridge = HexMetrics.GetBridge(dir);

        Color color=TerrainTypeExtentions.TypeToTileColor(TerrainType.Plain);

        //corners are always plain color
        AddTriangle(corner1, offset + HexMetrics.corners[(int)dir], corner1+bridge, color);
        AddTriangle(corner2, corner2+bridge, offset + HexMetrics.corners[(int)dir + 1], color);
        //always road color
        AddQuad(v1,offset,v3,v2, TerrainTypeExtentions.TypeToTileColor(TerrainType.Road));

        //might not be colored road
        //need to check if neighbor is also road or if opposite one is road
        if ( PropertiesKeeper.logicalMap.cells[ind].neighbors[(int)dir] != null &&  PropertiesKeeper.logicalMap.cells[ind].neighbors[(int)dir].terrain == TerrainType.Road)
        {
            AddTriangle(corner1, corner2, v3, TerrainTypeExtentions.TypeToTileColor(TerrainType.Road));
            AddTriangle(corner1, v3, v1, TerrainTypeExtentions.TypeToTileColor(TerrainType.Road));
            AddTriangle(v3, corner2, v2, TerrainTypeExtentions.TypeToTileColor(TerrainType.Road));
            AddQuad(corner1, corner2, corner1 + bridge, corner2 + bridge, TerrainTypeExtentions.TypeToTileColor(TerrainType.Road));
        }
        else
        {
            AddQuad(corner1, corner2, corner1 + bridge, corner2 + bridge, color);
            
            /* if ( PropertiesKeeper.logicalMap.cells[ind].neighbors[(int)HexDirectionExtensions.Opposite(dir)] != null &&  PropertiesKeeper.logicalMap.cells[ind].neighbors[(int)HexDirectionExtensions.Opposite(dir)].terrain == TerrainType.Road)
             {
                 AddTriangle(corner1, corner2, v3, TerrainTypeExtentions.TypeToTileColor(TerrainType.Road));
                 AddTriangle(corner1, v3, v1, TerrainTypeExtentions.TypeToTileColor(TerrainType.Road));
                 AddTriangle(v3, corner2, v2, TerrainTypeExtentions.TypeToTileColor(TerrainType.Road));
             }
             else
             {
                 AddTriangle(corner1, corner2, v3, TerrainTypeExtentions.TypeToTileColor(TerrainType.Plain));

                 if (( PropertiesKeeper.logicalMap.cells[ind].neighbors[(int)HexDirectionExtensions.NeighborLeft(dir)] != null &&  PropertiesKeeper.logicalMap.cells[ind].neighbors[(int)HexDirectionExtensions.NeighborLeft(dir)].terrain == TerrainType.Road)||
                     ( PropertiesKeeper.logicalMap.cells[ind].neighbors[(int)HexDirectionExtensions.OppositeNeighborLeft(dir)] != null &&  PropertiesKeeper.logicalMap.cells[ind].neighbors[(int)HexDirectionExtensions.OppositeNeighborLeft(dir)].terrain == TerrainType.Road))
                 {
                     AddTriangle(corner1, v3, v1, TerrainTypeExtentions.TypeToTileColor(TerrainType.Road));
                 }
                 else
                 {
                     AddTriangle(corner1, v3, v1, TerrainTypeExtentions.TypeToTileColor(TerrainType.Plain));
                 }
                 if (( PropertiesKeeper.logicalMap.cells[ind].neighbors[(int)HexDirectionExtensions.NeighborRight(dir)] != null &&  PropertiesKeeper.logicalMap.cells[ind].neighbors[(int)HexDirectionExtensions.NeighborRight(dir)].terrain == TerrainType.Road)||
                    ( PropertiesKeeper.logicalMap.cells[ind].neighbors[(int)HexDirectionExtensions.OppositeNeighborRight(dir)] != null &&  PropertiesKeeper.logicalMap.cells[ind].neighbors[(int)HexDirectionExtensions.OppositeNeighborRight(dir)].terrain == TerrainType.Road))
                 {
                     AddTriangle(v3, corner2, v2, TerrainTypeExtentions.TypeToTileColor(TerrainType.Road));
                 }
                 else
                 {
                     AddTriangle(v3, corner2, v2, TerrainTypeExtentions.TypeToTileColor(TerrainType.Plain));
                 }
             }*/
                AddTriangle(corner1, corner2, v3, color);

                if (( PropertiesKeeper.logicalMap.cells[ind].neighbors[(int)HexDirectionExtensions.NeighborLeft(dir)] != null &&  PropertiesKeeper.logicalMap.cells[ind].neighbors[(int)HexDirectionExtensions.NeighborLeft(dir)].terrain == TerrainType.Road))
                {
                    AddTriangle(corner1, v3, v1, TerrainTypeExtentions.TypeToTileColor(TerrainType.Road));
                }
                else
                {
                    AddTriangle(corner1, v3, v1, color);
                }
                if (( PropertiesKeeper.logicalMap.cells[ind].neighbors[(int)HexDirectionExtensions.NeighborRight(dir)] != null &&  PropertiesKeeper.logicalMap.cells[ind].neighbors[(int)HexDirectionExtensions.NeighborRight(dir)].terrain == TerrainType.Road) )
                {
                    AddTriangle(v3, corner2, v2, TerrainTypeExtentions.TypeToTileColor(TerrainType.Road));
                }
                else
                {
                    AddTriangle(v3, corner2, v2, color);
                }
        }
    }

    void AddForest(Vector3 offset, int ind)
    {
        int[] forestation = new int[] { -1, -1, -1, -1, -1, -1} ;
        int j = -1;
        for (int i = 0; i < 6; i++)
        {
            if ( PropertiesKeeper.logicalMap.cells[ind].neighbors[i] != null &&  PropertiesKeeper.logicalMap.cells[ind].neighbors[i].terrain != TerrainType.Forest
                &&  PropertiesKeeper.logicalMap.cells[ind].neighbors[i].terrain != TerrainType.Impassable
                &&  PropertiesKeeper.logicalMap.cells[ind].neighbors[i].terrain != TerrainType.Ocean)
            {
                j = Random.Range(1, 4);
                break;
            }
        }
        if (j == -1)
        {
            j = Random.Range(4, 7);
        }
        do
        {
            int k = Random.Range(0, 6);
            if (forestation[k] == -1)
            {
                forestation[k] = 1;
                j--;
            }
        } while (j != 0);
        for (int i = 0; i < forestation.Length; i++)
        {
            if (forestation[i] == -1) continue;
            AddTree(((HexMetrics.corners[i] + HexMetrics.corners[i + 1] + offset*3) / 3),(float)ind/ PropertiesKeeper.logicalMap.cells.Length,(float)i/forestation.Length);
        }
    }

    void AddTree(Vector3 offset,float x,float y)
    {
        Transform instance = Instantiate(PropertiesKeeper.treePrefab[Random.Range(0, PropertiesKeeper.treePrefab.Length)]);
        instance.localPosition = offset;
        instance.SetParent(forestContainer, false);
        instance.localRotation = Quaternion.Euler(0f, 360f * Mathf.PerlinNoise(x, y), 0f);
    }

    public void CreateCollider(int height, int width)
    {
        meshColl = new Mesh();
        if (meshCollider != null)
        {
            meshCollider = gameObject.GetComponent<MeshCollider>();
        }
        else
        {
            meshCollider = gameObject.AddComponent<MeshCollider>();

        }
        meshColl.name = "MapMeshCollider";
        verticesColl = new List<Vector3>();
        trianglesColl = new List<int>();
        CreateMapMeshCollider(height, width);
        meshCollider.sharedMesh = meshColl;
    }

    void CreateMapMeshCollider(int height, int width)
    {
        meshColl.Clear();
        verticesColl.Clear();
        trianglesColl.Clear();
        for (int i = 0; i < height; i++)
        {
            for (int j = 0; j < width; j++)
            {
                CreateHex(j, i);
            }
        }
        meshColl.vertices = verticesColl.ToArray();
        meshColl.triangles = trianglesColl.ToArray();
        meshColl.RecalculateNormals();
        meshCollider.sharedMesh = meshColl;
    }


    void MapMeshCollider(int height, int width)
    {
        meshColl.Clear();
        verticesColl.Clear();
        trianglesColl.Clear();
        for (int i = 0; i < height; i++)
        {
            for (int j = 0; j < width; j++)
            {

                switch ( PropertiesKeeper.logicalMap.cells[j + i * width].terrain)
                {
                    case TerrainType.Ocean: CreateHex(j, i); break;
                    case TerrainType.Impassable: CreateMountainHex(j, i, j + i * width); break;
                    default: CreateHex(j, i, j + i * width); break;
                }
            }
        }
        meshColl.vertices = verticesColl.ToArray();
        meshColl.triangles = trianglesColl.ToArray();
        meshColl.RecalculateNormals();
        meshCollider.sharedMesh = meshColl;
    }

    void CreateHex(int offsetX, int offsetZ)
    {
        Vector3 offset = new Vector3();
        offset.x = offsetX * HexMetrics.innerRadius * 2 + (HexMetrics.innerRadius * (offsetZ % 2));
        offset.y = 0f;
        offset.z = offsetZ * HexMetrics.outerRadius * 1.5f;

        for (int i = 0; i < 6; i++)
        {
            AddTriangleCollider(HexMetrics.corners[i] + offset, HexMetrics.corners[i + 1] + offset, offset);
        }
    }

    void CreateHex(int offsetX, int offsetZ, int ind)
    {
        Vector3 offset = new Vector3();
        offset.x = offsetX * HexMetrics.innerRadius * 2 + (HexMetrics.innerRadius * (offsetZ % 2));
        offset.y = 1.5f;
        offset.z = offsetZ * HexMetrics.outerRadius * 1.5f;

        for (int i = 0; i < 6; i++)
        {
            AddTriangleCollider(HexMetrics.corners[i] + offset, HexMetrics.corners[i + 1] + offset, offset);
            if ( PropertiesKeeper.logicalMap.cells[ind].neighbors[i] == null ||
                ( PropertiesKeeper.logicalMap.cells[ind].neighbors[i] != null &&  PropertiesKeeper.logicalMap.cells[ind].neighbors[i].terrain == TerrainType.Ocean))
            {
                AddQuadCollider(HexMetrics.corners[i] + offset, HexMetrics.corners[i + 1] + offset, HexMetrics.corners[i] + offset + new Vector3(0, -1.5f, 0), HexMetrics.corners[i + 1] + offset + new Vector3(0, -1.5f, 0));
            }
        }
    }

    void CreateMountainHex(int offsetX, int offsetZ, int ind)
    {
        Vector3 offset = new Vector3();
        offset.x = offsetX * HexMetrics.innerRadius * 2 + (HexMetrics.innerRadius * (offsetZ % 2));
        offset.y = 1.5f;
        offset.z = offsetZ * HexMetrics.outerRadius * 1.5f;

        for (int i = 0; i < 6; i++)
        {
            AddTriangleCollider(HexMetrics.corners[i] + offset, HexMetrics.corners[i + 1] + offset, offset + new Vector3(0, 12.5f, 0));
            if ( PropertiesKeeper.logicalMap.cells[ind].neighbors[i] == null ||
                ( PropertiesKeeper.logicalMap.cells[ind].neighbors[i] != null &&  PropertiesKeeper.logicalMap.cells[ind].neighbors[i].terrain == TerrainType.Ocean))
            {
                AddQuadCollider(HexMetrics.corners[i] + offset, HexMetrics.corners[i + 1] + offset, HexMetrics.corners[i] + offset + new Vector3(0, -1.5f, 0), HexMetrics.corners[i + 1] + offset + new Vector3(0, -1.5f, 0));
            }
        }
    }

    void TriangulateHex(Vector3 offset)
    {
        for (int i = 0; i < 6; i++)
        {
            AddTriangleCollider(HexMetrics.corners[i] + offset, HexMetrics.corners[i+1] + offset,offset);
        }
    }

    void AddTriangleCollider(Vector3 v1, Vector3 v2, Vector3 v3)
    {
        verticesColl.Add(v1);
        verticesColl.Add(v2);
        verticesColl.Add(v3);
        trianglesColl.Add(verticesColl.Count - 3);
        trianglesColl.Add(verticesColl.Count - 2);
        trianglesColl.Add(verticesColl.Count - 1);
    }

    void AddQuadCollider(Vector3 v1, Vector3 v2, Vector3 v3, Vector3 v4)
    {
        verticesColl.Add(v1);
        verticesColl.Add(v2);
        verticesColl.Add(v3);
        verticesColl.Add(v4);
        trianglesColl.Add(verticesColl.Count - 4);
        trianglesColl.Add(verticesColl.Count - 2);
        trianglesColl.Add(verticesColl.Count - 3);

        trianglesColl.Add(verticesColl.Count - 3);
        trianglesColl.Add(verticesColl.Count - 2);
        trianglesColl.Add(verticesColl.Count - 1);

    }    

    Vector3 TriangleOffset(float offsetX, float offsetZ, Vector3 coord)
    {
        return new Vector3(coord.x + offsetX, coord.y, coord.z + offsetZ);
    }

    public static void SaveMesh(string mapName)
    {
        MeshSaver.SaveMesh(mapName, mapName, "mesh", mesh);
        MeshSaver.SaveMesh(mapName, mapName, "meshCollider", meshColl);

        //UnityEditor.AssetDatabase.CreateAsset(mesh, "Assets/Maps/" + mapName + "_mesh.asset");
        //UnityEditor.AssetDatabase.CreateAsset(meshColl, "Assets/Maps/" + mapName + "_meshCollider.asset");
        SaveBorder(mapName);
       /* for(int i = 0; i < forestContainer.childCount; i++)
        {
            UnityEditor.AssetDatabase.CreateAsset(forestContainer.GetChild(i).GetComponent<MeshFilter>().mesh, "Assets/Maps/" + mapName + "_forest"+i+".asset");
        }*/
    }

    static void SaveBorder(string mapName)
    {
        for (int i = 0; i < border.Length; i++)
        {
            MeshSaver.SaveMesh(mapName, mapName, "border" + i, border[i]);
            //UnityEditor.AssetDatabase.CreateAsset(border[i], "Assets/Maps/" + mapName + "_border"+i+".asset");
        }
    }

    public void CreateBorders(int height, int width)
    {
        ClearBorders();

        if (PropertiesKeeper.countries == null && PropertiesKeeper.countries.Count == 0)
        {
            return;
        }

        border = new Mesh[PropertiesKeeper.countries.Count];
        verticesBorders = new List<Vector3>[border.Length];
        trianglesBorders = new List<int>[border.Length];
        uvsBorders = new List<Vector2>[border.Length];

        for (int i = 0; i < border.Length; i++)
        {
            borders.Add(Instantiate(borderPrefab, transform, false));
            border[i] = new Mesh();
            verticesBorders[i] = new List<Vector3>();
            trianglesBorders[i] = new List<int>();
            uvsBorders[i] = new List<Vector2>();
        }

        foreach (Country country in PropertiesKeeper.countries)
        {
            foreach (LogicalMapCell cell in country.area)
            {
                if (cell.terrain == TerrainType.Ocean||
                    cell.terrain == TerrainType.Impassable|| 
                    cell.country == null)
                {
                    continue;
                }
                Vector3 offset = OffsetVector((int)cell.coordinates.ToOffset().x, (int)cell.coordinates.ToOffset().y);
                AddBorder(cell, offset);
            }

        }        

        for (int i = 0; i < border.Length; i++)
        {
            border[i].vertices = verticesBorders[i].ToArray();
            border[i].triangles = trianglesBorders[i].ToArray();
            border[i].uv = uvsBorders[i].ToArray();
            border[i].RecalculateNormals();
            borders[i].GetComponent<MeshFilter>().mesh = border[i];
        }
    }


    void ClearBorders()
    {
        while (borders.Count > 0)
        {
            Destroy(borders[0]);
            borders.RemoveAt(0);
        }
    }

    void AddBorder(LogicalMapCell cell, Vector3 offset)
    {
        for (HexDirection i = HexDirection.NE; i <= HexDirection.NW; i++)
        {
            if (cell.neighbors[(int)i]!=null && 
                ((cell.neighbors[(int)i].terrain!=TerrainType.Ocean || cell.neighbors[(int)i].terrain != TerrainType.Impassable)
                &&(cell.neighbors[(int)i].country != cell.country)))
            {
                AddQuad(HexMetrics.GetBorderSolidCorner(i) + offset,
                    HexMetrics.GetBorderCorner(i) + offset,
                    HexMetrics.GetBorderSolidCorner(i) + offset + HexMetrics.GetBorderSolidVector(i),
                    HexMetrics.GetBorderCorner(i) + offset + HexMetrics.GetBorderVector(i),GetCountryIndex(cell.country),0);
                AddQuad(HexMetrics.GetBorderSolidCorner(i + 1) + offset - HexMetrics.GetBorderSolidVector(i),
                    HexMetrics.GetBorderCorner(i + 1) + offset - HexMetrics.GetBorderVector(i), 
                    HexMetrics.GetBorderSolidCorner(i+1) + offset,
                    HexMetrics.GetBorderCorner(i+1) + offset,
                    GetCountryIndex(cell.country),1);
            }
        }
    }

    void AddQuad(Vector3 v1, Vector3 v2, Vector3 v3, Vector3 v4,int ind, int uv)
    {
        verticesBorders[ind].Add(v1);
        verticesBorders[ind].Add(v2);
        verticesBorders[ind].Add(v3);
        verticesBorders[ind].Add(v4);
        trianglesBorders[ind].Add(verticesBorders[ind].Count - 4);
        trianglesBorders[ind].Add(verticesBorders[ind].Count - 2);
        trianglesBorders[ind].Add(verticesBorders[ind].Count - 3);
        trianglesBorders[ind].Add(verticesBorders[ind].Count - 3);
        trianglesBorders[ind].Add(verticesBorders[ind].Count - 2);
        trianglesBorders[ind].Add(verticesBorders[ind].Count - 1);
        uvsBorders[ind].Add(new Vector2(uv, uv));
        uvsBorders[ind].Add(new Vector2(uv, uv));
        uvsBorders[ind].Add(new Vector2(uv, uv));
        uvsBorders[ind].Add(new Vector2(uv, uv));
    }

    int GetCountryIndex(Country country)
    {
        for (int i = 0; i < PropertiesKeeper.countries.Count; i++)
        {
            if (country == PropertiesKeeper.countries[i])
            {
                return i;
            }
        }
        return -1;
    }
}
