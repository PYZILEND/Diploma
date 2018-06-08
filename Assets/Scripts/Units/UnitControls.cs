using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitControls : MonoBehaviour {

    static Unit selectedUnit;
    /// <summary>
    /// Nullifies selected unit and clears
    /// </summary>
    /// 
    public static void DropSelection()
    {
        DestroyRadius();
        selectedUnit = null;
        //Add clearing shit with pathfinder
        PropertiesKeeper.logicalMap.HideAllHighlights();
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

        DestroyRadius();

        selectedUnit = cell.unit;

        ShowRadius();
        ValidateRanges();
    }

    static void ShowRadius()
    {
        if (selectedUnit.type.attackRange > 0 && !selectedUnit.isEmbarked)
        {
            GameObject projector = Instantiate(PropertiesKeeper.projectorObject, selectedUnit.transform, false);
            float radius = 1.2f;
            switch (selectedUnit.type.attackRange)
            {
                case 2: radius = 2.1f; break;
                case 3: radius = 3f; break;
                case 4: radius = 3.9f; break;
                case 5: radius = 4.6f; break;
                case 6: radius = 5.5f; break;
            }
            projector.GetComponent<Projector>().material.SetFloat("_Radius", radius);
        }
    }

    static void DestroyRadius()
    {
        if (selectedUnit && selectedUnit.transform.childCount > 1 && selectedUnit.transform.Find("Projector(Clone)")!=null) Destroy(selectedUnit.transform.Find("Projector(Clone)").gameObject);
    }
    static void ShowCapitalSelection(LogicalMapCell cell)
    {
        GameMaster.multipleSelectionPanel.ShowForCapital(Input.mousePosition, cell);
        if (cell.unit)
        {
            DestroyRadius();
            selectedUnit = cell.unit;

            ShowRadius();

            ValidateRanges();
        }
    }

    public static void SelectUnit(Unit unit)
    {
        DestroyRadius();
        selectedUnit = unit;

        ShowRadius();

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
        PropertiesKeeper.logicalMap.HideAllHighlights();
        PropertiesKeeper.logicalMap.HighlightReachableRange();
    }
}
