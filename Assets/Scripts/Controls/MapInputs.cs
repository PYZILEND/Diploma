using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// This class handles map related input like
/// unit selection, placing units, selecting countries, etc.
/// </summary>
public class MapInputs : MonoBehaviour {

    //Holds last clicked cell
    //Needed in map editor
    public static LogicalMapCell selectedCell
    {
        get; private set;        
    } 
    LogicalMapCell hoveredCell; //Holds last hovered cell

    Unit hoveredUnit;

    bool editMode;//Flag of map editing mode

    /// <summary>
    /// Recieves map related input and invokes coresponding methods
    /// from static classes
    /// </summary>
	void Update ()
    {
        //Prevents raycasting from acting on cells that are behind UI elements
        if (!EventSystem.current.IsPointerOverGameObject())
        {
            //Editor controls only apply in edit mode
            if (editMode)
            {
                if (Input.GetMouseButtonDown(0))
                {
                    LogicalMapCell cell;
                    if (cell = GetCellUnderCoursor())
                    {
                        if (selectedCell)
                        {//Returning previously selected cell's color
                            selectedCell.ValidateHighlightWithAllegiance();
                        }
                        selectedCell = cell;
                        cell.highlight.color = Color.blue;
                        MapEditor.ApplyChanges(cell);

                    }
                }
            }
            //Non editor mode controls
            else
            {
                //Unit controls
                if (GameMaster.turnPhase == Phase.Battle)
                {
                    if (Input.GetMouseButtonDown(0))
                    {
                        LogicalMapCell cell;
                        if (cell = GetCellUnderCoursor())
                        {
                            selectedCell = cell;
                            UnitControls.ProcessInput(cell);
                        }
                    }
                }
                //Country controls
                else
                {
                    if (Input.GetMouseButtonDown(0))
                    {
                        LogicalMapCell cell;
                        if (cell = GetCellUnderCoursor())
                        {
                            selectedCell = cell;
                            CountryControls.ProcessInput(cell);
                        }
                    }
                }
            }

            //Uses hovering over cells to display information about
            //cell's country and unit
            if (hoveredCell = GetCellUnderCoursor())
            {
                if (hoveredCell.country)
                {
                    GameMaster.countryInfo.ShowCountryInfo(hoveredCell.country);
                }
                else
                {
                    GameMaster.countryInfo.Clear();
                }
                if (hoveredCell.unit)
                {
                    GameMaster.unitInfo.ShowUnitInfo(hoveredCell.unit);
                }
                else
                {
                    GameMaster.unitInfo.Hide();
                }
            }
        }

        //Can be used to highlight countryes and their allegiances
        if (Input.GetKeyDown(KeyCode.Z))
        {
            PropertiesKeeper.logicalMap.HighlightAllegiance();
        }
        //Can be used to highlight terrain
        if (Input.GetKeyDown(KeyCode.X))
        {
            PropertiesKeeper.logicalMap.HighlightTerrain();
        }
        //Can be used to highlight hex cooridnates
        if (Input.GetKeyDown(KeyCode.C))
        {
            PropertiesKeeper.logicalMap.ShowAllCoordinates();
        }
        //Can be used to hide hex cooridnates
        if (Input.GetKeyDown(KeyCode.V))
        {
            PropertiesKeeper.logicalMap.HideAllLabels();
        }
        //To drop unit selection
        if (Input.GetMouseButton(1) && GameMaster.turnPhase == Phase.Battle)
        {
            UnitControls.DropSelection();
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
            if (coordinates.ToIndex() < 0 || coordinates.ToIndex() > PropertiesKeeper.logicalMap.cells.Length) return null;
            if (PropertiesKeeper.logicalMap.cells[coordinates.ToIndex()].unit == null && hoveredUnit != null)
            {
                hoveredUnit.HideCanvas();
                hoveredUnit = null;
            }
            if (PropertiesKeeper.logicalMap.cells[coordinates.ToIndex()].unit != null && 
                PropertiesKeeper.logicalMap.cells[coordinates.ToIndex()].unit.allegiance !=GameMaster.allegianceTurn && 
                PropertiesKeeper.logicalMap.cells[coordinates.ToIndex()].unit != hoveredUnit)
            {
                if (hoveredUnit != null && hoveredUnit.allegiance == GameMaster.allegianceTurn) hoveredUnit = null;
                if (hoveredUnit !=null) hoveredUnit.HideCanvas();
                hoveredUnit = PropertiesKeeper.logicalMap.cells[coordinates.ToIndex()].unit;
                hoveredUnit.ShowCanvas();
            }
            return PropertiesKeeper.logicalMap.cells[coordinates.ToIndex()];
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

}
