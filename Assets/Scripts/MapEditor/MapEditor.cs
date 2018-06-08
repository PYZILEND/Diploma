using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This class handles input in map editing mode
/// </summary>
public class MapEditor : MonoBehaviour
{
    //Stores values to adjust
    static int selectedTerrainType;
    static bool applyTerrainType;

    //Unit placement fields
    static bool placeUnit;
    static Allegiance unitAllegiance;

    //Country editing fields
    string countryName="t";
    static Allegiance countryAllegiance;
    Country countryPrefab;

    public static LogicalMapCell selectedCell;

    void Start()
    {
        countryPrefab = PropertiesKeeper.countryPrefabs[0];
    }

    /// <summary>
    /// Applyes terrain and unit placement changes to specified cell
    /// </summary>
    /// <param name="cell"></param>
    public static void ApplyChanges(LogicalMapCell cell)
    {
        if (applyTerrainType)
        {
            cell.terrain = (TerrainType)selectedTerrainType;
            cell.ValidateHighlightWithTerrain();
        }

        if (placeUnit)
        {
            Unit newUnit = Instantiate(PropertiesKeeper.unitPrefabs[3]);
            newUnit.InitializeUnit(cell, unitAllegiance);
        }
        selectedCell = cell;
    }

    /// <summary>
    /// Takes terrain type from UI slider
    /// </summary>
    /// <param name="type"></param>
    public void SetTerrainType(float type)
    {
        selectedTerrainType = (int)type;
    }

    /// <summary>
    /// Takes weather terrain type should be applyed from UI checkbox
    /// </summary>
    /// <param name="value"></param>
    public void SetApplyTerrain(bool value)
    {
        applyTerrainType = value;
    }

    /// <summary>
    /// Takes weather unit must be placed in that cell
    /// </summary>
    public void SetPlaceUnit(bool value)
    {
        placeUnit = value;
    }
    
    /// <summary>
    /// Gets unit's allegiance
    /// </summary>
    /// <param name="value"></param>
    public void GetUnitAllegiance(int value)
    {
        unitAllegiance = (Allegiance)value;
    }

    /// <summary>
    /// Takes country name
    /// </summary>
    /// <param name="value"></param>
    public void GetCountryName(string value)
    {
        countryName = value;
    }

    /// <summary>
    /// Takes country allegiance
    /// </summary>
    /// <param name="value"></param>
    public void GetCountryAlligance(int value)
    {
        countryAllegiance = (Allegiance) value;
    }

    /// <summary>
    /// Takes country type
    /// </summary>
    /// <param name="value"></param>
    public void GetCountryPrefab(int value)
    {
        countryPrefab = PropertiesKeeper.countryPrefabs[value];
    }

    /// <summary>
    /// Creates a country with capital at selected cell
    /// </summary>
    public void CreateCountry()
    {
        if ((!countryName.Equals("")) && (selectedCell != null) &&
            ( PropertiesKeeper.countries.Find(u => u.countryName == countryName)==null) &&
            ( PropertiesKeeper.countries.Find(u => u.capital == selectedCell) == null))
        {
            Country newCountry = Instantiate(countryPrefab);
            newCountry.CreateCountry(countryName, countryAllegiance, selectedCell);
             PropertiesKeeper.countries.Add(newCountry);
        }
    }

    /// <summary>
    /// Changes country capital to selected cell
    /// </summary>
    public void ChangeCapital()
    {
        if ((!countryName.Equals("") && (selectedCell != null)) &&
            ( PropertiesKeeper.countries.Find(u => u.capital == selectedCell) == null))
        {
            Country selectedCountry =  PropertiesKeeper.countries.Find(u => u.countryName == countryName);
            if (selectedCountry != null)
            {
                selectedCountry.ChangeCapital(selectedCell);
            }                  
        }
    }

    /// <summary>
    /// Adds selected cell to country
    /// </summary>
    public void AddArea()
    {
        if ((!countryName.Equals("") && (selectedCell != null)) &&
            ( PropertiesKeeper.countries.Find(u => u.capital == selectedCell) == null))
        {
            Country selectedCountry =  PropertiesKeeper.countries.Find(u => u.countryName == countryName);
            if (selectedCountry != null)
            {
                selectedCountry.AddAreaToCountry(selectedCell);
            }
        }
    }

    /// <summary>
    /// Remove selected cell from country
    /// </summary>
    public void RemoveArea()
    {
        if ((selectedCell != null) &&
            ( PropertiesKeeper.countries.Find(u => u.capital == selectedCell) == null))
        {
            Country selectedCountry = selectedCell.country;
            if (selectedCountry != null)
            {
                selectedCountry.RemoveAreaFromCountry(selectedCell);
            }
        }
    }

    /// <summary>
    /// Deletes selected country
    /// </summary>
    public void DeleteCountry()
    {
        if ((selectedCell != null))
        {
            Country selectedCountry = selectedCell.country;
            if (selectedCountry != null)
            {
                selectedCountry.DeleteCountry();                
            }
        }
    }

    /// <summary>
    /// Changes country name
    /// </summary>
    public void ChangeName()
    {
        if ((selectedCell != null))
        {
            Country selectedCountry = selectedCell.country;
            if (selectedCountry != null)
            {
                selectedCountry.ChangeName(countryName);
            }
        }
    }
}
