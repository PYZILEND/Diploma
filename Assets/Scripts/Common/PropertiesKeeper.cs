using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PropertiesKeeper : MonoBehaviour {

    //Map size
    public static int mapWidth = 14;
    public static int mapHeight = 14;

    //Match settings fields
    public static byte incomeTurns = 10;//Debug value

    public static bool defaultGameMode=true;

    //1-low -25%
    //2-medium - 50%
    //3-high - 75%
    public static byte secretAlliesNum = 2;

    //prefbs for building map refrences
    public LogicalMap logicalMapPrefabRef;
    public PhysicalMap physicalMapPrefabRef;
    public GameObject borderPrefabRef;

    //prefbs for building map instantiating
    public static LogicalMap logicalMapPrefab;
    public static PhysicalMap physicalMapPrefab;
    public static GameObject borderPrefab;

    //actual logical and physical maps
    public static LogicalMap logicalMap;
    public static PhysicalMap physicalMap;

    //For instantiating (referances)
    public Unit[] unitPrefabsRef;
    public UnitType[] unitTypesRef;
    public Country[] countryPrefabsRef;
    public CountryType[] countryTypesRef;
    public Capital capitalPrefabRef;
    public  GameObject unitCanvasRef;
    public Transform[] treePrefabRef;

    public static Unit[] unitPrefabs;
    public static UnitType[] unitTypes;
    public static Country[] countryPrefabs;
    public static CountryType[] countryTypes;
    public static Capital capitalPrefab;
    public static GameObject unitCanvas;
    public static Transform[] treePrefab;

    //map name for loading
    public static string mapName = "test";

    //list of map's countries
    public static List<Country> countries;

    public GameObject capitalCanvasRef;
    public static GameObject capitalCanvas;

    //
    public Texture2D logoDominionRef;
    public Texture2D logoSentinelsRef;

    public static Texture2D logoDominion;
    public static Texture2D logoSentinels;

    public GameObject projectorObjectRef;
    public static GameObject projectorObject;

    public GameObject popUpRef;
    public static GameObject popUp;

    public static int turnNum;

    void Awake()
    {
        logoDominion = logoDominionRef;
        logoSentinels = logoSentinelsRef;
        turnNum = 0;
        countries = new List<Country>();
        logicalMapPrefab = logicalMapPrefabRef;
        physicalMapPrefab = physicalMapPrefabRef;
        unitPrefabs = unitPrefabsRef;
        unitTypes = unitTypesRef;
        countryPrefabs = countryPrefabsRef;
        countryTypes = countryTypesRef;
        capitalPrefab = capitalPrefabRef;
        borderPrefab = borderPrefabRef;
        capitalCanvas = capitalCanvasRef;
        unitCanvas = unitCanvasRef;
        projectorObject = projectorObjectRef;

        treePrefab = treePrefabRef;

        popUp = popUpRef;
    }

    
}
