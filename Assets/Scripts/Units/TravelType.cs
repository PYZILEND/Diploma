using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Possible unit travel types
/// </summary>
public enum TravelType { Land, Naval, Air, Amphibious}

public static class TravelTypeExtentions
{
    /// <summary>
    /// Converts unit type to it's travel type
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public static TravelType UnitTypeToTravelType(UnitType type)
    {
        if(type == UnitType.LBM ||
            type == UnitType.Tank ||
            type == UnitType.Mortair ||
            type == UnitType.WarBot ||
            type == UnitType.Artillery ||
            type == UnitType.Transport ||
            type == UnitType.Bullfrog ||
            type == UnitType.Bunker ||
            type == UnitType.Tower ||
            type == UnitType.AntiAir)
        {
            return TravelType.Land;
        }

        if(type == UnitType.Destroyer ||
            type == UnitType.Submarine ||
            type == UnitType.Battleship ||
            type == UnitType.NavalTransport ||
            type == UnitType.Carrier ||
            type == UnitType.Leviathan)
        {
            return TravelType.Naval;
        }

        if(type == UnitType.Helicopter ||
            type == UnitType.TransportHelicopter ||
            type == UnitType.Plane)
        {
            return TravelType.Air;
        }

        if(type == UnitType.Amphibia ||
            type == UnitType.Platform)
        {
            return TravelType.Amphibious;
        }

        return TravelType.Land;
    }

    /// <summary>
    /// Returns true if unit with said travel type can travel on said terrain
    /// </summary>
    /// <param name="terrain"></param>
    /// <param name="type"></param>
    /// <returns></returns>
    public static bool CanTravelOn(TerrainType terrain, TravelType type)
    {
        if(type == TravelType.Air)
        {
            return true;
        }

        if(type == TravelType.Naval && terrain == TerrainType.Ocean)
        {
            return true;
        }

        if(type == TravelType.Amphibious && terrain != TerrainType.Impassable)
        {
            return true;
        }

        if(type == TravelType.Land && terrain != TerrainType.Impassable && terrain != TerrainType.Ocean)
        {
            return true;
        }

        return false;
    }
}
