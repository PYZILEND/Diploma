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

    static bool placeUnit;


    string countryName="t";
    CountryType type=CountryType.poor;
    byte allegiance=0;

    static bool isUnitDominion;

    public static GameMaster master;

    public static void ApplyChanges(LogicalMapCell cell)
    {
        if (applyTerrainType)
        {
            cell.terrain = (TerrainType)selectedTerrainType;
            cell.ValidateHighlightWithTerrain();
        }
        if (placeUnit)
        {
            Unit unit = Instantiate(master.unitPrefab);
            unit.transform.SetParent(cell.transform, false);
            unit.cell = cell;
            unit.isDominion = unitIsDominion;
            Debug.Log(unitIsDominion);
            cell.unit = unit;
            if (isUnitDominion)
            {
                unit.isDominion = true;
            }
            else
            {
                unit.isDominion = false;
            }
            unit.Initialize(UnitType.Tank);
            unit.ValidatePosition();
            master.units.Add(unit);
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




    public void GetCountryName(string value)
    {
        countryName = value;
    }

    public void GetCountryAlligance(int value)
    {
        allegiance = (byte) value;
    }

    public void GetCountryType(int value)
    {
        type = ResourcesExtentions.GetCountyType(value);
    }

    public void CreateCountry()
    {
        if ((!countryName.Equals("")) &&( MapInputs.GetSelectedCell() != null)&&
            (GameMaster.countries.Find(u => u.GetCountryName() == countryName)==null)&&
            (GameMaster.countries.Find(u => u.GetCountryCapital() == MapInputs.GetSelectedCell()) == null))
        {
            Country newCountry= Instantiate(countryPrefab);
            newCountry.CreateCountry(countryName, type, allegiance,MapInputs.GetSelectedCell());
            GameMaster.countries.Add(newCountry);
        }
    }

    public void ChangeCapital()
    {
        if ((!countryName.Equals("")&&(MapInputs.GetSelectedCell() != null)) &&
            (GameMaster.countries.Find(u => u.GetCountryCapital() == MapInputs.GetSelectedCell()) == null))
        {
            Country selectedCountry = GameMaster.countries.Find(u => u.GetCountryName() == countryName);
            if (selectedCountry != null)
            {
                selectedCountry.ChangeCapital(MapInputs.GetSelectedCell());
            }                  
        }
    }

    public void AddArea()
    {
        if ((!countryName.Equals("") && (MapInputs.GetSelectedCell() != null)) &&
            (GameMaster.countries.Find(u => u.GetCountryCapital() == MapInputs.GetSelectedCell()) == null))
        {
            Country selectedCountry = GameMaster.countries.Find(u => u.GetCountryName() == countryName);
            if (selectedCountry != null)
            {
                selectedCountry.AddAreaToCountry(MapInputs.GetSelectedCell());
            }
        }
    }

    public void DeleteCountry()
    {
        Debug.Log("start");
        if ((MapInputs.GetSelectedCell() != null))
        {
            Country selectedCountry = MapInputs.GetSelectedCell().country;
            if (selectedCountry != null)
            {
                selectedCountry.DeleteCountry();
                GameMaster.countries.Remove(selectedCountry);
            }
        }
    }

    public void ChangeName()
    {
        if ((MapInputs.GetSelectedCell() != null))
        {
            Country selectedCountry = MapInputs.GetSelectedCell().country;
            if (selectedCountry != null)
            {
                selectedCountry.ChangeName(countryName);
            }
        }

    public void SetUnitAlignment(bool value)
    {
        isUnitDominion = value;

    }
}
