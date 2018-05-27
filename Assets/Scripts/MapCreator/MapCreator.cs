using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MapCreator : MonoBehaviour {

    public static int mapWidth = 14;
    public static int mapHeight = 14;
    float seaHeight = 2;
    public WaterGeneration water;
    public CreateHexMap hexMap;

    public static List<Country> countries;

    //For instantiating (referances)
    public LogicalMap newMapPrefab;
    public Country countryPrefabRef;
    public Capital capitalPrefabRef;

    //For instantiating (statics)
    public static LogicalMap newMap;
    public static Country countryPrefab;
    public static Capital capitalPrefab;

    public TerrainType currentType = TerrainType.Plain;

    // Use this for initialization
    void Awake () {
        // water = Instantiate(water);
        //water = GetComponentInChildren<WaterGeneration>();
        //water.CreateWater(mapWidth, mapHeight, HexMetrics.outerRadius * 2f, seaHeight);
        water.CreateWater(0, 0, HexMetrics.outerRadius * 2f, 1);
        hexMap.CreateCollider(mapHeight, mapWidth);
        newMap = Instantiate(newMapPrefab);
        newMap.CreateMap(mapWidth, mapHeight);
        foreach (LogicalMapCell cell in newMap.cells)
        {
            cell.terrain = TerrainType.Ocean;
        }
        hexMap.CreateMap(mapHeight, mapWidth);
        newMap.HighlightTerrain();
        if (countries == null)
        {
            countries = new List<Country>();
        }
        countryPrefab = countryPrefabRef;
        capitalPrefab = capitalPrefabRef;
    }

    public void GetSeaHeight(float value)
    {
        seaHeight = value;
        water.CreateWater(0, 0, HexMetrics.outerRadius * 2f, 1);
    }

    public void RedoMap()
    {
        Destroy(newMap.gameObject);
        hexMap.CreateCollider(mapHeight, mapWidth);
        CreateLogicalMap();

    }    

    void CreateLogicalMap()
    {
        newMap = Instantiate(newMapPrefab);
        newMap.CreateMap(mapWidth, mapHeight);
        foreach (LogicalMapCell cell in newMap.cells)
        {
            cell.terrain = TerrainType.Ocean;
        }
        hexMap.CreateMap(mapHeight, mapWidth);
        newMap.HighlightTerrain();
    }

    public void GetTerrainType(int value)
    {
        currentType = (TerrainType)value;
    }

    public void SaveWater(string mapName)
    {
        water.SaveMesh(mapName);
    }

    public void SaveMap(string mapName)
    {
        CreateHexMap.SaveMesh(mapName);
    }

}
