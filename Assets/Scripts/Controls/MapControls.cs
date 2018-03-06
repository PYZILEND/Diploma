using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This class handles map related input like
/// unit selection, placing units, selecting countries, etc.
/// </summary>
public class MapControls : MonoBehaviour {

    GameMaster master; //Needed to reference logical map

    //Needed to reference itself 
    //for switching between editor and game mode
    MapControls controls;

    int distance;

    LogicalMapCell selectedCell; //Holds last clicked cell

    LogicalMapCell hoveredCell; //Holds last hovered cell

    /// <summary>
    /// Initializes map controls
    /// </summary>
    void Awake()
    {
        master = GetComponentInParent<GameMaster>();
        controls = GetComponent<MapControls>();
    }

    /// <summary>
    /// Awaits input
    /// </summary>
	void Update ()
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
                       selectedCell.DisableHighlight();
                    }
                    //master.logicalMap.HideAllHighlights();
                    //master.logicalMap.EraseShootingRange();
                    //master.logicalMap.HideAllLabels();
                    // Pathfinder.FindDistance(distance,master.logicalMap, cell);
                    //master.logicalMap.HighlightShootingRange();
                    //Pathfinder.FindWeightedDistance(distance, master.logicalMap, cell);
                    cell.EnableHighlight(Color.black);
                    //master.logicalMap.ShowDistanceLabel();
                    selectedCell = cell;
                }
            }
        }
        else
        {
           if (Input.GetKey(KeyCode.LeftShift))
            {
                if (selectedCell)
                {
                    LogicalMapCell cell;
                    if (cell = GetCellUnderCoursor())
                    {
                        if (cell != selectedCell)
                        {
                            master.logicalMap.ShowAllHighlights();
                            selectedCell.EnableHighlight(Color.black);
                            /*if (hoveredCell)
                            {
                                hoveredCell.DisableHighlight();
                            }*/
                            cell.EnableHighlight(Color.blue);
                            Pathfinder.FindPath(master.logicalMap, selectedCell, cell);
                            hoveredCell = cell;
                        }
                    }
                }
            }
        }
         
    }


    public void GetDistance(float distance)
    {
       this.distance=(int)distance;
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
        controls.enabled = !value;
        if (value)
        {
            selectedCell = null;
        }
        else
        {
           // master.logicalMap.HideAllHighlights();
            master.logicalMap.HideAllLabels();
        }
    }
}
