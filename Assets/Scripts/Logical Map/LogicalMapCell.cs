using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// This class represents logical cell of the map.
/// It is used to store things like country of the cell,
/// it's unit (if any), it's terrain type, etc.
/// </summary>
public class LogicalMapCell : MonoBehaviour {

    //Cell's UI elements
    public Image highlight;
    public Text label;

    public HexCoordinates coordinates;     
    public TerrainType terrain;

    //Array for storing 6 or less neighbors of cell
    [SerializeField]
    LogicalMapCell[] neighbors;

    //Cell's pathfinding related fields
    public bool inShootingRange;
    public bool isReachable;
    public int distance;
    public LogicalMapCell pathFrom;

    public Unit unit;

    /// <summary>
    /// Initializes cell.
    /// Set's cells highlight, label, coordinates and terrain type.
    /// Also matches highlight's color with cell's terrain,
    /// validates cell's UI elements position and disables them
    /// </summary>
    /// <param name="highlight"></param>
    /// <param name="label"></param>
    /// <param name="coordinates"></param>
    /// <param name="terrain"></param>
    public void Initialize(Image highlight, Text label, HexCoordinates coordinates, TerrainType terrain)
    {
        this.highlight = highlight;
        this.label = label;
        this.coordinates = coordinates;
        this.terrain = terrain;

        ValidateHighlightWithTerrain();
        ValidateUIPosition();
        DisableHighlight();
        DisableLabel();
    }

    /// <summary>
    /// Enables cell's highlight
    /// </summary>
    public void EnableHighlight()
    {
        highlight.enabled = true;
    }

    /// <summary>
    /// Enables cell's highlihght and sets it's color
    /// </summary>
    /// <param name="color"></param>
    public void EnableHighlight(Color color)
    {
        highlight.color = color;
        highlight.enabled = true;
    }

    /// <summary>
    /// Disables cell's highlight
    /// </summary>
    public void DisableHighlight()
    {
        highlight.enabled = false;
    }

    /// <summary>
    /// Enables cell's label
    /// </summary>
    public void EnableLabel()
    {
        label.enabled = true;
    }

    /// <summary>
    /// Enables cell's label and sets it's text
    /// </summary>
    /// <param name="text"></param>
    public void EnableLabel(string text)
    {
        label.text = text;
        label.enabled = true;
    }

    /// <summary>
    /// Disables cell's label
    /// </summary>
    public void DisableLabel()
    {
        label.enabled = false;
    }

    /// <summary>
    /// Aligns cell's highlight color with it's terrain type.
    /// </summary>
    public void ValidateHighlightWithTerrain()
    {
        highlight.color = TerrainTypeExtentions.TypeToColor(terrain);
    }

    /// <summary>
    /// Aligns this cell's UI elements to it's
    /// relative physical map location
    /// </summary>
    public void ValidateUIPosition()
    {
        RaycastHit hit;
        Physics.Raycast(transform.position, new Vector3(0f, -1f, 0f), out hit);

        Vector3 position = hit.point;
        position.y += 0.5f;

        label.transform.position = position;
        label.transform.localRotation = Quaternion.Euler(hit.normal);

        highlight.transform.position = position;
        highlight.transform.localRotation = Quaternion.Euler(hit.normal);
    }

    /// <summary>
    /// Returns cell's neighbor for any given direction
    /// </summary>
    /// <param name="direction">Direction for retrieving neighbor</param>
    public LogicalMapCell GetNeighbor(HexDirection direction)
    {
        return neighbors[(int)direction];
    }

    /// <summary>
    /// Sets cell's neighbour in given direction.
    /// Also sets this cell as neighbour's neighbour
    /// in opposite direction
    /// </summary>
    /// <param name="direction">Direction in which to set neighbour</param>
    /// <param name="cell">Cell to be set as neighbour</param>
    public void SetNeighbor(HexDirection direction, LogicalMapCell cell)
    {
        neighbors[(int)direction] = cell;
        cell.neighbors[(int)direction.Opposite()] = this;
    }

    public void HighlightValidatedTerrain()
    {
        highlight.color = TerrainTypeExtentions.TypeToColor(terrain);
        highlight.enabled = true;
    }
}
