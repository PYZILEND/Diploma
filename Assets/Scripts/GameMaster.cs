using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// This class is a parent class to other game elements
/// and handles global gameplay mechanics
/// </summary>
public class GameMaster : MonoBehaviour {

    //Data for map initialization        
    public LogicalMap logicalMapPrefabRef;
    public PhysicalMap physicalMapPrefabRef;
    public static LogicalMap logicalMapPrefab;
    public static PhysicalMap physicalMapPrefab;
    public static int mapWidth = 28;
    public static int mapHeight = 28;

    //Stores initialized maps
    public static LogicalMap logicalMap;
    public static PhysicalMap physicalMap;

    //For instantiating (referances)
    public Unit[] unitPrefabsRef;
    public Country countryPrefabRef;
    public Capital capitalPrefabRef;
    public Canvas capitalInfoRef;

    //For instantiating (statics)
    public static Unit[] unitPrefabs;
    public static Country countryPrefab;
    public static Capital capitalPrefab;
    public static Canvas capitalInfo;

    //Match settings fields
    public static byte incomeTurns = 10;//Debug value

    //Lists
    public static List<Country> countries;  
    public static List<Unit> units;

    /// <summary>
    /// Shows who's turn it is
    /// </summary>
    public static Allegiance allegianceTurn;
    /// <summary>
    /// Shows what turn phase it is
    /// </summary>
    public static Phase turnPhase;

    //Used to reference GUI
    public CountryInfo countryInfoRef;
    public UnitInfo unitInfoRef;
    public MultipleSelection multipleSelectionPanelRef;

    //Used to control GUI via static methods
    public static CountryInfo countryInfo;
    public static UnitInfo unitInfo;
    public static MultipleSelection multipleSelectionPanel;
    public string mapName="test";

    public Text phaseInfo;

    public CameraControls mainCamera;

    /// <summary>
    /// Initializing game
    /// </summary>
    void Awake()
    {
        //Hooking static fields to usual ones
        unitPrefabs = new Unit[unitPrefabsRef.Length];       
        for(int i = 0; i< unitPrefabsRef.Length; i++)
        {
            unitPrefabs[i] = unitPrefabsRef[i];
        }
        countryPrefab = countryPrefabRef;
        capitalPrefab = capitalPrefabRef;

        countryInfo = countryInfoRef;
        unitInfo = unitInfoRef;
        multipleSelectionPanel = multipleSelectionPanelRef;
        multipleSelectionPanel.Hide();

        //Instantiating maps and creating cells
        logicalMapPrefab = logicalMapPrefabRef;
        physicalMapPrefab = physicalMapPrefabRef;
        capitalInfo = capitalInfoRef;
        BuildMap();
        logicalMap.HighlightAllegiance();

        if (countries == null)
        {
            countries = new List<Country>();
        }
        units = new List<Unit>();

        //Initializing game start
        CountryControls.selectedUnit = unitPrefabs[0];
        RestartGame();
       // MapSaver.SaveMap();
    }

    //This method should probably get a rework
    /// <summary>
    /// Reacts to player pressing end phase button
    /// Checks if current phase can be ended
    /// Verifyies next game phase and what player will play it
    /// If other player doesn't have any actions left current player wins
    /// </summary>
    public void EndPhase()
    {
        //Stores weather current player has won the match
        //Will likely be replaced with context checks later on
        bool victoryAchieved = false;
        
        switch (turnPhase)
        {
            //If current phase is guerrila, then player must spend all guerrilla forces
            //before they can end this phase
            case Phase.Guerrila:
                foreach(Country country in countries)
                {
                    if (country.willSpawnGuerrilla)
                    {
                        Debug.Log("You must spend all guerrilla in country "+ country.countryName);
                        return;
                    }
                }
                turnPhase = PhaseExtentions.VerifyNextPhase(turnPhase, out victoryAchieved);
                break;

            //Battle phase can be ended at any time
            case Phase.Battle:
                turnPhase = PhaseExtentions.VerifyNextPhase(turnPhase, out victoryAchieved);                
                break;

            //Recruitment can be ended at any time
            case Phase.Recruitment:
                turnPhase = PhaseExtentions.VerifyNextPhase(turnPhase, out victoryAchieved);
                break;

            //You must disclosure an ally before u can end this phase
            case Phase.SecretAllies:
                if (!CountryControls.selectedCountry)
                {
                    Debug.Log("You must disclosure an ally first");
                    return;
                }
                if(CountryControls.selectedCountry.hasSecretArmy)
                {
                    Debug.Log("You must spend all secret army money");
                    return;
                }
                turnPhase = PhaseExtentions.VerifyNextPhase(turnPhase, out victoryAchieved);                     
                break;
        }

        phaseInfo.text = allegianceTurn.ToString() + "\n " + turnPhase.ToString()+" phase";

        //Will likely be replaced with context checks later on
        if (victoryAchieved)
        {
            Debug.Log("Player " + allegianceTurn.ToString() + " won");
        }
               
        Debug.Log("Next phase is " + turnPhase.ToString());
        Debug.Log("Now is " + allegianceTurn.ToString() + " turn");

        CountryControls.ChangePhase();

        //Maybe find better place for this
        foreach (Unit unit in units.ToArray())
        {
            unit.ChangeTurn();
        }
    }

    /// <summary>
    /// Used to reinitialize game
    /// </summary>
    public void RestartGame()
    {
        mainCamera.CameraOver();
        //Restore each country's initial state
        foreach (Country country in countries)
        {
            country.ResetCountry();
        }

        //Restore each unit's initial state (NOT POSITION)
        foreach (Unit unit in units)
        {
            unit.ResetUnit();
        }

        //Game starts from dominion disclosuring it's secret ally
        turnPhase = Phase.SecretAllies;
        allegianceTurn = Allegiance.Dominion;
        
        phaseInfo.text = allegianceTurn.ToString() + "\n " + turnPhase.ToString() + " phase";

        //Showing possible disclosure options
        CountryControls.ChangePhase();
    }

    /// <summary>
    /// Deletes current map, countryes and units
    /// </summary>
    public static void DropMap()
    {
        countries.Clear();
        units.Clear();
        if (physicalMap)
        {
            Destroy(physicalMap.gameObject);
        }
        if (logicalMap)
        {
            Destroy(logicalMap.gameObject);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public static void BuildMap()
    {
        physicalMap = Instantiate(physicalMapPrefab);
        physicalMap.CreateMap(mapWidth, mapHeight);

        logicalMap = Instantiate(logicalMapPrefab);
        logicalMap.CreateMap(mapWidth, mapHeight);
    }
    
    public static void BuildMap(string mapName)
    {
        physicalMap = Instantiate(physicalMapPrefab);

        Mesh mesh = new Mesh();
        mesh.name = "MapMesh";
        Mesh mesh2 = (Mesh)UnityEditor.AssetDatabase.LoadAssetAtPath("Assets/Maps/"+mapName+"_mesh.asset", typeof(Mesh));
        mesh.vertices = mesh2.vertices;
        mesh.triangles = mesh2.triangles;
        mesh.colors = mesh2.colors;
        mesh.normals = mesh2.normals;
        physicalMap.GetComponent<MeshFilter>().mesh=mesh;
        physicalMap.GetComponent<MeshCollider>();

        mesh = new Mesh();
        mesh.name = "MapMeshCollider";
        mesh2 = (Mesh)UnityEditor.AssetDatabase.LoadAssetAtPath("Assets/Maps/" + mapName + "_meshCollider.asset", typeof(Mesh));
        mesh.vertices = mesh2.vertices;
        mesh.triangles = mesh2.triangles;
        mesh.colors = mesh2.colors;
        mesh.normals = mesh2.normals;

        physicalMap.GetComponent<MeshCollider>().sharedMesh= mesh;

        logicalMap = Instantiate(logicalMapPrefab);
        logicalMap.CreateMap(mapWidth, mapHeight);
    }

    public void SaveMap()
    {
        MapSaver.SaveMap();
    }

    public void LoadMap()
    {
        MapLoader.LoadMap(mapName);
        mainCamera.CameraOver();
    }
}
