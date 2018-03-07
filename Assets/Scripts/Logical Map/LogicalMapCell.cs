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

    public Image highlight;
    public Text label;

    public HexCoordinates coordinates;     
    public TerrainType terrain;

    //array for storing 6 or less neighbors of cell
    [SerializeField]
    LogicalMapCell[] neighbors;

    public bool inShootingRange;
    public bool isReachable;
    public int distance;

    public LogicalMapCell PathFrom { get; set; }
    //Following 6 methods seem redundant, but they might end up getting more complex,
    //thus we're going to keep them for now


    public void EnableHighlight()
    {
        highlight.enabled = true;
    }

    public void EnableHighlight(Color color)
    {
        highlight.color = color;
        highlight.enabled = true;
    }

    public void DisableHighlight()
    {
        highlight.enabled = false;
    }

    public void EnableLabel()
    {
        label.enabled = true;
    }

    public void EnableLabel(string text)
    {
        label.text = text;
        label.enabled = true;
    }

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

    public void InitializeArray()
    {
        neighbors = new LogicalMapCell[6];
    }

    /// <summary>
    /// Aligns this cell's UI elements to it's
    /// relative physical map location
    /// </summary>
    public void CalculateUIPosition()
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
    /// Returns its neighbor for any given direction
    /// </summary>
    /// <param name="direction">direction for retrieving neighbor</param>
    /// <returns>neighbor of the cell</returns>
    public LogicalMapCell GetNeighbor(HexDirection direction)
    {
        return neighbors[(int)direction];
    }

    /// <summary>
    /// Set cell's neighbor and neighbor's neighbor as current cell
    /// </summary>
    /// <param name="direction">direction of neighbor</param>
    /// <param name="cell">neighbor cell</param>
    public void SetNeighbor(HexDirection direction, LogicalMapCell cell)
    {
        neighbors[(int)direction] = cell;
        cell.neighbors[(int)direction.Opposite()] = this;
    }
}
