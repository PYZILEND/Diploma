using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CountryType
{
    poor, average, rich
}

public static class CountryTypeExtentions
{
    /// <summary>
    /// Returns country type based on number from 0 to 2
    /// </summary>
    /// <param name="type">number of type</param>
    /// <returns>country type</returns>
    public static CountryType GetCountyType(int type)
    {
        switch (type)
        {
            case 0: return CountryType.poor;
            case 1: return CountryType.average;
            case 2: return CountryType.rich;
            default: return CountryType.average;
        }
    }

    public static Color GetColor(CountryType type)
    {
        switch (type)
        {
            case CountryType.poor: return Color.black;
            case CountryType.average: return Color.green;
            case CountryType.rich: return Color.red;
            default: return Color.magenta;
        }
    }

    public static Color GetColorCapital(CountryType type)
    {
        switch (type)
        {
            case CountryType.poor: return Color.cyan;
            case CountryType.average: return Color.yellow;
            case CountryType.rich: return Color.gray;
            default: return Color.magenta;
        }
    }
    public static byte GetIncome(CountryType type)
    {
        switch (type)
        {
            case CountryType.poor: return 2;
            case CountryType.average: return 3;
            case CountryType.rich: return 4;
            default: return 0;
        }
    }

    public static byte GetSecretArmy(CountryType type)
    {
        switch (type)
        {
            case CountryType.poor: return 8;
            case CountryType.average: return 12;
            case CountryType.rich: return 18;
            default: return 0;
        }
    }

    public static byte GetReparations(CountryType type)
    {
        switch (type)
        {
            case CountryType.poor: return 8;
            case CountryType.average: return 12;
            case CountryType.rich: return 18;
            default: return 0;
        }
    }

    public static byte GetGuerilla(CountryType type)
    {
        switch (type)
        {
            case CountryType.poor: return 4;
            case CountryType.average: return 6;
            case CountryType.rich: return 8;
            default: return 0;
        }
    }
}