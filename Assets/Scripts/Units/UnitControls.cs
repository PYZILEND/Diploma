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
        if(!selectedUnit && cell.isCapital && cell.capital.hasPlanes)
        {
            ShowCapitalSelection(cell);
        }        
        if(selectedUnit && selectedUnit is Plane)
        {
            if (cell.isReachable)
            {
                MoveUnit(cell);
            }
            else if (cell.inShootingRange &&
                !selectedUnit.hasAttacked &&
                !cell.isProtected)
            {
                Shoot(cell);
            }
        }
        else if (cell.unit)
        {
            if(cell.unit.allegiance == GameMaster.allegianceTurn)
            {
                if (!selectedUnit || (!(cell.unit is Transport) && !(cell.unit is Platform)))
                {
                    if (cell.isCapital && cell.capital.hasPlanes)
                    {
                        ShowCapitalSelection(cell);
                    }
                    else
                    {
                        SelectUnit(cell);
                    }
                }
                else if(cell.isReachable)
                {
                    BoardUnit(cell);
                }
            }
            else
            {
                if (selectedUnit && 
                    cell.inShootingRange &&
                    (!selectedUnit.hasAttacked || selectedUnit is SuperUnit))
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
            GameMaster.multipleSelectionPanel.ShowForTransport(Input.mousePosition, cell.unit);
        }
        selectedUnit = cell.unit;
        ValidateRanges();
    }

    static void ShowCapitalSelection(LogicalMapCell cell)
    {
        GameMaster.multipleSelectionPanel.ShowForCapital(Input.mousePosition, cell);
        if (cell.unit)
        {
            selectedUnit = cell.unit;
            ValidateRanges();
        }
    }

    public static void SelectUnit(Unit unit)
    {
        selectedUnit = unit;
        ValidateRanges();
    }

    /// <summary>
    /// Moves selected unit to said cell
    /// </summary>
    /// <param name="cell"></param>
    static void MoveUnit(LogicalMapCell cell)
    {
        selectedUnit.MoveToCell(cell);
        ValidateRanges();
    }

    /// <summary>
    /// Selected unit shoots at said cell
    /// </summary>
    /// <param name="cell"></param>
    static void Shoot(LogicalMapCell cell)
    {
        if (selectedUnit is SuperUnit)
        {
            GameMaster.multipleSelectionPanel.ShowForSuperUnit(Input.mousePosition, selectedUnit, cell.unit);
        }
        else
        {
            selectedUnit.ShootAt(cell);
            ValidateRanges();
        }
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
        DropSelection();
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
        ValidateRanges();
    }

    /// <summary>
    /// Validates shooting and moving ranges for unit in selected cell
    /// </summary>
    /// <param name="cell"></param>
    static void ValidateRanges()
    {
        Pathfinder.MarkShootingRange(selectedUnit);
        if(selectedUnit is Plane)
        {
            Pathfinder.SearchPossiblePlaneLanding(selectedUnit);
        }
        else
        {
            Pathfinder.SearchPossiblePaths(selectedUnit);
        }        
        GameMaster.logicalMap.HideAllHighlights();
        GameMaster.logicalMap.HighlightReachableRange();
    }
}
