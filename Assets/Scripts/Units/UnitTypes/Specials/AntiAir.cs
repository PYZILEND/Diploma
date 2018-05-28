using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Anti-air unit
/// Protects nearby cells from plane attacks
/// </summary>
public class AntiAir : Unit
{
    /*
    /// <summary>
    /// When spawned starts protecting nearby cells
    /// </summary>
    /// <param name="type"></param>
    /// <param name="cell"></param>
    /// <param name="allegiance"></param>
    /// <returns></returns>
    public override void InitializeUnit(LogicalMapCell cell, Allegiance allegiance)
    {
        base.InitializeUnit(cell, allegiance);

        this.ProtectCell();
    }

    /// <summary>
    /// When moves must stop protecting former cells
    /// and start protecting new ones
    /// </summary>
    /// <param name="destination"></param>
    public override void MoveToCell(LogicalMapCell destination)
    {
        StopProtecting();
        base.MoveToCell(destination);
        ProtectCell();
    }

    /// <summary>
    /// Can't protect cells when embarked
    /// </summary>
    /// <param name="cell"></param>
    public override void Embark(LogicalMapCell cell)
    {
        base.Embark(cell);
        StopProtecting();
    }

    /// <summary>
    /// Cycles through cell's neighbours and
    /// sets their protected status to true
    /// </summary>
    void ProtectCell()
    {
        cell.isProtected = true;
        for(HexDirection i = HexDirection.NE; i<HexDirection.NW; i++)
        {
            cell.GetNeighbor(i).isProtected = true;
        }
    }

    /// <summary>
    /// Cycles through cell's neighbours and
    /// sets their protected status to false
    /// </summary>
    void StopProtecting()
    {
        cell.isProtected = false;
        for (HexDirection i = HexDirection.NE; i < HexDirection.NW; i++)
        {
            cell.GetNeighbor(i).isProtected = false;
        }
    }*/
}
