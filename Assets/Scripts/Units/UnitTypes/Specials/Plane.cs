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

    public override void InitializeUnit(LogicalMapCell cell, Allegiance allegiance)
    {
        GameMaster.units.Add(this);

        this.transform.SetParent(cell.transform, false);
        this.cell = cell;

        this.allegiance = allegiance;
        this.healthPoints = this.type.maxHealth;
        this.movePoints = 0;
        this.hasAttacked = true;
        this.isDestroyed = false;

        this.GetComponentInChildren<MeshRenderer>().material.color = AllegianceExtentions.AllegianceToColor(allegiance);
        this.ValidatePosition();

        cell.country.capitalCity.TakeAboard(this);
    }

    /// <summary>
    /// Plane can only be on a capital or carrier
    /// and when moving it must adjust their capacity accordingly
    /// </summary>
    /// <param name="cell"></param>
    public override void MoveToCell(LogicalMapCell cell)
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
            transform.SetParent(cell.transform, false);
            transform.localPosition = new Vector3(0, 0, 0);
        }
        else
        {
            Carrier carrier = (Carrier)cell.unit;
            carrier.TakeAboard(this);
            transform.SetParent(carrier.transform, false);
            transform.localPosition = new Vector3(0, 0, 0);
        }

        this.cell = cell;
        movePoints = 0;
        hasAttacked = true;
        ValidatePosition();
    }

    /// <summary>
    /// Removes unit from game
    /// </summary>
    public override void DestroyLogically()
    {
        GameMaster.units.Remove(this);        
        Destroy(gameObject);
    }

    /// <summary>
    /// Changes unit's model to destroyed one and marks it for logical destraction
    /// </summary>
    public override void DestroyVisually()
    {
        isDestroyed = true;
        GetComponentInChildren<MeshRenderer>().enabled = false;
    }    
}
