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
    public override void InitializeUnit(LogicalMapCell cell, Allegiance allegiance)
    {
        base.InitializeUnit(cell, allegiance);

        this.remainingCapacity = this.type.capacity;
        this.planes = new Plane[this.type.capacity];
    }

    /// <summary>
    /// When moving also move it's planes
    /// </summary>
    /// <param name="destination"></param>
    public override void MoveToCell(LogicalMapCell destination)
    {
        base.MoveToCell(destination);

        for(int i= 0; i < planes.Length; i++)
        {
            if (planes[i])
            {
                planes[i].cell = destination;
            }
        }
    }

    /// <summary>
    /// When destroyed also destroy it's planes
    /// </summary>
    public override void DestroyVisually()
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
        planes[FindFreeSpace()] = plane;
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
