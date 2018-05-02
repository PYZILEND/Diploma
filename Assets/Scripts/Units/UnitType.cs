using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Possible unit types
/// </summary>
public enum UnitType
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
    public static Unit CreateUnitFromType(UnitType type)
    {
        if(type == UnitType.Transport || type==UnitType.NavalTransport || type==UnitType.TransportHelicopter)
        {
            return new Transport();
        }
        if(type == UnitType.Bullfrog || type == UnitType.Leviathan)
        {
            return new SuperUnit();
        }
        if (type == UnitType.Platform)
        {
            return new Platform();
        }
        if(type == UnitType.Carrier)
        {
            return new Carrier();
        }
        if (type == UnitType.Plane)
        {
            return new Plane();
        }
        if (type == UnitType.AntiAir)
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
    public static int GetMaxHealth(UnitType type)
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

    /// <summary>
    /// Returns max move points for specified unit type
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public static int GetMaxMovePoints(UnitType type)
    {
        switch (type)
        {
            case UnitType.LBM: return 1;
            case UnitType.Tank: return 2;
            case UnitType.Mortair: return 1;
            case UnitType.WarBot: return 2;
            case UnitType.Artillery: return 1;
            case UnitType.Transport: return 5;
            case UnitType.Bullfrog: return 2;
            case UnitType.Destroyer: return 3;
            case UnitType.Submarine: return 5;
            case UnitType.Battleship: return 4;
            case UnitType.NavalTransport: return 6;
            case UnitType.Carrier: return 3;
            case UnitType.Leviathan: return 4;
            case UnitType.Bunker: return 0;
            case UnitType.Tower: return 0;
            case UnitType.Amphibia: return 3;
            case UnitType.Platform: return 3;
            case UnitType.Helicopter: return 4;
            case UnitType.TransportHelicopter: return 4;
            case UnitType.Plane: return 12;
            case UnitType.AntiAir: return 1;
            default: return 0;
        }
    }

    /// <summary>
    /// Returns attack power for specified unit type
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public static int GetAttackPower(UnitType type)
    {
        switch (type)
        {
            case UnitType.LBM: return 1;
            case UnitType.Tank: return 2;
            case UnitType.Mortair: return 2;
            case UnitType.WarBot: return 3;
            case UnitType.Artillery: return 3;
            case UnitType.Transport: return 0;
            case UnitType.Bullfrog: return 5; //Need to think smtg out
            case UnitType.Destroyer: return 1;
            case UnitType.Submarine: return 2;
            case UnitType.Battleship: return 3;
            case UnitType.NavalTransport: return 0;
            case UnitType.Carrier: return 0;
            case UnitType.Leviathan: return 5; //Need to think smtg out
            case UnitType.Bunker: return 1;
            case UnitType.Tower: return 2;
            case UnitType.Amphibia: return 2;
            case UnitType.Platform: return 0;
            case UnitType.Helicopter: return 2;
            case UnitType.TransportHelicopter: return 0;
            case UnitType.Plane: return 1;
            case UnitType.AntiAir: return 0;
            default: return 0;
        }
    }

    /// <summary>
    /// Returns attack range for specified unit type
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public static int GetAttackRange(UnitType type)
    {
        switch (type)
        {
            case UnitType.LBM: return 1;
            case UnitType.Tank: return 1;
            case UnitType.Mortair: return 2;
            case UnitType.WarBot: return 2;
            case UnitType.Artillery: return 4;
            case UnitType.Transport: return 0;
            case UnitType.Bullfrog: return 2;
            case UnitType.Destroyer: return 2;
            case UnitType.Submarine: return 2;
            case UnitType.Battleship: return 3;
            case UnitType.NavalTransport: return 0;
            case UnitType.Carrier: return 0;
            case UnitType.Leviathan: return 4;
            case UnitType.Bunker: return 2;
            case UnitType.Tower: return 4;
            case UnitType.Amphibia: return 1;
            case UnitType.Platform: return 0;
            case UnitType.Helicopter: return 2;
            case UnitType.TransportHelicopter: return 0;
            case UnitType.Plane: return 6;
            case UnitType.AntiAir: return 0;
            default: return 0;
        }
    }

    /// <summary>
    /// Returns purchase cost for specified unit type
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public static int GetCost(UnitType type)
    {
        switch (type)
        {
            case UnitType.LBM: return 1;
            case UnitType.Tank: return 2;
            case UnitType.Mortair: return 2;
            case UnitType.WarBot: return 4;
            case UnitType.Artillery: return 4;
            case UnitType.Transport: return 2;
            case UnitType.Bullfrog: return 6;
            case UnitType.Destroyer: return 2;
            case UnitType.Submarine: return 3;
            case UnitType.Battleship: return 5;
            case UnitType.NavalTransport: return 2;
            case UnitType.Carrier: return 6;
            case UnitType.Leviathan: return 10;
            case UnitType.Bunker: return 1;
            case UnitType.Tower: return 4;
            case UnitType.Amphibia: return 3;
            case UnitType.Platform: return 2;
            case UnitType.Helicopter: return 3;
            case UnitType.TransportHelicopter: return 2;
            case UnitType.Plane: return 3;
            case UnitType.AntiAir: return 3;
            default: return 0;
        }
    }

    /// <summary>
    /// Returns max room for specified transport type
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public static int GetMaxRoom(UnitType type)
    {
        switch (type)
        {
            case UnitType.Transport: return 2;
            case UnitType.NavalTransport: return 4;
            case UnitType.TransportHelicopter: return 1;
            default: return 0;
        }
    }
}

