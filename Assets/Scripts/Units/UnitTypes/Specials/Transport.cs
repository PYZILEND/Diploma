using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Transport can take other units aboard and move them
/// </summary>
public class Transport : Unit
{
    /// <summary>
    /// Shows how much units transport can take
    /// </summary>
    public int remainingCapacity;
    /// <summary>
    /// References embarked units
    /// </summary>
    public Unit[] embarkedUnits;

    /// <summary>
    /// When creating must also initialize unique fields
    /// </summary>
    /// <param name="type"></param>
    /// <param name="cell"></param>
    /// <param name="allegiance"></param>
    /// <returns></returns>
    new public static Transport CreateUnit(Unit unitPrefab, LogicalMapCell cell, Allegiance allegiance)
    {
        Transport unit = (Transport) Unit.CreateUnit(unitPrefab, cell, allegiance);

        unit.remainingCapacity = unit.type.capacity;
        unit.embarkedUnits = new Unit[unit.type.capacity];

        return unit;
    }

    /// <summary>
    /// When moving must also move embarked units
    /// In this case it's enough to just reassign
    /// their cell property
    /// </summary>
    /// <param name="destination"></param>
    new public void MoveToCell(LogicalMapCell destination)
    {
        base.MoveToCell(destination);

        foreach(Unit unit in embarkedUnits)
        {
            unit.cell = destination;
        }
    }

    /// <summary>
    /// When transport is destroyed
    /// All embarked units are also destroyed
    /// </summary>
    new public void DestroyVisually()
    {
        base.DestroyVisually();

        foreach (Unit unit in embarkedUnits)
        {
            unit.isDestroyed = true;
        }
    }

    /// <summary>
    /// Method for takeing unit aboard
    /// </summary>
    /// <param name="unit"></param>
    public void TakeAboard(Unit unit)
    {
        remainingCapacity--;
        embarkedUnits[FindFreeSpace()] = unit;
    }

    /// <summary>
    /// Method for removing unit from transport
    /// </summary>
    /// <param name="unit"></param>
    public void RemoveFromTransport(Unit unit)
    {
        remainingCapacity++;
        embarkedUnits[FindUnitIndex(unit)] = null;
    }

    /// <summary>
    /// Finds index of first free space
    /// returns -1 if no free space left
    /// </summary>
    /// <returns></returns>
    public int FindFreeSpace()
    {
        Debug.Log(embarkedUnits.Length);
        int i = 0;
        while(embarkedUnits[i] != null)
        {
            i++;
            Debug.Log(i);
            Debug.Log(embarkedUnits[i]);
            if (i >= embarkedUnits.Length)
            {
                return -1;
            }
        }
        return i;
    }

    /// <summary>
    /// Finds index of a said unit in transport
    /// returns -1 if no such unit is in transport
    /// </summary>
    /// <param name="unit"></param>
    /// <returns></returns>
    public int FindUnitIndex(Unit unit)
    {
        int i = 0;
        while(embarkedUnits[i] != unit)
        {            
            i++;
            if(i >= embarkedUnits.Length)
            {
                return -1;
            }
        }
        return i;
    }
}
