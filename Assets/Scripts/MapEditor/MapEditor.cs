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
    Country countryPrefab;
    static Allegiance countryAllegiance;    
    
    void Start()
    {
        countryPrefab = GameMaster.countryPrefabs[0];
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
            Unit newUnit = Instantiate(GameMaster.unitPrefabs[3]);
            newUnit.InitializeUnit(cell, unitAllegiance);
        }
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
        countryPrefab = GameMaster.countryPrefabs[value];
    }

    /// <summary>
    /// Creates a country with capital at selected cell
    /// </summary>
    public void CreateCountry()
    {
        if ((!countryName.Equals("")) && (MapInputs.selectedCell != null) &&
            (GameMaster.countries.Find(u => u.countryName == countryName)==null) &&
            (GameMaster.countries.Find(u => u.capital == MapInputs.selectedCell) == null))
        {
            Country newCountry = Instantiate(countryPrefab);
            newCountry.CreateCountry(countryName, countryAllegiance, MapInputs.selectedCell);
            GameMaster.countries.Add(newCountry);
        }
    }

    /// <summary>
    /// Changes country capital to selected cell
    /// </summary>
    public void ChangeCapital()
    {
        if ((!countryName.Equals("") && (MapInputs.selectedCell != null)) &&
            (GameMaster.countries.Find(u => u.capital == MapInputs.selectedCell) == null))
        {
            Country selectedCountry = GameMaster.countries.Find(u => u.countryName == countryName);
            if (selectedCountry != null)
            {
                selectedCountry.ChangeCapital(MapInputs.selectedCell);
            }                  
        }
    }

    /// <summary>
    /// Adds selected cell to country
    /// </summary>
    public void AddArea()
    {
        if ((!countryName.Equals("") && (MapInputs.selectedCell != null)) &&
            (GameMaster.countries.Find(u => u.capital == MapInputs.selectedCell) == null))
        {
            Country selectedCountry = GameMaster.countries.Find(u => u.countryName == countryName);
            if (selectedCountry != null)
            {
                selectedCountry.AddAreaToCountry(MapInputs.selectedCell);
            }
        }
    }

    /// <summary>
    /// Remove selected cell from country
    /// </summary>
    public void RemoveArea()
    {
        if ((MapInputs.selectedCell != null) &&
            (GameMaster.countries.Find(u => u.capital == MapInputs.selectedCell) == null))
        {
            Country selectedCountry = MapInputs.selectedCell.country;
            if (selectedCountry != null)
            {
                selectedCountry.RemoveAreaFromCountry(MapInputs.selectedCell);
            }
        }
    }

    /// <summary>
    /// Deletes selected country
    /// </summary>
    public void DeleteCountry()
    {
        if ((MapInputs.selectedCell != null))
        {
            Country selectedCountry = MapInputs.selectedCell.country;
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
        if ((MapInputs.selectedCell != null))
        {
            Country selectedCountry = MapInputs.selectedCell.country;
            if (selectedCountry != null)
            {
                selectedCountry.ChangeName(countryName);
            }
        }
    }
}
