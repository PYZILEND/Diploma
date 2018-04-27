using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Static class used to handle pathfinding algorithms
/// </summary>
public static class Pathfinder
{


    /// <summary>
    /// Marks cells withing said range from specified cell.
    /// Ignores weight.
    /// </summary>
    /// <param name="dist">Maximum distance</param>
    /// <param name="map">Logical map</param>
    /// <param name="cell">Cell to find range for</param>
    public static void FindRange(int dist, LogicalMap map, LogicalMapCell cell)
    {
        ResetCells(map);
        List<LogicalMapCell> frontier = new List<LogicalMapCell>();
        List<LogicalMapCell> usedCell = new List<LogicalMapCell>();
        frontier.Add(cell);
        usedCell.Add(cell);
        while (frontier.Count > 0)
        {
            LogicalMapCell current = frontier[0];
            frontier.RemoveAt(0);
            for (HexDirection d = HexDirection.NE; d <= HexDirection.NW; d++)
            {
                LogicalMapCell neighbor = current.GetNeighbor(d);
                if (neighbor == null)
                {
                    continue;
                }
                int distance = DistanceTo(cell.coordinates, neighbor.coordinates);
                if ((distance <= dist)&&(!usedCell.Contains(neighbor)))
                {
                    if ((neighbor.unit != null) && (neighbor.unit.isDominion != cell.unit.isDominion))
                    {
                        neighbor.inShootingRange = true;
                        //neighbor.highlight.color = Color.black;
                    }
                    frontier.Add(neighbor);
                    usedCell.Add(neighbor);                    
                }
            }
        }
    }
    /// <summary>
    /// Finds unweighted distance between two cells
    /// </summary>
    /// <param name="cell"></param>
    /// <param name="other"></param>
    /// <returns></returns>
    public static int DistanceTo(HexCoordinates cell, HexCoordinates other)
    {
        return (((cell.X < other.X ? other.X - cell.X : cell.X - other.X) +
            (cell.Y < other.Y ? other.Y - cell.Y : cell.Y - other.Y) +
            (cell.Z < other.Z ? other.Z - cell.Z : cell.Z - other.Z)) / 2);
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="dist"></param>
    /// <param name="map"></param>
    /// <param name="cell"></param>
    public static void FindWeightedDistance(int dist, LogicalMap map, LogicalMapCell cell)
    {
        SearchWeightedDistance(dist, cell);
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="dist"></param>
    /// <param name="map"></param>
    /// <param name="cell"></param>
    static void SearchWeightedDistance(int dist, LogicalMapCell cell)
    {
        List<LogicalMapCell> frontier = new List<LogicalMapCell>();
        cell.distance = 0;
        //cell.EnableLabel(cell.distance.ToString());
        frontier.Add(cell);

        while (frontier.Count > 0)
        {
            LogicalMapCell current = frontier[0];
            frontier.RemoveAt(0);
            for (HexDirection d = HexDirection.NE; d <= HexDirection.NW; d++)
            {
                {
                    LogicalMapCell neighbor = current.GetNeighbor(d);
                    if (neighbor == null)
                    {
                        continue;
                    }
                    if (neighbor.terrain == TerrainType.Ocean)
                    {
                        continue;
                    }
                    if (neighbor.terrain == TerrainType.Impassable)
                    {
                        continue;
                    }

                    int distance = current.distance;
                    if (neighbor.terrain == TerrainType.Road)
                    {
                        distance += 1;
                    }
                    else {

                        if (neighbor.terrain == TerrainType.Forest || neighbor.terrain == TerrainType.Sand || neighbor.terrain == TerrainType.River)
                        {
                            distance += 4;
                        }
                        else {
                            distance += 2;
                        }
                    }
                    if (neighbor.distance == int.MaxValue)
                    {
                        neighbor.distance = distance;
                        if (distance <= dist)
                        {
                            //neighbor.EnableLabel(distance.ToString());
                            neighbor.isReachable = true;
                            neighbor.pathFrom = current;
                            frontier.Add(neighbor);
                        }
                    }
                    else
                    if (distance < neighbor.distance)
                    {
                        if (distance <= dist)
                        {
                            neighbor.isReachable = true;
                            neighbor.pathFrom = current;
                        }
                        else
                        {
                            neighbor.isReachable = false;
                        }
                        neighbor.distance = distance;
                    }

                    frontier.Sort((x, y) => x.distance.CompareTo(y.distance));
                }
            }
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="map"></param>
    /// <param name="cell"></param>
    /// <param name="toCell"></param>
    public static List<LogicalMapCell> FindPath(LogicalMap map, LogicalMapCell cell, LogicalMapCell toCell)
    {
        return SearchPath(cell, toCell);
    }

    /// <summary>
    /// Finds shortest path from one cell to another.
    /// 
    /// </summary>
    /// <param name="fromCell"></param>
    /// <param name="toCell"></param>
    static List<LogicalMapCell> SearchPath(LogicalMapCell fromCell, LogicalMapCell toCell)
    {
        List<LogicalMapCell> path = new List<LogicalMapCell>();
        LogicalMapCell current = toCell;
        while (current != fromCell)
        {
            path.Insert(0, current);
            current.EnableHighlight(Color.magenta);
            current = current.pathFrom;
        }
        return path;
    }

    /// <summary>
    /// Resets every cell's distance and shooting range
    /// </summary>
    /// <param name="map"></param>
    static void ResetCells(LogicalMap map)
    {
        for (int i = 0; i < map.cells.Length; i++)
        {
            map.cells[i].inShootingRange = false;
            map.cells[i].isReachable = false;
            map.cells[i].distance = int.MaxValue;
        }
    }
}
