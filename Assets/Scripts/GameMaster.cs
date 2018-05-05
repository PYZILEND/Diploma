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
    public static int mapWidth = 28;
    public static int mapHeight = 28;

    //Stores initialized maps
    public static LogicalMap logicalMap;
    public static PhysicalMap physicalMap;

    //For instantiating (referances)
    public Unit unitPrefabRef;
    public Country countryPrefabRef;
    public Capital capitalPrefabRef;

    //For instantiating (statics)
    public static Unit unitPrefab;
    public static Country countryPrefab;
    public static Capital capitalPrefab;

    //Match settings fields
    public static byte incomeTurns = 10;//Debug value

    //Lists
    public static List<Country> countries;  
    public static List<Unit> units;

    /// <summary>
    /// Shows who's turn it is
    /// </summary>
    public static Allegiance allegianceTurn = Allegiance.Dominion;
    
    /// <summary>
    /// Initializing game
    /// </summary>
    void Awake()
    {
        unitPrefab = unitPrefabRef;
        countryPrefab = countryPrefabRef;
        capitalPrefab = capitalPrefabRef;

        //Instantiating maps and creating cells
        physicalMap = Instantiate(physicalMapPrefab, this.transform, false);
        physicalMap.CreateMap(mapWidth, mapHeight);

        logicalMap = Instantiate(logicalMapPrefab, this.transform, false);
        logicalMap.CreateMap(mapWidth, mapHeight);
        logicalMap.HighlightAllegiance();

        countries = new List<Country>();
        units = new List<Unit>();
    }

    /// <summary>
    /// Passes turn to opposite allegiance
    /// This allegiance's unit's move points and attacks are reset
    /// Also drops unit selection
    /// </summary>
    public void ChangeTurn()
    {
        allegianceTurn = AllegianceExtentions.Opposite(allegianceTurn);
        foreach (Unit unit in units.ToArray())
        {
            if (unit.allegiance==allegianceTurn)
            {
                unit.ChangeTurn();
            }
        }
        UnitControls.DropSelection();
    }
}
