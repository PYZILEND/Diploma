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
        map.HideAllHighlights();
        map.HighlightShootingRange();
    }

    public void GetRange(float value)
    {
        range = (int)value;
    }
}
