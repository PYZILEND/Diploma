using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Carrier is a unit that can house planes
/// </summary>
public class Carrier : Unit
{
    /// <summary>
    /// Shows how much free space there is on carrier
    /// </summary>
    public int remainingCapacity;

    /// <summary>
    /// References stationed planes
    /// </summary>
    public Plane[] planes;

    /// <summary>
    /// Initializing carrier's unique fields
    /// </summary>
    /// <param name="type"></param>
    /// <param name="cell"></param>
    /// <param name="allegiance"></param>
    /// <returns></returns>
    new public static Carrier CreateUnit(Unit unitPrefab, LogicalMapCell cell, Allegiance allegiance)
    {
        Carrier unit = (Carrier)Unit.CreateUnit(unitPrefab, cell, allegiance);

        unit.remainingCapacity = unit.type.capacity;
        unit.planes = new Plane[unit.type.capacity];

        return unit;
    }

    /// <summary>
    /// When moving also move it's planes
    /// </summary>
    /// <param name="destination"></param>
    new public void MoveToCell(LogicalMapCell destination)
    {
        base.MoveToCell(destination);

        foreach (Plane plane in planes)
        {
            plane.cell = destination;
        }
    }

    /// <summary>
    /// When destroyed also destroy it's planes
    /// </summary>
    new public void DestroyVisually()
    {
        base.DestroyVisually();

        for (int i = 0; i < planes.Length; i++)
        {
            if (planes[i])
            {
                planes[i].DestroyVisually();
            }
        }
    }

    /// <summary>
    /// Method for taking plane aboard
    /// </summary>
    /// <param name="unit"></param>
    public void TakeAboard(Plane plane)
    {
        remainingCapacity--;
        planes[FindFreeSpace()] = plane;
    }

    /// <summary>
    /// Method for removing plane from carrier
    /// </summary>
    /// <param name="unit"></param>
    public void RemoveFromCarrier(Plane plane)
    {
        remainingCapacity++;
        planes[FindPlaneIndex(plane)] = null;
    }

    /// <summary>
    /// Finds index of first free space
    /// returns -1 if no free space left
    /// </summary>
    /// <returns></returns>
    public int FindFreeSpace()
    {
        int i = 0;
        while (planes[i] != null)
        {
            i++;
            if (i > planes.Length)
            {
                return -1;
            }
        }
        return i;
    }

    /// <summary>
    /// Finds index of a said plane on carrier
    /// returns -1 if no such plane is on carrier
    /// </summary>
    /// <param name="unit"></param>
    /// <returns></returns>
    public int FindPlaneIndex(Plane plane)
    {
        int i = 0;
        while (planes[i] != plane)
        {
            i++;
            if (i > planes.Length)
            {
                return -1;
            }
        }
        return i;
    }
}
