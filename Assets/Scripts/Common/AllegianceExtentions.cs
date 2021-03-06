﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Possible Allegiances
/// </summary>
public enum Allegiance { Sentinels, Dominion, Neutral}

public static class AllegianceExtentions
{

    /// <summary>
    /// Changes allegiance to opposite
    /// </summary>
    /// <param name="allegiance"></param>
    /// <returns></returns>
    public static Allegiance Opposite(Allegiance allegiance)
    {
        if (allegiance == Allegiance.Dominion)
        {
            return Allegiance.Sentinels;
        }
        else
        {
            return Allegiance.Dominion;
        }
    }

    /// <summary>
    /// Returns color assosiated with specified allegiance
    /// </summary>
    /// <param name="allegiance"></param>
    /// <returns></returns>
    public static Color AllegianceToColor(Allegiance allegiance)
    {
        switch (allegiance)
        {
            case Allegiance.Dominion: return Color.red;
            case Allegiance.Sentinels: return Color.blue;
            case Allegiance.Neutral: return Color.grey;
        }
        return Color.white;
    }
}