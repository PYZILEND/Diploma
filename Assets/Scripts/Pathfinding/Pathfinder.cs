﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Static class used to handle pathfinding algorithms
/// </summary>
public static class Pathfinder  {
    

    /// <summary>
    /// Marks cells withing said range from specified cell.
    /// Ignores weight.
    /// </summary>
    /// <param name="dist">Maximum distance</param>
    /// <param name="map">Logical map</param>
    /// <param name="cell">Cell to find range for</param>
    public static void FindRange(int dist, LogicalMap map, LogicalMapCell cell)
    {
        EraseShootingRange(map);
        for (int i = 0; i < map.cells.Length; i++)
        {
            int distance = DistanceTo(cell.coordinates, map.cells[i].coordinates);
            if (distance <= dist) {
                map.cells[i].inShootingRange = true;
            }
        }
    }
    /// <summary>
    /// Finds unweighted distance between two cells
    /// </summary>
    /// <param name="cell"></param>
    /// <param name="other"></param>
    /// <returns></returns>
    public static int DistanceTo(HexCoordinates cell,HexCoordinates other)
    {
        return (((cell.X < other.X ? other.X - cell.X : cell.X - other.X) +
            (cell.Y < other.Y ? other.Y - cell.Y : cell.Y - other.Y) +
            (cell.Z < other.Z ? other.Z - cell.Z : cell.Z - other.Z))/2);
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="dist"></param>
    /// <param name="map"></param>
    /// <param name="cell"></param>
    public static void FindWeightedDistance(int dist, LogicalMap map, LogicalMapCell cell)
    {
        ResetCellsDistance(map);
        SearchWeightedDistance(dist,cell);
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="dist"></param>
    /// <param name="map"></param>
    /// <param name="cell"></param>
    static void SearchWeightedDistance(int dist,  LogicalMapCell cell)
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
                    if (neighbor.terrain== TerrainType.Ocean)
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
                            frontier.Add(neighbor);
                        }
                    }
                    else 
                    if (distance < neighbor.distance)
                    {
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
    public static void FindPath(LogicalMap map, LogicalMapCell cell, LogicalMapCell toCell)
    {
        ResetCellsDistance(map);
        SearchPath(cell, toCell);
    }

    /// <summary>
    /// Finds shortest path from one cell to another.
    /// 
    /// </summary>
    /// <param name="fromCell"></param>
    /// <param name="toCell"></param>
    static void SearchPath(LogicalMapCell fromCell, LogicalMapCell toCell)
    {
        List<LogicalMapCell> frontier = new List<LogicalMapCell>();
        fromCell.distance = 0;
        //fromCell.EnableHighlight(Color.blue);
        frontier.Add(fromCell);

        while (frontier.Count > 0)
        {
            LogicalMapCell current = frontier[0];
            frontier.RemoveAt(0);

            if (current == toCell)
            {
                current = current.pathFrom;
                while (current != fromCell)
                {
                    current.EnableHighlight(Color.magenta);
                    current = current.pathFrom;
                }
                break;
            }

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
                        neighbor.pathFrom = current;
                        //neighbor.EnableLabel(distance.ToString());
                        frontier.Add(neighbor);                        
                    }
                    else
                    if (distance < neighbor.distance)
                    {
                        neighbor.distance = distance;
                        neighbor.pathFrom = current;
                    }

                    frontier.Sort((x, y) => x.distance.CompareTo(y.distance));
                }
            }
        }
    }

    /// <summary>
    /// Resets every cell's distance
    /// </summary>
    /// <param name="map"></param>
    static void ResetCellsDistance(LogicalMap map)
    {
        for (int i = 0; i < map.cells.Length; i++)
        {
            map.cells[i].distance = int.MaxValue;
        }
    }

    /// <summary>
    /// Removes all cells from shooting range
    /// </summary>
    public static void EraseShootingRange(LogicalMap map)
    {
        LogicalMapCell[] cells = map.cells;
        for (int i = 0; i < cells.Length; i++)
        {
            cells[i].inShootingRange = false;
        }
    }
}
