using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Possible Allegiances
/// </summary>
public enum Allegiance { Guardians, Dominion, Neutral}

public static class AllegianceExtentions
{

    /// <summary>
    /// Changes allegiance to opposite
    /// </summary>
    /// <param name="allegiance"></param>
    /// <returns></returns>
    public static Allegiance Opposite(Allegiance allegiance)
    {
        if(allegiance == Allegiance.Dominion)
        {
            return Allegiance.Guardians;
        }
        else
        {
            return Allegiance.Dominion;
        }
    }
}