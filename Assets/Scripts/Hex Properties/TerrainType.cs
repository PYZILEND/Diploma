using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Enumerator for terrain types
/// </summary>
public enum TerrainType
{
    Plain, Road, Forest, Sand, River, Ocean, Impassable
}

/// <summary>
/// Extends TerrainType enumerator with support methods
/// </summary>
public static class TerrainTypeExtentions
{
    /// <summary>
    /// Returns given terrain type's cell highlight color
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public static Color TypeToColor(TerrainType type)
    {
        switch (type)
        {
            case TerrainType.Plain:
                return Color.yellow;
            case TerrainType.Road:
                return Color.grey;
            case TerrainType.Forest:
                return Color.green;
            case TerrainType.Sand:
                return Color.red;
            case TerrainType.River:
                return Color.cyan;
            case TerrainType.Ocean:
                return Color.blue;
            case TerrainType.Impassable:
                return Color.magenta;
        }
        return Color.black;
    }

    public static Color TypeToTileColor(TerrainType type)
    {
        switch (type)
        {
            case TerrainType.Plain:
                return new Color32(114, 195, 67, 255);
            case TerrainType.Road:
                return new Color32(122, 137, 132, 255);
            case TerrainType.Forest:
                return new Color32(159, 189, 92, 255);
            case TerrainType.Sand:
                return new Color32(251, 196, 140, 255);
            case TerrainType.River:
                return new Color32(103, 179, 151, 255);
            case TerrainType.Ocean:
                return new Color32(95, 145, 216, 255);
            case TerrainType.Impassable:
                return new Color32(65, 92, 101, 255);
        }
        return Color.black;
    }
}
