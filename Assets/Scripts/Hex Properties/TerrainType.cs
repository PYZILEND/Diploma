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
                return Color.white;
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
}
