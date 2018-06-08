using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MapCreator : MonoBehaviour {
    
    float seaHeight = 2;
    public WaterGeneration water;
    public CreateHexMap hexMap;

 /*   public static List<Country> countries;

    //For instantiating (referances)
    public LogicalMap newMapPrefab;
    public Country countryPrefabRef;
    public Capital capitalPrefabRef;

    //For instantiating (statics)
    public static LogicalMap newMap;
    public static Country countryPrefab;
    public static Capital capitalPrefab;*/

    public TerrainType currentType = TerrainType.Plain;

    // Use this for initialization
    void Awake () {
        // water = Instantiate(water);
        //water = GetComponentInChildren<WaterGeneration>();
        //water.CreateWater(mapWidth, mapHeight, HexMetrics.outerRadius * 2f, seaHeight);
        water.CreateWater(0, 0, HexMetrics.outerRadius * 2f, 1);
        hexMap.CreateCollider(PropertiesKeeper.mapHeight, PropertiesKeeper.mapWidth);
        PropertiesKeeper.logicalMap = Instantiate(PropertiesKeeper.logicalMapPrefab);
        PropertiesKeeper.logicalMap.CreateMap(PropertiesKeeper.mapWidth, PropertiesKeeper.mapHeight);
        foreach (LogicalMapCell cell in PropertiesKeeper.logicalMap.cells)
        {
            cell.terrain = TerrainType.Ocean;
        }
        hexMap.CreateMap(PropertiesKeeper.mapHeight, PropertiesKeeper.mapWidth);
        PropertiesKeeper.logicalMap.HighlightTerrain();
        if (PropertiesKeeper.countries == null)
        {
            PropertiesKeeper.countries = new List<Country>();
        }
    }

    public void GetSeaHeight(float value)
    {
        seaHeight = value;
        water.CreateWater(0, 0, HexMetrics.outerRadius * 2f, 1);
    }

    public void RedoMap()
    {
        Destroy(PropertiesKeeper.logicalMap.gameObject);
        hexMap.CreateCollider(PropertiesKeeper.mapHeight, PropertiesKeeper.mapWidth);
        CreateLogicalMap();

    }    

    void CreateLogicalMap()
    {
        PropertiesKeeper.logicalMap = Instantiate(PropertiesKeeper.logicalMapPrefab);
        PropertiesKeeper.logicalMap.CreateMap(PropertiesKeeper.mapWidth, PropertiesKeeper.mapHeight);
        foreach (LogicalMapCell cell in PropertiesKeeper.logicalMap.cells)
        {
            cell.terrain = TerrainType.Ocean;
        }
        hexMap.CreateMap(PropertiesKeeper.mapHeight, PropertiesKeeper.mapWidth);
        PropertiesKeeper.logicalMap.HighlightTerrain();
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
