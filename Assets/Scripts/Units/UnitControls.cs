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
        GameMaster.multipleSelectionPanel.Hide();
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
                if (!selectedUnit || (!(cell.unit is Transport) && !(cell.unit is Platform)))
                {
                    SelectUnit(cell);
                }
                else if(cell.isReachable)
                {
                    BoardUnit(cell);
                }
            }
            else
            {
                if (selectedUnit && !selectedUnit.hasAttacked && cell.inShootingRange)
                {
                    Shoot(cell);
                }
            }
        }
        else
        {
            if (selectedUnit && cell.isReachable)
            {
                if (selectedUnit.isEmbarked || selectedUnit.isOnPlatform)
                {
                    Disembark(cell);
                }
                else
                {
                    MoveUnit(cell);
                }
            }
        }
    }

    /// <summary>
    /// Selects unit in said cell
    /// </summary>
    /// <param name="cell"></param>
    static void SelectUnit(LogicalMapCell cell)
    {
        if (cell.unit.type.capacity > 0)
        {
            GameMaster.multipleSelectionPanel.Show(Input.mousePosition, cell.unit);
        }
        selectedUnit = cell.unit;
        ValidateRanges(cell);
    }

    public static void SelectUnit(Unit unit)
    {
        selectedUnit = unit;
        ValidateRanges(unit.cell);
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

    static void BoardUnit(LogicalMapCell cell)
    {
        if(cell.unit is Transport)
        {
            selectedUnit.Embark(cell);
        }
        else if(cell.unit is Platform)
        {
            selectedUnit.BoardPlatform(cell);
        }
    }

    static void Disembark(LogicalMapCell cell)
    {
        if (selectedUnit.isEmbarked)
        {
            selectedUnit.Disembark(cell);
        }
        else if (selectedUnit.isOnPlatform)
        {
            selectedUnit.LeavePlatform(cell);
        }
    }

    /// <summary>
    /// Validates shooting and moving ranges for unit in selected cell
    /// </summary>
    /// <param name="cell"></param>
    static void ValidateRanges(LogicalMapCell cell)
    {
        Pathfinder.MarkShootingRange(selectedUnit.type.attackRange, cell);
        Pathfinder.SearchPossiblePaths(selectedUnit.movePoints, cell, selectedUnit.type);
        GameMaster.logicalMap.HideAllHighlights();
        GameMaster.logicalMap.HighlightReachableRange();
    }
}
