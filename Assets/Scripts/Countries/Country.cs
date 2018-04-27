using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Country : MonoBehaviour {
    /// <summary>
    /// name of the country
    /// </summary>
    string countryName;
    /// <summary>
    /// country area
    /// cells that belong to the country
    /// </summary>
    List<LogicalMapCell> area;
    /// <summary>
    /// capital of the country
    /// </summary>
    LogicalMapCell capital;
    /// <summary>
    /// how many turns country will give income
    /// </summary>
    byte incomeTurnsLeft;
    /// <summary>
    /// treasury
    /// </summary>
    byte treasury;

    /// <summary>
    /// if not 0 then has reparations and secret army money
    /// was not disclosed yet
    /// </summary>
    byte secretAliance;

    /// <summary>
    /// if not 0 then has guerilla army money left
    /// </summary>
    byte guerilla;

    /// <summary>
    /// if true then has reparations money left
    /// </summary>
    bool hasReparations;

    /// <summary>
    /// 0 - is ??
    /// 1 - is dominion
    /// 2 - country is neutral
    /// </summary>
    byte allegiance;

    /// <summary>
    /// country type - poor, average, rich
    /// </summary>
    CountryType type;


    /// <summary>
    /// Creating country
    /// Sets alligiance, name, type, adds statingCell to area and sets it as capital
    /// Adds link to cell to country
    /// Adds guerilla money based on country type
    /// Adds sectretAliance money based on country type and allegiance
    /// Sets reparations based on allegiance
    /// Sets income turns
    /// </summary>
    /// <param name="countryName">name of the country</param>
    /// <param name="type">country type - poor, average, rich</param>
    /// <param name="allegiance"> 0 - ??, 1 - dominion, 2 - neutral</param>
    /// <param name="startingCell">first cell of the country</param>
    public void CreateCountry(string countryName, CountryType type, byte allegiance,LogicalMapCell startingCell)
    {
        area = new List<LogicalMapCell>();
        area.Add(startingCell);
        this.countryName = countryName;
        this.type = type;
        this.allegiance = allegiance;
        capital = startingCell;
        startingCell.highlight.color = ResourcesExtentions.GetColorCapital(type);
        startingCell.country = this;
        guerilla = ResourcesExtentions.GetGuerilla(type);
        incomeTurnsLeft = GameMaster.incomeTurns;
        if (allegiance<2)
        {
            secretAliance= ResourcesExtentions.GetSecretArmy(type);
            hasReparations = true;
        }
        else
        {
            secretAliance = 0;
            hasReparations = false;
        }
    }

    /// <summary>
    /// Adds area to country
    /// Check if cell's not already country's area
    /// Adds link to cell to country
    /// Changes color based on type of country
    /// </summary>
    /// <param name="areaCell">cell which will be added to area</param>
    public void AddAreaToCountry(LogicalMapCell areaCell)
    {
        if (!area.Contains(areaCell))
        {
            area.Add(areaCell);
            areaCell.highlight.color = ResourcesExtentions.GetColor(type);
            areaCell.country = this;
        }        
    }
    /// <summary>
    /// Adds area to country
    /// Check if it's not already country's area
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
                this.area.Add(cell);
                cell.highlight.color = ResourcesExtentions.GetColor(type);
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
                area.Add(cell);
                cell.highlight.color = ResourcesExtentions.GetColorCapital(type);
                cell.country = this;
            }
            capital.highlight.color = ResourcesExtentions.GetColor(type);
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
        if ((allegiance != 3) && (incomeTurnsLeft != 0) && (CheckIfInvasion()))
        {
            treasury += ResourcesExtentions.GetIncome(type);
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
            if ((cell.unit!=null)&&(cell.unit.isDominion != System.Convert.ToBoolean(allegiance)))
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
    /// Returns status of guerilla
    /// </summary>
    /// <returns>returns false if no guerilla money left</returns>
    public bool hasGuerilla()
    {
        if (guerilla > 0)
        {
            return true;
        }
        return false;
    }

    /// <summary>
    /// Adds reparation money to treasury based on country type
    /// sets hasReparations as false
    /// </summary>
    public void UseReparations()
    {
        if (hasReparations)
        {
            treasury += ResourcesExtentions.GetReparations(type);
            hasReparations = false;
        }
    }

    /// <summary>
    /// Substructs sum from secret army money
    /// </summary>
    /// <param name="sum">money spend sum</param>
    /// <returns>returns false if sum is bigger than secret army money</returns>
    public bool SpendSecretArmyMoney(byte sum)
    {
        if (secretAliance - sum >= 0)
        {
            secretAliance -= sum;
            return true;
        }
        return false;
    }

    /// <summary>
    /// Returns status of guerilla
    /// </summary>
    /// <returns>returns false if secretAliance is 0</returns>
    public bool isSecretAliance()
    {
        if (secretAliance > 0)
        {
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

public enum CountryType
{
    poor, average, rich
}

public static class ResourcesExtentions
{
    /// <summary>
    /// Returns country type based on number from 0 to 2
    /// </summary>
    /// <param name="type">number of type</param>
    /// <returns>country type</returns>
    public static CountryType GetCountyType(int type)
    {
        switch (type)
        {
            case 0: return CountryType.poor;
            case 1: return CountryType.average;
            case 2: return CountryType.rich;
            default: return CountryType.average;
        }
    }

    public static Color GetColor(CountryType type)
    {
        switch (type)
        {
            case CountryType.poor: return Color.black;
            case CountryType.average: return Color.green;
            case CountryType.rich: return Color.red;
            default: return Color.magenta;
        }
    }

    public static Color GetColorCapital(CountryType type)
    {
        switch (type)
        {
            case CountryType.poor: return Color.cyan;
            case CountryType.average: return Color.yellow;
            case CountryType.rich: return Color.gray;
            default: return Color.magenta;
        }
    }
    public static byte GetIncome(CountryType type)
    {
        switch (type)
        {
            case CountryType.poor: return 2;
            case CountryType.average: return 3;
            case CountryType.rich: return 4;
            default: return 0;
        }
    }

    public static byte GetSecretArmy(CountryType type)
    {
        switch (type)
        {
            case CountryType.poor: return 8;
            case CountryType.average: return 12;
            case CountryType.rich: return 18;
            default: return 0;
        }
    }

    public static byte GetReparations(CountryType type)
    {
        switch (type)
        {
            case CountryType.poor: return 8;
            case CountryType.average: return 12;
            case CountryType.rich: return 18;
            default: return 0;
        }
    }

    public static byte GetGuerilla(CountryType type)
    {
        switch (type)
        {
            case CountryType.poor: return 4;
            case CountryType.average: return 6;
            case CountryType.rich: return 8;
            default: return 0;
        }
    }
}
