using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Super unit has two weapons to attack with
/// </summary>
public class SuperUnit : Unit
{
    /// <summary>
    /// Weather unit will shoot with his primary
    /// or secondary weapon
    /// </summary>
    public bool primaryWeaponSelected;
    /// <summary>
    /// Weather unit has attacked with his
    /// secondary weapon
    /// </summary>
    public bool hasAttackedWithSecondaryWeapon;

    /// <summary>
    /// When created also sets unique fields initial values
    /// </summary>
    /// <param name="type"></param>
    /// <param name="cell"></param>
    /// <param name="allegiance"></param>
    /// <returns></returns>
    new public static SuperUnit CreateUnit(Unit unitPrefab, LogicalMapCell cell, Allegiance allegiance)
    {
        SuperUnit unit = (SuperUnit)Unit.CreateUnit(unitPrefab, cell, allegiance);

        unit.primaryWeaponSelected = true;
        unit.hasAttackedWithSecondaryWeapon = false;
        return unit;
    }

    /// <summary>
    /// When reseted also resets unique values
    /// </summary>
    new public void ResetUnit()
    {
        base.ResetUnit();
        primaryWeaponSelected = true;
        hasAttackedWithSecondaryWeapon = false;
    }

    /// <summary>
    /// When changes turn also refreshes unique values
    /// Must be refreshed before calling Destroy in ChangeTurn
    /// if unit is destroyed
    /// </summary>
    new public void ChangeTurn()
    {
        primaryWeaponSelected = true;
        hasAttackedWithSecondaryWeapon = false;
        base.ChangeTurn();        
    }

    /// <summary>
    /// Shoots based on what weapon is selected
    /// </summary>
    /// <param name="cell"></param>
    new public void ShootAt(LogicalMapCell cell)
    {
        if (primaryWeaponSelected)
        {
            base.ShootAt(cell);
        }
        else
        {
            hasAttackedWithSecondaryWeapon = true;
            cell.unit.healthPoints -= type.secondAttackPower;
            if (cell.unit.healthPoints <= 0)
            {
                cell.unit.DestroyVisually();
            }
        }
    }
}
