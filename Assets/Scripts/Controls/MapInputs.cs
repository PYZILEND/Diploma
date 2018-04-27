using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// This class handles map related input like
/// unit selection, placing units, selecting countries, etc.
/// </summary>
public class MapInputs : MonoBehaviour {

    GameMaster master; //Needed to reference logical map

    static LogicalMapCell selectedCell; //Holds last clicked cell
    LogicalMapCell hoveredCell; //Holds last hovered cell

    bool editMode;

    /// <summary>
    /// Initializes map controls
    /// </summary>
    void Awake()
    {
        master = GetComponentInParent<GameMaster>();
    }

    /// <summary>
    /// Recieves map related input and invokes coresponding methods
    /// from static classes
    /// </summary>
	void Update ()
    {
        if (editMode)
        {
            if (Input.GetMouseButton(0))
            {
                LogicalMapCell cell;
                if (cell = GetCellUnderCoursor())
                {
                    if (cell != selectedCell)
                    {
                        selectedCell = cell;
                        cell.highlight.color = Color.blue;
                        MapEditor.ApplyChanges(cell);
                    }
                }
            }
        }
        else
        {
            if (Input.GetMouseButton(0))
            {
                LogicalMapCell cell;
                if (cell = GetCellUnderCoursor())
                {
                    if (cell != selectedCell)
                    {
                        selectedCell = cell;
                        if (cell.unit)
                        {
                            UnitControls.SelectUnit(cell, master.logicalMap);
                        }
                    }
                }
            }
            else
            {
                if (Input.GetKeyDown(KeyCode.LeftShift))
                {
                    LogicalMapCell cell;
                    if (cell = GetCellUnderCoursor())
                    {
                        if (selectedCell.unit)
                        {
                            if (cell.isReachable)
                            {
                                UnitControls.MoveUnit(cell, master.logicalMap);
                            }
                        }
                    }
                }
            }
        }        
    }

    /// <summary>
    /// Uses current mouse position to raycast
    /// into the world space.
    /// If ray hits a collider, returns cell using
    /// hit point's X and Z coordinates.
    /// Returns null if no collider is hit.
    /// </summary>
    LogicalMapCell GetCellUnderCoursor()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if(Physics.Raycast(ray, out hit))
        {
            LogicalMap map = master.logicalMap;
            HexCoordinates coordinates = HexCoordinates.fromPosition(hit.point);
            int index = coordinates.X + coordinates.Z * master.mapWidth + (coordinates.Z / 2);
            return map.cells[index];
        }

        return null;
    }

    /// <summary>
    /// Switches controls between editor mode and game mode
    /// </summary>
    /// <param name="value"></param>
    public void EditMode(bool value)
    {
        editMode = value;
        /*
        if (value)
        {
            master.logicalMap.ShowAllHighlights();
            master.logicalMap.ShowAllCoordinates();
        }
        else
        {
            master.logicalMap.HideAllHighlights();
            master.logicalMap.HideAllLabels();
        }*/
    }

    public static LogicalMapCell GetSelectedCell()
    {
        return selectedCell;
    }

}
