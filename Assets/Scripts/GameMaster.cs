using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameMaster : MonoBehaviour {


    public LogicalMap logicalMapPrefab;
    public PhysicalMap physicalMapPrefab;
    public int mapWidth = 6;
    public int mapHeight = 6;
    
    void Awake()
    {
        LogicalMap logicalMap = Instantiate(logicalMapPrefab, this.transform, false);
        logicalMap.CreateMap(mapWidth, mapHeight);

        PhysicalMap physicalMap = Instantiate(physicalMapPrefab, this.transform, false);
        physicalMap.CreateMap(mapWidth, mapHeight);
    }
}
