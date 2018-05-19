using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Possible unit types
/// </summary>
public enum UnitTypeOld
{
    LBM, Tank, Mortair, WarBot, Artillery, Transport, Bullfrog, Destroyer, Submarine, Battleship, NavalTransport, Carrier, Leviathan,
    Bunker, Tower, Amphibia, Platform, Helicopter, TransportHelicopter, Plane, AntiAir
}

public static class UnitTypeExtentions
{
    
    /// <summary>
    /// Creates unit of certain class depending on it's type
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public static Unit CreateUnitFromType(UnitTypeOld type)
    {
        if(type == UnitTypeOld.Transport || type==UnitTypeOld.NavalTransport || type==UnitTypeOld.TransportHelicopter)
        {
            return new Transport();
        }
        if(type == UnitTypeOld.Bullfrog || type == UnitTypeOld.Leviathan)
        {
            return new SuperUnit();
        }
        if (type == UnitTypeOld.Platform)
        {
            return new Platform();
        }
        if (type == UnitTypeOld.AntiAir)
        {
            return new AntiAir();
        }
        return new Unit();
    }

    /*
    public static string GetName(UnitType type, bool isCoalition)
    {
        if (isCoalition)
        {
            switch (type)
            {
                case UnitType.LBM: return "ЛБМ";
                case UnitType.Tank: return 5;
                case UnitType.Mortair: return 4;
                case UnitType.WarBot: return 7;
                case UnitType.Artillery: return 3;
                case UnitType.Transport: return 3;
                case UnitType.Bullfrog: return 11;
                case UnitType.Destroyer: return 5;
                case UnitType.Submarine: return 4;
                case UnitType.Battleship: return 8;
                case UnitType.NavalTransport: return 4;
                case UnitType.Carrier: return 7;
                case UnitType.Leviathan: return 10;
                case UnitType.Bunker: return 4;
                case UnitType.Tower: return 9;
                case UnitType.Amphibia: return 4;
                case UnitType.Platform: return 3;
                case UnitType.Helicopter: return 3;
                case UnitType.TransportHelicopter: return 3;
                case UnitType.Plane: return 1;
                case UnitType.AntiAir: return 4;
                default: return 0;
            }
        }
        else
        {
            switch (type)
            {
                case UnitType.LBM: return 4;
                case UnitType.Tank: return 5;
                case UnitType.Mortair: return 4;
                case UnitType.WarBot: return 7;
                case UnitType.Artillery: return 3;
                case UnitType.Transport: return 3;
                case UnitType.Bullfrog: return 11;
                case UnitType.Destroyer: return 5;
                case UnitType.Submarine: return 4;
                case UnitType.Battleship: return 8;
                case UnitType.NavalTransport: return 4;
                case UnitType.Carrier: return 7;
                case UnitType.Leviathan: return 10;
                case UnitType.Bunker: return 4;
                case UnitType.Tower: return 9;
                case UnitType.Amphibia: return 4;
                case UnitType.Platform: return 3;
                case UnitType.Helicopter: return 3;
                case UnitType.TransportHelicopter: return 3;
                case UnitType.Plane: return 1;
                case UnitType.AntiAir: return 4;
                default: return 0;
            }
        }
    }*/

    /// <summary>
    /// Returns max health for specified unit type
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public static int GetMaxHealth(UnitTypeOld type)
    {
        switch (type)
        {
            case UnitTypeOld.LBM: return 4;
            case UnitTypeOld.Tank: return 5;
            case UnitTypeOld.Mortair: return 4;
            case UnitTypeOld.WarBot: return 7;
            case UnitTypeOld.Artillery: return 3;
            case UnitTypeOld.Transport: return 3;
            case UnitTypeOld.Bullfrog: return 11;
            case UnitTypeOld.Destroyer: return 5;
            case UnitTypeOld.Submarine: return 4;
            case UnitTypeOld.Battleship: return 8;
            case UnitTypeOld.NavalTransport: return 4;
            case UnitTypeOld.Carrier: return 7;
            case UnitTypeOld.Leviathan: return 10;
            case UnitTypeOld.Bunker: return 4;
            case UnitTypeOld.Tower: return 9;
            case UnitTypeOld.Amphibia: return 4;
            case UnitTypeOld.Platform: return 3;
            case UnitTypeOld.Helicopter: return 3;
            case UnitTypeOld.TransportHelicopter: return 3;
            case UnitTypeOld.Plane: return 1;
            case UnitTypeOld.AntiAir: return 4;
            default: return 0;
        }
    }

    /// <summary>
    /// Returns max move points for specified unit type
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public static int GetMaxMovePoints(UnitTypeOld type)
    {
        switch (type)
        {
            case UnitTypeOld.LBM: return 1;
            case UnitTypeOld.Tank: return 2;
            case UnitTypeOld.Mortair: return 1;
            case UnitTypeOld.WarBot: return 2;
            case UnitTypeOld.Artillery: return 1;
            case UnitTypeOld.Transport: return 5;
            case UnitTypeOld.Bullfrog: return 2;
            case UnitTypeOld.Destroyer: return 3;
            case UnitTypeOld.Submarine: return 5;
            case UnitTypeOld.Battleship: return 4;
            case UnitTypeOld.NavalTransport: return 6;
            case UnitTypeOld.Carrier: return 3;
            case UnitTypeOld.Leviathan: return 4;
            case UnitTypeOld.Bunker: return 0;
            case UnitTypeOld.Tower: return 0;
            case UnitTypeOld.Amphibia: return 3;
            case UnitTypeOld.Platform: return 3;
            case UnitTypeOld.Helicopter: return 4;
            case UnitTypeOld.TransportHelicopter: return 4;
            case UnitTypeOld.Plane: return 12;
            case UnitTypeOld.AntiAir: return 1;
            default: return 0;
        }
    }

    /// <summary>
    /// Returns attack power for specified unit type
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public static int GetAttackPower(UnitTypeOld type)
    {
        switch (type)
        {
            case UnitTypeOld.LBM: return 1;
            case UnitTypeOld.Tank: return 2;
            case UnitTypeOld.Mortair: return 2;
            case UnitTypeOld.WarBot: return 3;
            case UnitTypeOld.Artillery: return 3;
            case UnitTypeOld.Transport: return 0;
            case UnitTypeOld.Bullfrog: return 5; //Need to think smtg out
            case UnitTypeOld.Destroyer: return 1;
            case UnitTypeOld.Submarine: return 2;
            case UnitTypeOld.Battleship: return 3;
            case UnitTypeOld.NavalTransport: return 0;
            case UnitTypeOld.Carrier: return 0;
            case UnitTypeOld.Leviathan: return 5; //Need to think smtg out
            case UnitTypeOld.Bunker: return 1;
            case UnitTypeOld.Tower: return 2;
            case UnitTypeOld.Amphibia: return 2;
            case UnitTypeOld.Platform: return 0;
            case UnitTypeOld.Helicopter: return 2;
            case UnitTypeOld.TransportHelicopter: return 0;
            case UnitTypeOld.Plane: return 1;
            case UnitTypeOld.AntiAir: return 0;
            default: return 0;
        }
    }

    /// <summary>
    /// Returns attack range for specified unit type
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public static int GetAttackRange(UnitTypeOld type)
    {
        switch (type)
        {
            case UnitTypeOld.LBM: return 1;
            case UnitTypeOld.Tank: return 1;
            case UnitTypeOld.Mortair: return 2;
            case UnitTypeOld.WarBot: return 2;
            case UnitTypeOld.Artillery: return 4;
            case UnitTypeOld.Transport: return 0;
            case UnitTypeOld.Bullfrog: return 2;
            case UnitTypeOld.Destroyer: return 2;
            case UnitTypeOld.Submarine: return 2;
            case UnitTypeOld.Battleship: return 3;
            case UnitTypeOld.NavalTransport: return 0;
            case UnitTypeOld.Carrier: return 0;
            case UnitTypeOld.Leviathan: return 4;
            case UnitTypeOld.Bunker: return 2;
            case UnitTypeOld.Tower: return 4;
            case UnitTypeOld.Amphibia: return 1;
            case UnitTypeOld.Platform: return 0;
            case UnitTypeOld.Helicopter: return 2;
            case UnitTypeOld.TransportHelicopter: return 0;
            case UnitTypeOld.Plane: return 6;
            case UnitTypeOld.AntiAir: return 0;
            default: return 0;
        }
    }

    /// <summary>
    /// Returns purchase cost for specified unit type
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public static int GetCost(UnitTypeOld type)
    {
        switch (type)
        {
            case UnitTypeOld.LBM: return 1;
            case UnitTypeOld.Tank: return 2;
            case UnitTypeOld.Mortair: return 2;
            case UnitTypeOld.WarBot: return 4;
            case UnitTypeOld.Artillery: return 4;
            case UnitTypeOld.Transport: return 2;
            case UnitTypeOld.Bullfrog: return 6;
            case UnitTypeOld.Destroyer: return 2;
            case UnitTypeOld.Submarine: return 3;
            case UnitTypeOld.Battleship: return 5;
            case UnitTypeOld.NavalTransport: return 2;
            case UnitTypeOld.Carrier: return 6;
            case UnitTypeOld.Leviathan: return 10;
            case UnitTypeOld.Bunker: return 1;
            case UnitTypeOld.Tower: return 4;
            case UnitTypeOld.Amphibia: return 3;
            case UnitTypeOld.Platform: return 2;
            case UnitTypeOld.Helicopter: return 3;
            case UnitTypeOld.TransportHelicopter: return 2;
            case UnitTypeOld.Plane: return 3;
            case UnitTypeOld.AntiAir: return 3;
            default: return 0;
        }
    }

    /// <summary>
    /// Returns max room for specified transport type
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public static int GetMaxRoom(UnitTypeOld type)
    {
        switch (type)
        {
            case UnitTypeOld.Transport: return 2;
            case UnitTypeOld.NavalTransport: return 4;
            case UnitTypeOld.TransportHelicopter: return 1;
            default: return 0;
        }
    }
}

