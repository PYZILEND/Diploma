using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Platform is a one place transport
/// that allows embarked unit to shoot while aboard
/// </summary>
public class Platform : Unit
{
    /// <summary>
    /// References boareded unit
    /// </summary>
    public Unit boardedUnit;

    /// <summary>
    /// Initializes unique field
    /// </summary>
    /// <param name="type"></param>
    /// <param name="cell"></param>
    /// <param name="allegiance"></param>
    /// <returns></returns>
    new public static Platform CreateUnit(Unit unitPrefab, LogicalMapCell cell, Allegiance allegiance)
    {
        Platform unit = (Platform)Unit.CreateUnit(unitPrefab, cell, allegiance);
        unit.boardedUnit = null;
        return unit;
    }

    /// <summary>
    /// Also moves boarded unit with it
    /// </summary>
    /// <param name="destination"></param>
    new public void MoveToCell(LogicalMapCell destination)
    {
        base.MoveToCell(destination);
        boardedUnit.cell = destination;
    }

    /// <summary>
    /// If destroyed, boarded unit is also destroyed
    /// </summary>
    new public void DestroyVisually()
    {
        base.DestroyVisually();
        boardedUnit.DestroyVisually();
    }
}
