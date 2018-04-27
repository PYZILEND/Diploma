using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitControls : MonoBehaviour {

    static Unit selectedUnit;

    public static void DropSelection(LogicalMap map)
    {
        selectedUnit = null;
        //Add clearing shit with pathfinder
        map.HideAllHighlights();
    }

    public static void ProcessInput(LogicalMapCell cell, LogicalMap map, GameMaster master)
    {
        if (cell.unit)
        {
            if(cell.unit.isDominion == master.isDominionTurn)
            {
                SelectUnit(cell, map);
            }
            else
            {
                if (selectedUnit && !selectedUnit.hasAttacked)
                {
                    Shoot(cell);
                }
            }
        }
        else
        {
            if (selectedUnit && cell.distance<=selectedUnit.movePoints)
            {
                MoveUnit(cell, map);
            }
        }
    }

    static void SelectUnit(LogicalMapCell cell, LogicalMap map)
    {
        selectedUnit = cell.unit;
        ValidateRanges(cell, map);
    }

    static void MoveUnit(LogicalMapCell cell, LogicalMap map)
    {
        selectedUnit.MoveToCell(cell);
        ValidateRanges(cell, map);
    }

    static void Shoot(LogicalMapCell cell)
    {
        selectedUnit.ShootAt(cell);
    }

    static void ValidateRanges(LogicalMapCell cell, LogicalMap map)
    {
        Pathfinder.FindRange(UnitTypeExtentions.GetAttackRange(selectedUnit.type), map, cell);
        Pathfinder.FindWeightedDistance(selectedUnit.movePoints, map, cell);
        map.HideAllHighlights();
        map.HighlightReachableRange();
    }
}
