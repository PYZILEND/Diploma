﻿using System.Collections;
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
    public static Allegiance allegianceTurn;
    /// <summary>
    /// Shows what turn phase it is
    /// </summary>
    public static Phase turnPhase;

    //Used to reference GUI
    public CountryInfo countryInfoRef;
    public UnitInfo unitInfoRef;

    //Used to control GUI via static methods
    public static CountryInfo countryInfo;
    public static UnitInfo unitInfo;

    /// <summary>
    /// Initializing game
    /// </summary>
    void Awake()
    {
        //Hooking static fields to usual ones
        unitPrefab = unitPrefabRef;
        countryPrefab = countryPrefabRef;
        capitalPrefab = capitalPrefabRef;

        countryInfo = countryInfoRef;
        unitInfo = unitInfoRef;

        //Instantiating maps and creating cells
        physicalMap = Instantiate(physicalMapPrefab, this.transform, false);
        physicalMap.CreateMap(mapWidth, mapHeight);

        logicalMap = Instantiate(logicalMapPrefab, this.transform, false);
        logicalMap.CreateMap(mapWidth, mapHeight);
        logicalMap.HighlightAllegiance();

        countries = new List<Country>();
        units = new List<Unit>();

        //Initializing game start
        RestartGame();
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
        //Restore each country's initial state
        foreach(Country country in countries)
        {
            country.ResetCountry();
        }

        //Restore each unit's initial state (NOT POSITION)
        foreach(Unit unit in units)
        {
            unit.ResetUnit();
        }

        //Game starts from dominion disclosuring it's secret ally
        turnPhase = Phase.SecretAllies;
        allegianceTurn = Allegiance.Dominion;

        //Showing possible disclosure options
        CountryControls.ChangePhase();
    }
}
