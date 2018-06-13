using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Country : MonoBehaviour {

    /// <summary>
    /// Name of the country
    /// </summary>
    [SerializeField]
    public string countryName
    {
        get; private set;
    }

    /// <summary>
    /// Country area
    /// Cells that belong to the country
    /// </summary>
    /// 
    [SerializeField]
    public List<LogicalMapCell> area { get; private set; }

    /// <summary>
    /// Capital of the country 
    /// </summary>
    public LogicalMapCell capital
    {
        get; private set;
    }

    /// <summary>
    /// Capital city object
    /// </summary>
    public Capital capitalCity
    {
        get; private set;
    }

    /// <summary>
    /// How many turns country will give income
    /// </summary>
    public byte incomeTurnsLeft
    {
        get; private set;
    }

    /// <summary>
    /// Treasury
    /// </summary>
    public byte treasury
    {
        get; private set;
    }    

    /// <summary>
    /// If not 0 then secret army
    /// was not disclosed yet
    /// </summary>
    public byte secretArmy
    {
        get; private set;
    }

    /// <summary>
    /// Returns if country is a secret ally
    /// </summary>
    /// <returns>returns false if secretArmy is 0</returns>
    public bool hasSecretArmy
    {
        get
        {
            if (secretArmy > 0)
            {
                return true;
            }
            return false;
        }
    }

    /// <summary>
    /// If not 0 then has guerilla army
    /// </summary>
    public byte guerilla
    {
        get; private set;
    }

    /// <summary>
    /// Returns status of guerilla
    /// </summary>
    /// <returns>returns false if no guerilla money left</returns>
    public bool hasGuerilla
    {
        get
        {
            if (guerilla > 0)
            {
                return true;
            }
            return false;
        }
    }

    /// <summary>
    /// If true then has reparation money
    /// </summary>
    public bool hasReparation
    {
        get; private set;
    }

    /// <summary>
    /// County's allegiance
    /// Sentinels, Dominion, Neutral
    /// </summary>
    public Allegiance allegiance
    {
        get; private set;
    }

    /// <summary>
    /// Country's initial allegiance
    /// affects reparations
    /// </summary>
    public Allegiance secretAllegiance
    {
        get; private set;
    }

    /// <summary>
    /// Country type - poor, average, rich
    /// </summary>
    public CountryType type;

    /// <summary>
    /// Checks if country is invaded by enemy units
    /// If guerrilla will spawn it also counts as invasion
    /// </summary>
    public bool isInvaded
    {
        get
        {
            foreach (LogicalMapCell cell in area)
            {
                if ((cell.unit != null) && (cell.unit.allegiance != allegiance && !cell.unit.isDestroyed))
                {
                    return true;
                }
            }
            return false;
        }
    }

    /// <summary>
    /// Checks if country can place guerrilla
    /// </summary>
    public bool willSpawnGuerrilla
    {
        get; private set;
    }

    /// <summary>
    /// Checks if country can purchase units
    /// </summary>
    public bool CanBuyUnits
    {
        get
        {
            if(allegiance == GameMaster.allegianceTurn &&
                !isInvaded && !willSpawnGuerrilla &&
                treasury > 0)
            {
                return true;
            }
            return false;
        }
    }

    /// <summary>
    /// Checks if a secret ally can be disclosured
    /// </summary>
    public bool CanBeDisclosured
    {
        get
        {
            if(secretAllegiance == GameMaster.allegianceTurn &&
                hasSecretArmy)
            {
                return true;
            }
            return false;
        }
    }

    /// <summary>
    /// Creating country
    /// Sets alligiance, name, type, adds staringCell to area and sets it as capital
    /// Adds link to cell to country
    /// Adds guerilla money based on country type
    /// Adds sectretAliance money based on country type and allegiance
    /// Sets reparations based on allegiance
    /// Sets income turns
    /// </summary>
    /// <param name="countryName">Name of the country</param>
    /// <param name="type">Country type - poor, average, rich</param>
    /// <param name="allegiance"> Country allegiance - Sentinels, Dominion, Neutral</param>
    /// <param name="startingCell">First cell of the country, becomes it's capital</param>
    public void CreateCountry(string countryName, Allegiance allegiance,LogicalMapCell startingCell)
    {
        if (startingCell.country)
        {
            startingCell.country.area.Remove(startingCell);
        }
        area = new List<LogicalMapCell>();
        area.Add(startingCell);
        startingCell.highlight.color = AllegianceExtentions.AllegianceToColor(allegiance);
        startingCell.country = this;

        capital = startingCell;
        capitalCity = Instantiate(PropertiesKeeper.capitalPrefab, capital.transform, false);
        capitalCity.Initialize(allegiance);
        GameObject capitalCanvas = Instantiate(PropertiesKeeper.capitalCanvas, capital.transform, false);
        capitalCanvas.transform.position = capital.GetUIPosition() + new Vector3(0, 3f, 0);
        capitalCanvas.transform.GetChild(0).gameObject.SetActive(false);

        this.countryName = countryName;
        this.allegiance = Allegiance.Neutral;
        secretAllegiance = allegiance;
        guerilla = (byte) type.guerrilla;
        willSpawnGuerrilla = false;
        incomeTurnsLeft = PropertiesKeeper.incomeTurns;
        if (secretAllegiance != Allegiance.Neutral)
        {
            secretArmy = (byte)type.secretArmy;
            hasReparation = true;            
        }
        else
        {
            secretArmy = 0;
            hasReparation = false;
        }
    }

    public void ResetCountry()
    {
        capitalCity.GetComponentInChildren<MeshRenderer>().material.color = AllegianceExtentions.AllegianceToColor(allegiance);
        allegiance = Allegiance.Neutral;
        guerilla = (byte) type.guerrilla;
        willSpawnGuerrilla = false;
        incomeTurnsLeft = PropertiesKeeper.incomeTurns;
        treasury = 0;
        if (secretAllegiance != Allegiance.Neutral)
        {
            secretArmy = (byte) type.secretArmy;
            hasReparation = true;
        }
        else
        {
            secretArmy = 0;
            hasReparation = false;
        }
    }

    /// <summary>
    /// Adds area to country
    /// Check if cell's not already country's area
    /// Remove cell from it's former country if any
    /// Adds link to cell to country
    /// Changes color based on type of country
    /// </summary>
    /// <param name="areaCell">cell which will be added to area</param>
    public void AddAreaToCountry(LogicalMapCell areaCell)
    {
        if (!area.Contains(areaCell))
        {
            if (areaCell.country)
            {
                areaCell.country.area.Remove(areaCell);
            }
            area.Add(areaCell);
            areaCell.highlight.color = AllegianceExtentions.AllegianceToColor(allegiance);
            areaCell.country = this;
        }        
    }

    /// <summary>
    /// Adds area to country
    /// Check if it's not already country's area
    /// Remove cells from their former country if any
    /// Adds link to cells to country
    /// Changes color based on type of country
    /// </summary>
    /// <param name="area">list of cells which will be added to area</param>
    public void AddAreaToCountry(List<LogicalMapCell> area)
    {
        foreach (LogicalMapCell cell in area)
        {
            if (!this.area.Contains(cell))
            {
                if (cell.country)
                {
                    cell.country.area.Remove(cell);
                }
                this.area.Add(cell);
                cell.highlight.color = AllegianceExtentions.AllegianceToColor(allegiance);
                cell.country = this;
            }
        }
    }

    /// <summary>
    /// Removes area from country
    /// </summary>
    /// <param name="areaCell">cell which will be removed</param>
    public void RemoveAreaFromCountry(LogicalMapCell areaCell)
    {
        area.Remove(areaCell);
        areaCell.country = null;
        areaCell.ValidateHighlightWithTerrain();
    }

    public void ChangeAllegiance(Allegiance allegiance)
    {
        this.secretAllegiance = allegiance;
        capitalCity.ChangeAllegiance(AllegianceExtentions.AllegianceToColor(allegiance));
    }
    /// <summary>
    /// Changes capital of country
    /// Checks if cell's not already capital
    /// Adds it to area if it's not already in it
    /// Adds link to cell to country
    /// Changes color of previous capital and current according to country type
    /// </summary>
    /// <param name="cell">new capital cell</param>
    public void ChangeCapital(LogicalMapCell cell)
    {
        if (cell != capital)
        {
            if (!area.Contains(cell))
            {
                if (cell.country)
                {
                    cell.country.area.Remove(cell);
                }
                area.Add(cell);
                cell.highlight.color = AllegianceExtentions.AllegianceToColor(allegiance);
                cell.country = this;
            }
            capital = cell;
            capitalCity.transform.SetParent(cell.transform, false);
            capitalCity.ValidatePosition();
        }
    }

    /// <summary>
    /// Deletion of country
    /// Chnages color of cells to terrain color
    /// Removes links from cells that are in the area of country
    /// </summary>
    public void DeleteCountry()
    {
        foreach (LogicalMapCell cell in area)
        {
            cell.ValidateHighlightWithTerrain();
            cell.country = null;
        }
         PropertiesKeeper.countries.Remove(this);
        Destroy(capitalCity.gameObject);
        Destroy(this.gameObject);
    }

    /// <summary>
    /// Changes name of country
    /// </summary>
    /// <param name="newName">new name of country</param>
    public void ChangeName(string newName)
    {
        countryName = newName;
    }

    public void ChangeType(CountryType type)
    {
        this.type = type;
    }

    /// <summary>
    /// Buys unit using treasury money
    /// </summary>
    public void BuyWithTresury(Unit unitPrefab, LogicalMapCell cell)
    {
        if (treasury >= unitPrefab.type.cost)
        {
            treasury -= unitPrefab.type.cost;
            Unit newUnit = Instantiate(unitPrefab);
            newUnit.InitializeUnit(cell, GameMaster.allegianceTurn);
            
            if (newUnit is Plane && capitalCity.remainingCapacity > 0)
            {

            }
            else
            {
                cell.DisableHighlight();
                cell.canPlaceUnit = false;
            }
        }
        else
        {
            PropertiesKeeper.popUp.transform.GetChild(1).GetComponent<Text>().text = "This unit is too expensive";
            PropertiesKeeper.popUp.SetActive(true);
            CameraControls.fixCamera(true);
            //Debug.Log("This unit is too expansive");
        }
    }


    /// <summary>
    /// Adds turn income to treasury
    /// Checks if income turns is not 0
    /// Checks if country was not invaded by enemy units
    /// </summary>
    public void GetIncome()
    {
        if (allegiance == GameMaster.allegianceTurn &&
            incomeTurnsLeft > 0 &&
            !isInvaded &&
            !willSpawnGuerrilla) 
        {
            treasury += (byte) type.income;
            incomeTurnsLeft--;
        }
    }

    /// <summary>
    /// Buys unit with guerrilla
    /// </summary>
    public void BuyWithGuerrilla(Unit unitPrefab, LogicalMapCell cell)
    {
        if (guerilla >= unitPrefab.type.cost)
        {
            guerilla -= unitPrefab.type.cost;
            Unit newUnit = Instantiate(unitPrefab);
            newUnit.InitializeUnit(cell, GameMaster.allegianceTurn);
            if (newUnit is Plane && capitalCity.remainingCapacity > 0)
            {

            }
            else
            {
                cell.DisableHighlight();
                cell.canPlaceUnit = false;
            }
        }
        else
        {
            PropertiesKeeper.popUp.transform.GetChild(1).GetComponent<Text>().text = "This unit is too expensive";
            PropertiesKeeper.popUp.SetActive(true);
            CameraControls.fixCamera(true);
            //Debug.Log("This unit is too expensive");
        }
        if(guerilla == 0)
        {
            willSpawnGuerrilla = false;
        }        
    }

    /// <summary>
    /// Adds reparation money to treasury based on country type
    /// sets hasReparation as false
    /// </summary>
    public void GetReparations()
    {
        if (hasReparation)
        {
            treasury += (byte) type.reparations;
            hasReparation = false;            
        }
    }

    public void Disclosure()
    {
        transform.GetComponentInChildren<Renderer>().material = new Material(Shader.Find("Custom/BorderShader"));
        if (secretAllegiance == Allegiance.Dominion)
        {
            transform.GetComponentInChildren<Renderer>().material.SetFloat("_ModeDominion", 1);
        }
        else
        {
            transform.GetComponentInChildren<Renderer>().material.SetFloat("_ModeSentinels", 1);
        }

        PropertiesKeeper.popUp.transform.GetChild(1).GetComponent<Text>().text = countryName + " has been disclosured as " + secretAllegiance.ToString() + " secret ally";
        PropertiesKeeper.popUp.SetActive(true);
        CameraControls.fixCamera(true);
       // Debug.Log(countryName + " has been disclosured as " + secretAllegiance.ToString() + " secret ally");
        if (allegiance == Allegiance.Neutral)
        {
            allegiance = secretAllegiance;
            capitalCity.GetComponentInChildren<MeshRenderer>().material.color = AllegianceExtentions.AllegianceToColor(secretAllegiance);
        }        
    }

    /// <summary>
    /// Substructs sum from treasury if possible
    /// </summary>
    public void BuyWithSecretArmy(Unit unitPrefab, LogicalMapCell cell)
    {
        if (secretArmy >= unitPrefab.type.cost)
        {
            secretArmy -= unitPrefab.type.cost;
            Unit newUnit = Instantiate(unitPrefab);
            newUnit.InitializeUnit(cell, GameMaster.allegianceTurn);
            if (newUnit is Plane && capitalCity.remainingCapacity > 0)
            {

            }
            else
            {
                cell.DisableHighlight();
                cell.canPlaceUnit = false;
            }
        }
        else
        {
            PropertiesKeeper.popUp.transform.GetChild(1).GetComponent<Text>().text = "This unit is too expensive";
            PropertiesKeeper.popUp.SetActive(true);
            CameraControls.fixCamera(true);
           // Debug.Log("This unit is too expansive");
        }
    }

    /// <summary>
    /// Must be called when unit with opposite allegiance enters capital
    /// </summary>
    /// <param name="newAllegiance"></param>
    public void SwitchAllegiance(Allegiance newAllegiance)
    {
        allegiance = newAllegiance;
        capitalCity.GetComponentInChildren<MeshRenderer>().material.color = AllegianceExtentions.AllegianceToColor(allegiance);
        capitalCity.DestroyPlanes();
        if (!isInvaded && (!willSpawnGuerrilla || GameMaster.turnPhase != Phase.Battle))
        {
            TriggerLiberation();
        }
    }

    /// <summary>
    /// Must be called when country is invaded
    /// </summary>
    /// <param name="invaderAllegiance">Invading unit's allegiance,
    /// used in case a neutral country is invaded</param>
    public void TriggerInvasion(Allegiance invaderAllegiance)
    {
        PropertiesKeeper.popUp.transform.GetChild(1).GetComponent<Text>().text = countryName + " is invaded";
        PropertiesKeeper.popUp.SetActive(true);
        CameraControls.fixCamera(true);
       // Debug.Log(countryName + " is invaded");
        transform.GetComponentInChildren<Renderer>().material = new Material(Shader.Find("Custom/BorderShader"));
        if (allegiance == Allegiance.Dominion)
        {
            transform.GetComponentInChildren<Renderer>().material.SetFloat("_ModeDominion", 1);
            transform.GetComponentInChildren<Renderer>().material.SetFloat("_ModeSentinels", 1);
        }
        else
        {
            transform.GetComponentInChildren<Renderer>().material.SetFloat("_ModeDominion", 1);
            transform.GetComponentInChildren<Renderer>().material.SetFloat("_ModeSentinels", 1);
            transform.GetComponentInChildren<Renderer>().material.SetFloat("_OppositeColors", 1);
        }
        if (allegiance == Allegiance.Neutral)
        {
            SwitchAllegiance(AllegianceExtentions.Opposite(invaderAllegiance));
        }
        if (hasGuerilla)
        {
            PropertiesKeeper.popUp.transform.GetChild(1).GetComponent<Text>().text = countryName + " will spawn guerrila";
            PropertiesKeeper.popUp.SetActive(true);
            CameraControls.fixCamera(true);
            //Debug.Log(countryName + " will spawn guerrila");
            willSpawnGuerrilla = true;
        }
    }

    /// <summary>
    /// Must be called when country is libirated
    /// </summary>
    public void TriggerLiberation()
    {
        transform.GetComponentInChildren<Renderer>().material = new Material(Shader.Find("Custom/BorderShader"));
        if (allegiance == Allegiance.Dominion)
        {
            transform.GetComponentInChildren<Renderer>().material.SetFloat("_ModeDominion", 1);
        }
        else
        {
            transform.GetComponentInChildren<Renderer>().material.SetFloat("_ModeSentinels", 1);
        }

        PropertiesKeeper.popUp.transform.GetChild(1).GetComponent<Text>().text = countryName + " is libirated";
        PropertiesKeeper.popUp.SetActive(true);
        CameraControls.fixCamera(true);
        //Debug.Log(countryName + " is libirated");
        if(hasReparation && secretAllegiance != allegiance)
        {
            if (hasSecretArmy)
            {
                PropertiesKeeper.popUp.transform.GetChild(1).GetComponent<Text>().text = countryName + " was a " + secretAllegiance.ToString() + " secret ally";
                PropertiesKeeper.popUp.SetActive(true);
                CameraControls.fixCamera(true);
                //Debug.Log(countryName + " was a " + secretAllegiance.ToString() + " secret ally");
                secretArmy = 0;
            }
            PropertiesKeeper.popUp.transform.GetChild(1).GetComponent<Text>().text = countryName + " gives reparations";
            PropertiesKeeper.popUp.SetActive(true);
            CameraControls.fixCamera(true);
            //Debug.Log(countryName + " gives reparations");
            GetReparations();
        }
    }

    /// <summary>
    /// Marks country's area as selectable
    /// </summary>
    public void MakeSelectable()
    {
        foreach (LogicalMapCell cell in area)
        {
            cell.EnableHighlight(AllegianceExtentions.AllegianceToColor(GameMaster.allegianceTurn));
            cell.isSelectable = true;
            ShowMoneyText(GameMaster.turnPhase);
            
        }
    }

    /// <summary>
    /// Marks country's area as unselectable
    /// </summary>
    public void MakeUnselectable()
    {
        foreach (LogicalMapCell cell in area)
        {
            cell.DisableHighlight();
            cell.isSelectable = false;
        }
    }

    public void DontShowMoney()
    {
        capital.transform.GetChild(1).GetChild(0).gameObject.SetActive(false);
    }

    public void ShowMoneyText(Phase phase)
    {
        capital.transform.GetChild(1).GetChild(0).gameObject.SetActive(true);
        switch (phase)
        {
            case Phase.Guerrila:
                capital.transform.GetChild(1).GetChild(0).GetComponentsInChildren<Text>()[0].text = "$ " + guerilla;
                break;
            case Phase.SecretAllies:
                capital.transform.GetChild(1).GetChild(0).GetComponentsInChildren<Text>()[0].text = "$ " + secretArmy;
                break;
            case Phase.Recruitment:
                capital.transform.GetChild(1).GetChild(0).GetComponentsInChildren<Text>()[0].text = "$ " + treasury;
                break;
        }
    }
    /// <summary>
    /// Marks cells where a unit can be placed based on it's travel type and cell's terrain type
    /// </summary>
    /// <param name="unit"></param>
    public void ValidatePossibleUnitPlacements(Unit unit)
    {
        if (unit is Plane)
        {
            DropUnitPlacements();
            if(!capital.unit || (capital.unit && capital.unit.allegiance == GameMaster.allegianceTurn))
            capital.HighlightValidatedTerrain();
            capital.canPlaceUnit = true;
        }
        else
        {
            foreach (LogicalMapCell cell in area)
            {
                if (TravelTypeExtentions.CanTravelOn(cell.terrain, unit.type.travelType) &&
                    cell.unit == null)
                {
                    cell.HighlightValidatedTerrain();
                    cell.canPlaceUnit = true;
                }
                else
                {
                    cell.DisableHighlight();
                    cell.canPlaceUnit = false;
                }
                
                for (int i = (int)HexDirection.NE; i <= (int)HexDirection.NW; i++)
                {
                    LogicalMapCell neighbour;
                    if (cell.GetNeighbor((HexDirection)i))
                    {
                        neighbour = cell.GetNeighbor((HexDirection) i);
                    }
                    else
                    {
                        continue;
                    }
                    if (neighbour.terrain == TerrainType.Ocean)
                    {
                        if (TravelTypeExtentions.CanTravelOn(neighbour.terrain, unit.type.travelType) &&
                    neighbour.unit == null)
                        {
                            neighbour.HighlightValidatedTerrain();
                            neighbour.canPlaceUnit = true;
                        }
                        else
                        {
                            neighbour.DisableHighlight();
                            neighbour.canPlaceUnit = false;
                        }
                    }
                }
            }
        }
    }

    /// <summary>
    /// Clears country's area unit placement permits,
    /// used when selection of a country is dropped
    /// </summary>
    public void DropUnitPlacements()
    {
        foreach(LogicalMapCell cell in area)
        {
            cell.DisableHighlight();
            cell.canPlaceUnit = false;
        }
    }   
}


