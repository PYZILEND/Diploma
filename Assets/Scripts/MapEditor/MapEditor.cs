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
    public static Unit unitPrefab;

    public static void ApplyChanges(LogicalMapCell cell)
    {
        if (applyTerrainType)
        {
            cell.terrain = (TerrainType)selectedTerrainType;
            cell.ValidateHighlightWithTerrain();
        }
        if (placeUnit)
        {
            Unit unit = Instantiate(unitPrefab);
            unit.transform.SetParent(cell.transform, false);
            unit.cell = cell;
            cell.unit = unit;
            cell.ValidateRelativePhysicalPosition();
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
}
