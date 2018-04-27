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

    public void SetUnitAlignment(bool value)
    {
        isUnitDominion = value;
    }
}
