using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitControls : MonoBehaviour {

    static Unit selectedUnit;

    /// <summary>
    /// Nullifies selected unit and clears
    /// </summary>
    public static void DropSelection()
    {
        selectedUnit = null;
        //Add clearing shit with pathfinder
        GameMaster.logicalMap.HideAllHighlights();
    }

    /// <summary>
    /// Processes player's input
    /// </summary>
    /// <param name="cell"></param>
    public static void ProcessInput(LogicalMapCell cell)
    {
        if (cell.unit)
        {
            if(cell.unit.allegiance == GameMaster.allegianceTurn)
            {
                SelectUnit(cell);
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
                MoveUnit(cell);
            }
        }
    }

    /// <summary>
    /// Selects unit in said cell
    /// </summary>
    /// <param name="cell"></param>
    static void SelectUnit(LogicalMapCell cell)
    {
        selectedUnit = cell.unit;
        ValidateRanges(cell);
    }

    /// <summary>
    /// Moves selected unit to said cell
    /// </summary>
    /// <param name="cell"></param>
    static void MoveUnit(LogicalMapCell cell)
    {
        selectedUnit.MoveToCell(cell);
        ValidateRanges(cell);
    }

    /// <summary>
    /// Selected unit shoots at said cell
    /// </summary>
    /// <param name="cell"></param>
    static void Shoot(LogicalMapCell cell)
    {
        selectedUnit.ShootAt(cell);
    }

    /// <summary>
    /// Validates shooting and moving ranges for unit in selected cell
    /// </summary>
    /// <param name="cell"></param>
    static void ValidateRanges(LogicalMapCell cell)
    {
        Pathfinder.FindRange(UnitTypeExtentions.GetAttackRange(selectedUnit.type), GameMaster.logicalMap, cell);
        Pathfinder.FindWeightedDistance(selectedUnit.movePoints, GameMaster.logicalMap, cell);
        GameMaster.logicalMap.HideAllHighlights();
        GameMaster.logicalMap.HighlightReachableRange();
    }
}
