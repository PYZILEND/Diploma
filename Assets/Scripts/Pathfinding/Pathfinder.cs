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
    /// <param name="cell">Cell to find range for</param>
    public static void MarkShootingRange(int dist, LogicalMapCell cell)
    {
        ResetCells();
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
                    if ((neighbor.unit != null) && (neighbor.unit.allegiance != cell.unit.allegiance))
                    {
                        neighbor.inShootingRange = true;
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
    /// Searches possible paths for a said unit, withing said distance
    /// </summary>
    /// <param name="dist"></param>
    /// <param name="cell"></param>
    /// <param name="type"></param>
    public static void SearchPossiblePaths(int maxDistance, LogicalMapCell cell, UnitType type)
    {
        List<LogicalMapCell> frontier = new List<LogicalMapCell>();
        cell.distance = 0;
        frontier.Add(cell);

        while (frontier.Count > 0)
        {
            LogicalMapCell current = frontier[0];
            frontier.RemoveAt(0);
            for (HexDirection d = HexDirection.NE; d <= HexDirection.NW; d++)
            {
                LogicalMapCell neighbor = current.GetNeighbor(d);

                int distance = CountDistance(neighbor, type);
                if(distance == -1)
                {
                    continue;
                }
                distance += current.distance;

                if (neighbor.distance == int.MaxValue)
                {
                    neighbor.distance = distance;
                    if (distance <= maxDistance)
                    {
                        if (!wentOverUnit)
                        {
                            neighbor.isReachable = true;
                        }
                        neighbor.pathFrom = current;
                        if (!canBoardHere)
                        {
                            frontier.Add(neighbor);
                        }
                    }
                }
                else
                if (distance < neighbor.distance)
                {
                    if (distance <= maxDistance)
                    {
                        if (!wentOverUnit)
                        {
                            neighbor.isReachable = true;
                        }
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

    //Used to store weather unit will be getting over other unit
    //or boarding
    static bool wentOverUnit;
    static bool canBoardHere;

    static int CountDistance(LogicalMapCell neighbour, UnitType type)
    {
        wentOverUnit = false;
        canBoardHere = false;

        if (neighbour == null)
        {
            return -1;
        }

        if (neighbour.unit)
        {
            if (type.canGoOverUnits)
            {
                wentOverUnit = true;
            }
            else if(type.canEmbark)
            {
                if(neighbour.unit.allegiance == GameMaster.allegianceTurn)
                {
                    if(neighbour.unit is Transport || neighbour.unit is Platform)
                    {
                        canBoardHere = true;
                    }
                    else
                    {
                        return -1;
                    }
                }
                else
                {
                    return -1;
                }
            }
            else
            {
                return -1;
            }
        }

        switch (type.travelType)
        {
            case TravelType.Land:
                return CountDistanceLand(neighbour, type);

            case TravelType.Naval:
                return CountDistanceNaval(neighbour, type);

            case TravelType.Amphibious:
                return CountDistanceAmphibious(neighbour, type);

            case TravelType.Air:
                return CountDistanceAir(neighbour, type);
        }

        return -1;            
    }

    static int CountDistanceLand(LogicalMapCell neighbour, UnitType type)
    {
        if(neighbour.terrain == TerrainType.Impassable ||
            neighbour.terrain == TerrainType.Ocean)
        {
            return -1;
        }
        if (type.maxMovePoints > 10)
        {
            if(neighbour.terrain == TerrainType.Plain || neighbour.terrain == TerrainType.Road)
            {
                return 10;
            }
            else
            {
                return 20;
            }
        }
        else
        {
            if(neighbour.terrain == TerrainType.Road)
            {
                return 5;
            }
            else
            {
                return 10;
            }
        }
    }

    static int CountDistanceNaval(LogicalMapCell neighbour, UnitType type)
    {
        if(neighbour.terrain != TerrainType.Ocean)
        {
            return -1;
        }
        return 10;
    }

    static int CountDistanceAmphibious(LogicalMapCell neighbour, UnitType type)
    {
        if(neighbour.terrain != TerrainType.Impassable)
        {
            return 10;
        }
        return -1;
    }

    static int CountDistanceAir(LogicalMapCell neighbour, UnitType type)
    {
        return 10;
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
    /// Finds shortest path from one cell to another.    /// 
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
    static void ResetCells()
    {
        for (int i = 0; i < GameMaster.logicalMap.cells.Length; i++)
        {
            GameMaster.logicalMap.cells[i].inShootingRange = false;
            GameMaster.logicalMap.cells[i].isReachable = false;
            GameMaster.logicalMap.cells[i].distance = int.MaxValue;
        }
    }
}
