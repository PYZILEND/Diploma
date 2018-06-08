using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// This class is a parent class to other game elements
/// and handles global gameplay mechanics
/// </summary>
public class GameMaster : MonoBehaviour {
    

    //Lists
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

    public GameObject phaseInfoRef;
    public static GameObject phaseInfo;

    /// <summary>
    /// Initializing game
    /// </summary>
    void Awake()
    {
        countryInfo = countryInfoRef;
        unitInfo = unitInfoRef;
        phaseInfo = phaseInfoRef;
        multipleSelectionPanel = multipleSelectionPanelRef;
        multipleSelectionPanel.Hide();
        if (!PropertiesKeeper.mapName.Equals(""))
        {
            BuildMap();
        }
        else
        {
            LoadMap();// BuildMap(PropertiesKeeper.mapName, PropertiesKeeper.mapName);
        }
        PropertiesKeeper.logicalMap.HighlightAllegiance();

        units = new List<Unit>();

        //Initializing game start
        CountryControls.selectedUnitPrefab = PropertiesKeeper.unitPrefabs[0];
        RestartGame();
        LoadMap();
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
                foreach(Country country in PropertiesKeeper.countries)
                {
                    if (country.willSpawnGuerrilla)
                    {
                        PropertiesKeeper.popUp.transform.GetChild(1).GetComponent<Text>().text = "You must spend all guerrilla in country " + country.countryName;
                        PropertiesKeeper.popUp.SetActive(true);
                        CameraControls.fixCamera(true);
                        //Debug.Log("You must spend all guerrilla in country "+ country.countryName);
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
                    PropertiesKeeper.popUp.transform.GetChild(1).GetComponent<Text>().text = "You must disclosure an ally first";
                    PropertiesKeeper.popUp.SetActive(true);
                    CameraControls.fixCamera(true);
                    //Debug.Log("You must disclosure an ally first");
                    return;
                }
                if(CountryControls.selectedCountry.hasSecretArmy)
                {
                    PropertiesKeeper.popUp.transform.GetChild(1).GetComponent<Text>().text = "You must spend all secret army money";
                    PropertiesKeeper.popUp.SetActive(true);
                    CameraControls.fixCamera(true);
                    //Debug.Log("You must spend all secret army money");
                    return;
                }
                turnPhase = PhaseExtentions.VerifyNextPhase(turnPhase, out victoryAchieved);                     
                break;

        }
        if (turnPhase != Phase.SecretAllies)
        {
            phaseInfo.GetComponentsInChildren<Text>()[1].text = turnPhase.ToString() + " phase";

        }
        else
        {
            phaseInfo.GetComponentsInChildren<Text>()[1].text = "Secret allies phase";
        }
        
         PhaseExtentions.UnitsCanvas(turnPhase);
        
        
        //Will likely be replaced with context checks later on
        if (victoryAchieved)
        {
            PropertiesKeeper.popUp.transform.GetChild(1).GetComponent<Text>().text = "Player " + allegianceTurn.ToString() + " won";
            PropertiesKeeper.popUp.SetActive(true);
            CameraControls.fixCamera(true);
            //Debug.Log("Player " + allegianceTurn.ToString() + " won");
        }
               
      //  Debug.Log("Next phase is " + turnPhase.ToString());
        //Debug.Log("Now is " + allegianceTurn.ToString() + " turn");

        UnitControls.DropSelection();
        CountryControls.ShowSelectable();

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
        //Restore each country's initial state
        foreach(Country country in PropertiesKeeper.countries)
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

        //Showing possible disclosure options
        CountryControls.ShowSelectable();
    }

    /// <summary>
    /// Deletes current map, countryes and units
    /// </summary>
    public static void DropMap()
    {
        foreach(Country country in PropertiesKeeper.countries.ToArray())
        {
            country.DeleteCountry();
        }
        foreach(Unit unit in units.ToArray())
        {
            unit.DestroyLogically();
        }
        if (PropertiesKeeper.physicalMap)
        {
            Destroy(PropertiesKeeper.physicalMap.gameObject);
        }
        if (PropertiesKeeper.logicalMap)
        {
            Destroy(PropertiesKeeper.logicalMap.gameObject);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public static void BuildMap()
    {
        PropertiesKeeper.physicalMap = Instantiate(PropertiesKeeper.physicalMapPrefab );
        PropertiesKeeper.physicalMap.CreateMap(PropertiesKeeper.mapWidth, PropertiesKeeper.mapHeight);

        PropertiesKeeper.logicalMap = Instantiate(PropertiesKeeper.logicalMapPrefab);
        PropertiesKeeper.logicalMap.CreateMap(PropertiesKeeper.mapWidth, PropertiesKeeper.mapHeight);
    }

    public void SaveMap()
    {
        MapSaver.SaveMap();
    }

    public void LoadMap()
    {
        MapLoader.LoadMap(PropertiesKeeper.mapName, PropertiesKeeper.mapName);
    }

    public static void BuildMap(string mapName, string folderName)
    {
        PropertiesKeeper.physicalMap = Instantiate(PropertiesKeeper.physicalMapPrefab);

        Mesh mesh = new Mesh();
        MeshSaver.LoadMesh(mapName, folderName, "mesh", mesh);
        /* mesh.vertices = mesh2.vertices;
         mesh.triangles = mesh2.triangles;
         mesh.colors = mesh2.colors;
         mesh.normals = mesh2.normals;*/
        PropertiesKeeper.physicalMap.GetComponent<MeshFilter>().mesh = mesh;
        PropertiesKeeper.physicalMap.GetComponent<MeshCollider>();

        mesh = new Mesh();
        MeshSaver.LoadMesh(mapName, folderName, "meshCollider", mesh);
        /* mesh.vertices = mesh2.vertices;
         mesh.triangles = mesh2.triangles;
         mesh.colors = mesh2.colors;
         mesh.normals = mesh2.normals;*/

        PropertiesKeeper.physicalMap.GetComponent<MeshCollider>().sharedMesh = mesh;

        PropertiesKeeper.logicalMap = Instantiate(PropertiesKeeper.logicalMapPrefab);
        PropertiesKeeper.logicalMap.CreateMap(PropertiesKeeper.mapWidth, PropertiesKeeper.mapHeight);
    }
    public void GetMapName(string value)
    {
        PropertiesKeeper.mapName = value;
    }


    public void ClosePopUp()
    {
        PropertiesKeeper.popUp.SetActive(false);
        CameraControls.fixCamera(false);
    }
}
