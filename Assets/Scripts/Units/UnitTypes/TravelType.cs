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
    /*
    public static TravelType UnitTypeToTravelType(UnitTypeOld type)
    {
        if(type == UnitTypeOld.LBM ||
            type == UnitTypeOld.Tank ||
            type == UnitTypeOld.Mortair ||
            type == UnitTypeOld.WarBot ||
            type == UnitTypeOld.Artillery ||
            type == UnitTypeOld.Transport ||
            type == UnitTypeOld.Bullfrog ||
            type == UnitTypeOld.Bunker ||
            type == UnitTypeOld.Tower ||
            type == UnitTypeOld.AntiAir)
        {
            return TravelType.Land;
        }

        if(type == UnitTypeOld.Destroyer ||
            type == UnitTypeOld.Submarine ||
            type == UnitTypeOld.Battleship ||
            type == UnitTypeOld.NavalTransport ||
            type == UnitTypeOld.Carrier ||
            type == UnitTypeOld.Leviathan)
        {
            return TravelType.Naval;
        }

        if(type == UnitTypeOld.Helicopter ||
            type == UnitTypeOld.TransportHelicopter ||
            type == UnitTypeOld.Plane)
        {
            return TravelType.Air;
        }

        if(type == UnitTypeOld.Amphibia ||
            type == UnitTypeOld.Platform)
        {
            return TravelType.Amphibious;
        }

        return TravelType.Land;
    }*/

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
