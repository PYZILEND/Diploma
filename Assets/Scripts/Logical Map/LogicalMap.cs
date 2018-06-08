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

    int width;
    int height;

    /// <summary>
    /// Initializes logical map fields
    /// </summary>
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
        //Assigning map parameters
        this.width = width;
        this.height = height;

        //Initializing array
        cells = new LogicalMapCell[width * height];

        //Creating cells
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

        //Initializing cell's label
        Text label = Instantiate(cellLabelPrefab);
        label.rectTransform.SetParent(mapCanvas.transform, false);
        label.rectTransform.anchoredPosition = new Vector2(position.x, position.z);

        //Initializing cell's highlight
        Image image = Instantiate(highlightPrefab);
        image.rectTransform.SetParent(mapCanvas.transform, false);
        image.rectTransform.anchoredPosition = new Vector2(position.x, position.z);

        //Initializing cell
        LogicalMapCell cell = cells[i] = Instantiate(cellPrefab);
        cell.transform.SetParent(this.transform, false);
        cell.transform.localPosition = position;
        cell.Initialize(image, label, HexCoordinates.fromOffsetCoordinates(x, z), TerrainType.Plain);

        //Connecting cell to its neighbors
        ConnectWithNeighbours(cell, x, z, i);
    }

    /// <summary>
    /// Connects cell with it's neighbour cells using it's offset coordinates and index
    /// </summary>
    /// <param name="cell">Cell to connect with neighbours</param>
    /// <param name="x">Cell's X offset coordinate</param>
    /// <param name="z">Cell's Z offset coordinate</param>
    /// <param name="i">Cell's index in cells array</param>
    public void ConnectWithNeighbours(LogicalMapCell cell, int x, int z, int i)
    {
        //Connecting cell's left neighbour
        if (x > 0)
        {
            cell.SetNeighbor(HexDirection.W, cells[i - 1]);
        }
        //If not lowest row
        if (z > 0)
        {
            //If even row
            if ((z & 1) == 0)
            {
                cell.SetNeighbor(HexDirection.SE, cells[i - width]);
                if (x > 0)
                {
                    cell.SetNeighbor(HexDirection.SW, cells[i - width - 1]);
                }
            }
            //If odd row
            else
            {
                cell.SetNeighbor(HexDirection.SW, cells[i - width]);
                if (x < width - 1)
                {
                    cell.SetNeighbor(HexDirection.SE, cells[i - width + 1]);
                }
            }
        }
    }

    /// <summary>
    /// Shows distances of cells in shooting range
    /// </summary>
    public void ShowDistancesInShootingRange()
    {
        for (int i = 0; i < cells.Length; i++)
        {
            if (cells[i].inShootingRange)
            {
                cells[i].EnableLabel(cells[i].distance.ToString());
            }
        }
    }

    /// <summary>
    /// Shows distances of all cells
    /// </summary>
    public void ShowAllDistances()
    {
        for (int i = 0; i < cells.Length; i++)
        {
            cells[i].EnableLabel(cells[i].distance.ToString());
        }
    }


    /// <summary>
    /// Highlights cells in shooting range
    /// </summary>
    public void HighlightShootingRange()
    {
        for (int i = 0; i < cells.Length; i++)
        {
            if (cells[i].inShootingRange) { cells[i].EnableHighlight(); }
        }
    }    

    /// <summary>
    /// Hides every cell's highlight
    /// </summary>
    public void HideAllHighlights()
    {
        for(int i=0; i < cells.Length; i++)
        {
            cells[i].DisableHighlight();
        }
    }

    /// <summary>
    /// Shows every cell's highlight
    /// </summary>
    public void HighlightTerrain()
    {
        for (int i = 0; i < cells.Length; i++)
        {
            cells[i].EnableHighlight();
            cells[i].ValidateHighlightWithTerrain();
        }
    }

    /// <summary>
    /// Hides every cell's label
    /// </summary>
    public void HideAllLabels()
    {
        for (int i = 0; i < cells.Length; i++)
        {
            cells[i].DisableLabel();
        }
    }

    /// <summary>
    /// Shows every cell's coordinates
    /// </summary>
    public void ShowAllCoordinates()
    {
        for(int i = 0; i<cells.Length; i++)
        {
            cells[i].EnableLabel(cells[i].coordinates.ToString());
        }
    }

    public void HighlightReachableRange()
    {
        for (int i = 0; i < cells.Length; i++)
        {
            if (cells[i].isReachable) { cells[i].HighlightValidatedTerrain(); }
        }
    }

    /// <summary>
    /// Highlights cells based on their allegiance
    /// </summary>
    public void HighlightAllegiance()
    {
        for (int i = 0; i < cells.Length; i++)
        {
            cells[i].EnableHighlight();
            cells[i].ValidateHighlightWithAllegiance();
        }
    }

    public void AddForest()
    {
        foreach (LogicalMapCell cell in cells)
        {
            if (cell.terrain != TerrainType.Forest) continue;
            cell.AddForest();
        }
    }
}
