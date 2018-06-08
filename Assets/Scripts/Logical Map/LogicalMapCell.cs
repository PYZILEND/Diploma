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
    public LogicalMapCell[] neighbors;

    //Cell's pathfinding related fields
    public bool inShootingRange;
    public bool isReachable;
    public int distance;
    public LogicalMapCell pathFrom;

    /// <summary>
    /// Weather cell's country can be selected
    /// </summary>
    public bool isSelectable;
    /// <summary>
    /// Weather a purchased unit can be placed into that cell
    /// </summary>
    public bool canPlaceUnit;

    /// <summary>
    /// Weather cell is protected by anti-air
    /// </summary>
    public bool isProtected
    {
        get
        {

            if (unit &&
                unit is AntiAir &&
                !unit.isEmbarked &&
                !unit.isDestroyed &&
                unit.allegiance != GameMaster.allegianceTurn)
            {
                return true;
            }
            
            for (HexDirection d = HexDirection.NE; d <= HexDirection.NW; d++)
            {
                if (neighbors[(int) d])
                {
                    if (neighbors[(int)d].unit &&
                        neighbors[(int)d].unit is AntiAir &&
                        !neighbors[(int)d].unit.isEmbarked &&
                        !neighbors[(int)d].unit.isDestroyed &&
                        neighbors[(int)d].unit.allegiance != GameMaster.allegianceTurn)
                    {
                        return true;
                    }
                }
            }
            return false;
        }
    }

    public Unit unit;
    public Country country;

    public bool isCapital
    {
        get
        {
            if(country && country.capital == this)
            {
                return true;
            }
            return false;
        }
    }

    public Capital capital
    {
        get
        {
            if (isCapital)
            {
                return country.capitalCity;
            }
            return null;
        }
    }

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

    public Vector3 GetUIPosition()
    {
        RaycastHit hit;
        Physics.Raycast(transform.position, new Vector3(0f, -1f, 0f), out hit);

        Vector3 position = hit.point;
        position.y += 0.5f;

        return position;
    }

    public Vector3 GetRelativePhysicalPosition()
    {
        RaycastHit hit;
        Physics.Raycast(transform.position, new Vector3(0f, -1f, 0f), out hit);

        Vector3 position = hit.point;
        position.y += 0.5f;

        return position;
    }

    public Vector3 GetRelativePhysicalPosition(Vector3 logicalPosition)
    {
        RaycastHit hit;
        Physics.Raycast(logicalPosition, new Vector3(0f, -1f, 0f), out hit);

        Vector3 position = hit.point;
        position.y += 0.5f;

        return position;
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

    /// <summary>
    /// Aligns cell's highlight color with it's terrain type.
    /// </summary>
    public void ValidateHighlightWithTerrain()
    {
        highlight.color = TerrainTypeExtentions.TypeToColor(terrain);
    }

    public void ValidateHighlightWithAllegiance()
    {
        if (country)
        {
            highlight.color = AllegianceExtentions.AllegianceToColor(country.allegiance);
        }
        else
        {
            highlight.color = Color.white;
        }
    }

    public void AddForest()
    {
        // if (terrain != TerrainType.Forest) return;
        int[] forestation = new int[] { -1, -1, -1, -1, -1, -1 };
        int j = -1;
        int ind = coordinates.ToIndex();
        for (int i = 0; i < 6; i++)
        {
            if (PropertiesKeeper.logicalMap.cells[ind].neighbors[i] != null && PropertiesKeeper.logicalMap.cells[ind].neighbors[i].terrain != TerrainType.Forest
                && PropertiesKeeper.logicalMap.cells[ind].neighbors[i].terrain != TerrainType.Impassable
                && PropertiesKeeper.logicalMap.cells[ind].neighbors[i].terrain != TerrainType.Ocean)
            {
                j = Random.Range(1, 4);
                break;
            }
        }
        if (j == -1)
        {
            j = Random.Range(4, 7);
        }
        do
        {
            int k = Random.Range(0, 6);
            if (forestation[k] == -1)
            {
                forestation[k] = 1;
                j--;
            }
        } while (j != 0);
        for (int i = 0; i < forestation.Length; i++)
        {
            if (forestation[i] == -1) continue;
            AddTree(((HexMetrics.corners[i] + HexMetrics.corners[i + 1] + CreateHexMap.OffsetVector((int)coordinates.ToOffset().x, (int)coordinates.ToOffset().y) * 3) / 3),
                (float)ind / PropertiesKeeper.logicalMap.cells.Length, (float)i / forestation.Length);
        }
    }

    void AddTree(Vector3 offset, float x, float y)
    {
        if (PropertiesKeeper.physicalMap.transform.Find("forestContainer") == null)
        {
            GameObject forestContainer = new GameObject("forestContainer");
            forestContainer.transform.SetParent(PropertiesKeeper.physicalMap.transform);
        }
        Transform instance = Instantiate(PropertiesKeeper.treePrefab[Random.Range(0, PropertiesKeeper.treePrefab.Length)]);
        instance.localPosition = offset;
        instance.SetParent(PropertiesKeeper.physicalMap.transform.Find("forestContainer"), false);
        instance.localRotation = Quaternion.Euler(0f, 360f * Mathf.PerlinNoise(x, y), 0f);
    }

    /*void DestroyForest()
    {
        if (terrain != TerrainType.Forest) return;
        for (int i = 0; i < PropertiesKeeper.treePrefab.Length; i++)
        {
            while (transform.Find("tree" + i + "(Clone)") != null)
            {
                Destroy(transform.Find("tree" + i + "(Clone)"));
            }
        }
    }*/
}
