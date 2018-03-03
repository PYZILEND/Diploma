using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This class handles input in map editing mode
/// </summary>
public class MapEditorControls : MonoBehaviour {

    GameMaster master;//Needed to reference logical map

    //Needed to refrernce itself 
    //for switching between editor and game mode
    MapEditorControls controls;

    //Stores values to adjust
    int selectedTerrainType;
    bool applyTerrainType;

    /// <summary>
    /// Initialization
    /// </summary>
	void Awake () {
        controls = this.GetComponent<MapEditorControls>();
        controls.enabled = false;
        master = GetComponentInParent<GameMaster>();
	}

    /// <summary>
    /// Scans for input
    /// </summary>
	void Update () {
        if (Input.GetMouseButton(0))
        {
            LogicalMapCell cell;
            if (cell = GetCellUnderCoursor())
            {
                if (applyTerrainType)
                {
                    cell.terrain = (TerrainType)selectedTerrainType;
                    cell.ValidateHighlightWithTerrain();
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
        if (Physics.Raycast(ray, out hit))
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
        controls.enabled = value;
        if (value)
        {
            master.logicalMap.ShowAllHighlights();
            master.logicalMap.ShowAllCoordinates();
        }
    }

    /// <summary>
    /// Takes terrain type from UI slider
    /// </summary>
    /// <param name="type"></param>
    public void SetTerrainType(float type)
    {
        selectedTerrainType = (int)type;
    }

    /// <summary>
    /// Takes weather terrain type should be applyed from UI checkbox
    /// </summary>
    /// <param name="value"></param>
    public void SetApplyTerrain(bool value)
    {
        applyTerrainType = value;
    }
}
