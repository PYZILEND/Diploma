using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// This class handles map related input like
/// unit selection, placing units, selecting countries, etc.
/// </summary>
public class MapInputs : MonoBehaviour {

    static LogicalMapCell selectedCell; //Holds last clicked cell
    LogicalMapCell hoveredCell; //Holds last hovered cell

    bool editMode;//Flag of map editing mode

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
                        if (selectedCell)
                        {
                            selectedCell.ValidateHighlightWithAllegiance();
                        }
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
                        UnitControls.ProcessInput(cell);
                    }
                }
            }
        }
        if (Input.GetKeyDown(KeyCode.Z))
        {
            GameMaster.logicalMap.HighlightAllegiance();
        }
        if (Input.GetKeyDown(KeyCode.X))
        {
            GameMaster.logicalMap.HighlightTerrain();
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
            HexCoordinates coordinates = HexCoordinates.fromPosition(hit.point);
            int index = coordinates.X + coordinates.Z * GameMaster.mapWidth + (coordinates.Z / 2);
            return GameMaster.logicalMap.cells[index];
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
    }

    public static LogicalMapCell GetSelectedCell()
    {
        return selectedCell;
    }
}
