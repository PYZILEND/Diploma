using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Country : MonoBehaviour {

    /// <summary>
    /// Name of the country
    /// </summary>
    [SerializeField]
    string countryName;

    /// <summary>
    /// Country area
    /// Cells that belong to the country
    /// </summary>
    /// 
    [SerializeField]
    List<LogicalMapCell> area;

    /// <summary>
    /// Capital of the country
    /// </summary>
    LogicalMapCell capital;

    /// <summary>
    /// How many turns country will give income
    /// </summary>
    byte incomeTurnsLeft;

    /// <summary>
    /// Treasury
    /// </summary>
    byte treasury;    

    /// <summary>
    /// If not 0 then secret army
    /// was not disclosed yet
    /// </summary>
    byte secretArmy;

    /// <summary>
    /// Returns if country is a secret ally
    /// </summary>
    /// <returns>returns false if secretArmy is 0</returns>
    public bool isSecretAliance
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
    byte guerilla;

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
    bool hasReparation;

    /// <summary>
    /// County's allegiance
    /// Guardians, Dominion, Neutral
    /// </summary>
    Allegiance allegiance;

    /// <summary>
    /// Country type - poor, average, rich
    /// </summary>
    CountryType type;


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
        capital = startingCell;        
        startingCell.highlight.color = CountryTypeExtentions.GetColorCapital(type);
        startingCell.country = this;

        this.countryName = countryName;
        this.type = type;
        this.allegiance = allegiance;
        guerilla = CountryTypeExtentions.GetGuerilla(type);
        incomeTurnsLeft = GameMaster.incomeTurns;
        if (allegiance!=Allegiance.Neutral)
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
            areaCell.highlight.color = CountryTypeExtentions.GetColor(type);
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
                cell.highlight.color = CountryTypeExtentions.GetColor(type);
                cell.country = this;
            }
        }
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
                cell.highlight.color = CountryTypeExtentions.GetColorCapital(type);
                cell.country = this;
            }
            capital.highlight.color = CountryTypeExtentions.GetColor(type);
            capital = cell;
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
    /// Substructs sum from treasury if possible
    /// </summary>
    /// <param name="sum">sum of money that is spend</param>
    /// <returns>returns false if sum is bigger than treasury</returns>
    public bool SpendMoney(byte sum)
    {
        if (treasury - sum >= 0)
        {
            treasury -= sum;
            return true;
        }
        return false;
    }

    /// <summary>
    /// Adds turn income to treasury
    /// Checks if income turns is not 0
    /// Checks if country was not invaded by enemy units
    /// </summary>
    public void AddToTreasury()
    {
        if ((allegiance != Allegiance.Neutral) && (incomeTurnsLeft != 0) && (CheckIfInvasion())) 
        {
            treasury += CountryTypeExtentions.GetIncome(type);
            incomeTurnsLeft--;
        }
    }

    /// <summary>
    /// Checks if country was not invaded by enemy units
    /// </summary>
    /// <returns>returns true if there'no enemy units in cells of country's area</returns>
    public bool CheckIfInvasion()
    {
        foreach (LogicalMapCell cell in area)
        {
            if ((cell.unit!=null) && (cell.unit.allegiance != allegiance))
            {
                return false;
            }
        }
        return true;
    }

    /// <summary>
    /// Substructs sum from guerilla money
    /// </summary>
    /// <param name="sum">money spend sum</param>
    /// <returns>returns false if sum is bigger than guerilla money</returns>
    public bool SpendGuerilla(byte sum)
    {
        if (guerilla - sum >= 0)
        {
            guerilla -= sum;
            return true;
        }
        return false;
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

    /// <summary>
    /// Substructs sum from secret army money
    /// </summary>
    /// <param name="sum">money spend sum</param>
    /// <returns>returns false if sum is bigger than secret army money</returns>
    public bool SpendSecretArmyMoney(byte sum)
    {
        if (secretArmy - sum >= 0)
        {
            secretArmy -= sum;
            return true;
        }
        return false;
    }    

    /// <summary>
    /// Get country name
    /// </summary>
    /// <returns>returns country's name</returns>
    public string GetCountryName()
    {
        return countryName;
    }

    /// <summary>
    /// Get country capital LogicalMapCell
    /// </summary>
    /// <returns>returns cell of country's capital</returns>
    public LogicalMapCell GetCountryCapital()
    {
        return capital;
    }
}


