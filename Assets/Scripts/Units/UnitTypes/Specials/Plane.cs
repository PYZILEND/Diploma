using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Plane is a unique unit that doesn't inherit Unit class
/// should inherit afterall???
/// </summary>
public class Plane : Unit
{
    /// <summary>
    /// Plane can only attack or move
    /// but not both in one turn
    /// </summary>
    public bool hasActed
    {
        get
        {
            return hasAttacked && (movePoints != 0);
        }
    }

    /// <summary>
    /// Weather plane is on carrier or in a capital
    /// </summary>
    public bool isOnCarrier
    {
        get
        {
            if (cell.country)
            {
                return false;
            }
            return true;
        }
    }

    /// <summary>
    /// Method for creating a plane
    /// </summary>
    /// <param name="cell"></param>
    /// <param name="allegiance"></param>
    /// <returns></returns>
    new public static Plane CreateUnit(Unit unitPrefab, LogicalMapCell cell, Allegiance allegiance)
    {
        Plane plane = (Plane) Unit.CreateUnit(unitPrefab, cell, allegiance);

        return plane;
    }

    /// <summary>
    /// Plane can only be on a capital or carrier
    /// and when moving it must adjust their capacity accordingly
    /// </summary>
    /// <param name="cell"></param>
    new public void MoveToCell(LogicalMapCell cell)
    {
        if (isOnCarrier)
        {
            Carrier carrier = (Carrier) this.cell.unit;
            carrier.RemoveFromCarrier(this);
        }
        else
        {
            this.cell.country.capitalCity.RemoveFromCapital(this);
        }

        if (cell.country)
        {
            cell.country.capitalCity.TakeAboard(this);
        }
        else
        {
            Carrier carrier = (Carrier)cell.unit;
            carrier.TakeAboard(this);
        }

        this.cell = cell;
        movePoints = 0;
    }

    /// <summary>
    /// Removes unit from game
    /// </summary>
    new public void DestroyLogically()
    {
        GameMaster.units.Remove(this);
        Destroy(gameObject);
    }

    /// <summary>
    /// Changes unit's model to destroyed one and marks it for logical destraction
    /// </summary>
    new public void DestroyVisually()
    {
        isDestroyed = true;
        GetComponentInChildren<MeshRenderer>().enabled = false;
    }
}
