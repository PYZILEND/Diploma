using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This class is a parent class to other game elements
/// and handles global gameplay mechanics
/// </summary>
public class GameMaster : MonoBehaviour {

    //Data for map initialization
    
    public LogicalMap logicalMapPrefab;
    public PhysicalMap physicalMapPrefab;
    public int mapWidth = 28;
    public int mapHeight = 28;

    //Stores initialized maps
    public LogicalMap logicalMap;
    public PhysicalMap physicalMap;
    
    /// <summary>
    /// Initializing game
    /// </summary>
    void Awake()
    {
        //Instantiating maps and creating cells
        physicalMap = Instantiate(physicalMapPrefab, this.transform, false);
        physicalMap.CreateMap(mapWidth, mapHeight);

        logicalMap = Instantiate(logicalMapPrefab, this.transform, false);
        logicalMap.CreateMap(mapWidth, mapHeight);
        logicalMap.ShowAllHighlights();
    }
}
