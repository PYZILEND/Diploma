using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Logical map handles logical aspects of the map
///such as countryes, units, terrain types etc.
/// </summary>
public class LogicalMap : MonoBehaviour {

    public LogicalMapCell cellPrefab;
    public LogicalMapCell[] cells;

    public Text cellLabelPrefab;
    public Image highlightPrefab;
    Canvas mapCanvas;

    void Awake()
    {
        mapCanvas = GetComponentInChildren<Canvas>();
    }

    /// <summary>
    /// Creates logical map of specified width and height
    /// </summary>
    /// <param name="width">Width of the map in hexagons</param>
    /// <param name="height">Height of the map in hexagons</param>
    public void CreateMap(int width, int height)
    {
        //Initializing array
        cells = new LogicalMapCell[width * height];

        int i = 0;
        for(int z = 0; z < height; z++)
        {
            for(int x = 0; x < width; x++)
            {
                CreateCell(x, z, i);
                i++;
            }
        }
    }

    /// <summary>
    /// Creates a logical cell with said offset coordinates and index in cells array
    /// </summary>
    /// <param name="x">X offset coordinate</param>
    /// <param name="z">Z offset coordinate</param>
    /// <param name="i">Index in array</param>
    void CreateCell(int x, int z, int i)
    {
        //Calculating cell's center position
        Vector3 position;
        position.x = (x + z * 0.5f - z / 2) * (2f * HexMetrics.innerRadius);
        position.y = 100f;
        position.z = z * (1.5f * HexMetrics.outerRadius);

        //Initializing cell
        LogicalMapCell cell = cells[i] = Instantiate(cellPrefab);
        cell.transform.SetParent(this.transform, false);
        cell.transform.localPosition = position;
        cell.coordinates = HexCoordinates.fromOffsetCoordinates(x, z);
        cell.terrain = TerrainType.Plain;

        //Initializing cell's label
        Text label = Instantiate(cellLabelPrefab);
        label.rectTransform.SetParent(mapCanvas.transform, false);
        label.rectTransform.anchoredPosition = new Vector2(position.x, position.z);
        label.text = cell.coordinates.X + "\n" + cell.coordinates.Y + "\n" + cell.coordinates.Z;

        //Initializing cell's highlight
        Image image = Instantiate(highlightPrefab);
        image.rectTransform.SetParent(mapCanvas.transform, false);
        image.rectTransform.anchoredPosition = new Vector2(position.x, position.z);

        //Connecting cell with it's UI elements        
        cell.label = label;
        cell.highlight = image;
        cell.ValidateHighlightWithTerrain();
        cell.CalculateUIPosition();
        cell.DisableHighlight();
        cell.DisableLabel();
    }

    public void HideAllHighlights()
    {
        for(int i=0; i < cells.Length; i++)
        {
            cells[i].DisableHighlight();
        }
    }

    public void ShowAllHighlights()
    {
        for (int i = 0; i < cells.Length; i++)
        {
            cells[i].EnableHighlight();
            cells[i].ValidateHighlightWithTerrain();
        }
    }

    public void HideAllLabels()
    {
        for (int i = 0; i < cells.Length; i++)
        {
            cells[i].DisableLabel();
        }
    }

    public void ShowAllCoordinates()
    {
        for(int i = 0; i<cells.Length; i++)
        {
            cells[i].EnableLabel(cells[i].coordinates.ToString());
        }
    }
}
