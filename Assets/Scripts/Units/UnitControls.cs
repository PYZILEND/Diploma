using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitControls : MonoBehaviour {

    static Unit selectedUnit;
    static int range;

    public static void SelectUnit(LogicalMapCell cell, LogicalMap map)
    {
        selectedUnit = cell.unit;
        Pathfinder.FindRange(range, map, cell);
        Pathfinder.FindWeightedDistance(range, map, cell);
        map.HideAllHighlights();
        map.HighlightReachableRange();
    }

    public void GetRange(float value)
    {
        range = (int)value;
    }

    public static void MoveUnit(LogicalMapCell cell, LogicalMap map)
    {
        map.HideAllHighlights();
        map.HighlightReachableRange();
        Pathfinder.FindPath(map, selectedUnit.cell, cell);
    }

    public static void ShootWithUnit(LogicalMapCell cell, LogicalMap map)
    {
        map.HideAllHighlights();
        map.HighlightReachableRange();
        Pathfinder.FindPath(map, selectedUnit.cell, cell);
    }
}
