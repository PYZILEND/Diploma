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
    CountryType type=CountryType.poor;
    static Allegiance countryAllegiance;

    public static List<Country> countries;

    public static LogicalMapCell selectedCell;

    void Awake()
    {
        if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex != 1)
        {
            GameMaster.countries= new List<Country>();
            countries = GameMaster.countries;
        }
        else
        {
            MapCreator.countries = new List<Country>();
            countries = MapCreator.countries;
        }
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
            Unit.CreateUnit(GameMaster.unitPrefabs[3], cell, unitAllegiance);
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
    public void GetCountryType(int value)
    {
        type = CountryTypeExtentions.GetCountyType(value);
    }

    /// <summary>
    /// Creates a country with capital at selected cell
    /// </summary>
    public void CreateCountry()
    {
        if ((!countryName.Equals("")) && (selectedCell != null) &&
            (countries.Find(u => u.countryName == countryName) == null) &&
            (countries.Find(u => u.capital == selectedCell) == null))
        {
            Country newCountry;
            if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex != 1)
            {
                newCountry = Instantiate(GameMaster.countryPrefab);
            }
            else
            {
                newCountry = Instantiate(MapCreator.countryPrefab);
            }
            newCountry.CreateCountry(countryName, type, countryAllegiance, selectedCell);
            countries.Add(newCountry);
        }
        if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex != 1)
        {
            GameMaster.logicalMap.HighlightAllegiance();
        }
        else
        {
            MapCreator.newMap.HighlightAllegiance();
        }
    }

    /// <summary>
    /// Changes country capital to selected cell
    /// </summary>
    public void ChangeCapital()
    {
        if ((!countryName.Equals("") && (selectedCell != null)) &&
            (countries.Find(u => u.capital == selectedCell) == null))
        {
            Country selectedCountry =countries.Find(u => u.countryName == countryName);
            if (selectedCountry != null)
            {
                selectedCountry.ChangeCapital(selectedCell);
            }                  
        }
        if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex != 1)
        {
            GameMaster.logicalMap.HighlightAllegiance();
        }
        else
        {
            MapCreator.newMap.HighlightAllegiance();
        }
    }

    /// <summary>
    /// Adds selected cell to country
    /// </summary>
    public void AddArea()
    {
        if ((!countryName.Equals("") && (selectedCell != null)) &&
            (countries.Find(u => u.capital == selectedCell) == null))
        {
            Country selectedCountry =countries.Find(u => u.countryName == countryName);
            if (selectedCountry != null)
            {
                selectedCountry.AddAreaToCountry(selectedCell);
            }
        }
        if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex != 1)
        {
            GameMaster.logicalMap.HighlightAllegiance();
        }
        else
        {
            MapCreator.newMap.HighlightAllegiance();
        }
    }

    /// <summary>
    /// Remove selected cell from country
    /// </summary>
    public void RemoveArea()
    {
        if ((selectedCell != null) &&
            (countries.Find(u => u.capital == selectedCell) == null))
        {
            Country selectedCountry = selectedCell.country;
            if (selectedCountry != null)
            {
                selectedCountry.RemoveAreaFromCountry(selectedCell);
            }
        }
        if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex != 1)
        {
            GameMaster.logicalMap.HighlightAllegiance();
        }
        else
        {
            MapCreator.newMap.HighlightAllegiance();
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
        if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex != 1)
        {
            GameMaster.logicalMap.HighlightAllegiance();
        }
        else
        {
            MapCreator.newMap.HighlightAllegiance();
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
            if (selectedCountry != null&& selectedCountry.countryName!=countryName)
            {
                selectedCountry.ChangeName(countryName);
            }
        }
    }

    public void ChangeAllegiance()
    {
        if ((selectedCell != null))
        {
            Country selectedCountry = selectedCell.country;
            if (selectedCountry != null && selectedCountry.allegiance != countryAllegiance)
            {
                selectedCountry.ChangeAlligiance(countryAllegiance);
            }
        }
        if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex != 1)
        {
            GameMaster.logicalMap.HighlightAllegiance();
        }
        else
        {
            MapCreator.newMap.HighlightAllegiance();
        }
    }

    public void ChangeType()
    {
        if ((selectedCell != null))
        {
            Country selectedCountry = selectedCell.country;
            if (selectedCountry != null && selectedCountry.type!=type)
            {
                selectedCountry.ChangeType(type);
            }
        }
    }

    public void Reset()
    {
        while (countries.Count > 0)
        {
            countries[0].DeleteCountry();
        }
        if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex != 1)
        {
            GameMaster.logicalMap.HighlightAllegiance();
        }
        else
        {
            MapCreator.newMap.HighlightAllegiance();
        }
    }
}
