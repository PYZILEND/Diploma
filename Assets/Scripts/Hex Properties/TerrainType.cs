using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Enumerator for terrain types
/// </summary>
public enum TerrainType
{
    Plain, Forest, Sand, Swamp, River, Ocean, Impassable
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
                return Color.black;
            case TerrainType.Forest:
                return Color.green;
            case TerrainType.Sand:
                return Color.yellow;
            case TerrainType.Swamp:
                return Color.red;
            case TerrainType.River:
                return Color.grey;
            case TerrainType.Ocean:
                return Color.blue;
            case TerrainType.Impassable:
                return Color.magenta;
        }
        return Color.white;
    }
}
