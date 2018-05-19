using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    public Capital capitalCity;

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
    /// Guardians, Dominion, Neutral
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
    public CountryType type
    {
        get; private set;
    }

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
    /// <param name="allegiance"> Country allegiance - Guardians, Dominion, Neutral</param>
    /// <param name="startingCell">First cell of the country, becomes it's capital</param>
    public void CreateCountry(string countryName, CountryType type, Allegiance allegiance,LogicalMapCell startingCell)
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
        capitalCity = Instantiate(GameMaster.capitalPrefab, capital.transform, false);
        capitalCity.Initialize(allegiance);

        this.countryName = countryName;
        this.type = type;
        this.allegiance = Allegiance.Neutral;
        secretAllegiance = allegiance;
        guerilla = CountryTypeExtentions.GetGuerilla(type);
        willSpawnGuerrilla = false;
        incomeTurnsLeft = GameMaster.incomeTurns;
        if (secretAllegiance != Allegiance.Neutral)
        {
            secretArmy= CountryTypeExtentions.GetSecretArmy(type);
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
        guerilla = CountryTypeExtentions.GetGuerilla(type);
        willSpawnGuerrilla = false;
        incomeTurnsLeft = GameMaster.incomeTurns;
        treasury = 0;
        if (secretAllegiance != Allegiance.Neutral)
        {
            secretArmy = CountryTypeExtentions.GetSecretArmy(type);
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
        GameMaster.countries.Remove(this);
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

    /// <summary>
    /// Buys unit using treasury money
    /// </summary>
    public void BuyWithTresury(Unit unit, LogicalMapCell cell)
    {
        if (treasury >= unit.type.cost)
        {
            treasury -= unit.type.cost;
            CreateUnit(unit, cell);
        }
        else
        {
            Debug.Log("This unit is too expansive");
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
            incomeTurnsLeft != 0 &&
            !isInvaded &&
            !willSpawnGuerrilla) 
        {
            treasury += CountryTypeExtentions.GetIncome(type);
            incomeTurnsLeft--;
        }
    }

    /// <summary>
    /// Buys unit with guerrilla
    /// </summary>
    public void BuyWithGuerrilla(Unit unit, LogicalMapCell cell)
    {
        if (guerilla >= unit.type.cost)
        {
            guerilla -= unit.type.cost;
            CreateUnit(unit, cell);
        }
        else
        {
            Debug.Log("This unit is too expansive");
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
            treasury += CountryTypeExtentions.GetReparations(type);
            hasReparation = false;
        }
    }

    public void Disclosure()
    {
        Debug.Log(countryName + " has been disclosured as " + secretAllegiance.ToString() + " secret ally");
        if (allegiance == Allegiance.Neutral)
        {
            allegiance = secretAllegiance;
            capitalCity.GetComponentInChildren<MeshRenderer>().material.color = AllegianceExtentions.AllegianceToColor(secretAllegiance);
        }
    }

    /// <summary>
    /// Substructs sum from treasury if possible
    /// </summary>
    public void BuyWithSecretArmy(Unit unit, LogicalMapCell cell)
    {
        if (secretArmy >= unit.type.cost)
        {
            secretArmy -= unit.type.cost;
            CreateUnit(unit, cell);
        }
        else
        {
            Debug.Log("This unit is too expansive");
        }
    }

    void CreateUnit(Unit unit, LogicalMapCell cell)
    {
        if (unit is AntiAir)
        {
            AntiAir.CreateUnit(unit, cell, GameMaster.allegianceTurn);
        }
        else if (unit is Carrier)
        {
            Carrier.CreateUnit(unit, cell, GameMaster.allegianceTurn);
        }
        else if (unit is Plane)
        {
            Plane.CreateUnit(unit, cell, GameMaster.allegianceTurn);
        }
        else if (unit is Platform)
        {
            Platform.CreateUnit(unit, cell, GameMaster.allegianceTurn);
        }
        else if (unit is SuperUnit)
        {
            SuperUnit.CreateUnit(unit, cell, GameMaster.allegianceTurn);
        }
        else if (unit is Transport)
        {
            Transport.CreateUnit(unit, cell, GameMaster.allegianceTurn);
        }
        else
        {
            Unit.CreateUnit(unit, cell, GameMaster.allegianceTurn);
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
        Debug.Log(countryName + " is invaded");
        if(allegiance == Allegiance.Neutral)
        {
            SwitchAllegiance(AllegianceExtentions.Opposite(invaderAllegiance));
        }
        if (hasGuerilla)
        {
            Debug.Log(countryName + " will spawn guerrila");
            willSpawnGuerrilla = true;
        }
    }

    /// <summary>
    /// Must be called when country is libirated
    /// </summary>
    public void TriggerLiberation()
    {
        Debug.Log(countryName + " is libirated");
        if(hasReparation && secretAllegiance != allegiance)
        {
            if (hasSecretArmy)
            {
                Debug.Log(countryName + " was a " + secretAllegiance.ToString() + " secret ally");
                secretArmy = 0;
            }
            Debug.Log(countryName + " gives reparations");
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

    /// <summary>
    /// Marks cells where a unit can be placed based on it's travel type and cell's terrain type
    /// </summary>
    /// <param name="unit"></param>
    public void ValidatePossibleUnitPlacements(Unit unit)
    {
        foreach (LogicalMapCell cell in area)
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


