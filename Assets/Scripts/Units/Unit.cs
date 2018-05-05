using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour {

    /// <summary>
    /// Unit type
    /// </summary>
    public UnitType type;

    //Unit's health and move points
    public int healthPoints;
    public int movePoints;

    /// <summary>
    /// Cell at which unit is positioned
    /// </summary>
    public LogicalMapCell cell;

    /// <summary>
    /// Unit's allegiance
    /// </summary>
    public Allegiance allegiance;

    //Flags for attack and being destroyed
    public bool hasAttacked;
    public bool isDestroyed;

    /// <summary>
    /// Initializes unit based on it's type
    /// </summary>
    /// <param name="type"></param>
    public void Initialize(UnitType type)
    {
        this.type = type;
        healthPoints = UnitTypeExtentions.GetMaxHealth(type);
        movePoints = 0;
        hasAttacked = true;
        isDestroyed = false;
        GetComponentInChildren<MeshRenderer>().material.color = AllegianceExtentions.AllegianceToColor(allegiance);
    }

    /// <summary>
    /// Restores unit's move points and attack status
    /// If unit was destroyed on prev turn it's removed from game
    /// </summary>
    public void ChangeTurn()
    {
        movePoints = UnitTypeExtentions.GetMaxMovePoints(type);
        hasAttacked = false;
        if (isDestroyed)
        {
            DestroyLogically();
        }
    }

    /// <summary>
    /// Moves unit to specified cell
    /// </summary>
    /// <param name="destination"></param>
    public void MoveToCell(LogicalMapCell destination)
    {
        if (destination.country)
        {
            if (cell.country == null || destination.country != cell.country)
            {
                if (!destination.country.isInvaded)
                {
                    destination.unit = this;
                    if (destination.country.isInvaded)
                    {
                        destination.country.TriggerInvasion();
                    }
                }
            }

            if (destination.country.capital == destination && destination.country.GetAllegiance() != allegiance)
            {
                destination.country.SwitchAllegiance(allegiance);
            }
        }

        if (cell.country && cell.country.isInvaded)
        {
            cell.unit = null;
            if (!cell.country.isInvaded)
            {
                cell.country.TriggerLiberation();
            }
        }

        movePoints -= destination.distance;
        cell.unit = null;
        destination.unit = this;
        cell = destination;
        transform.SetParent(destination.transform, false);
        ValidatePosition();        
    }

    /// <summary>
    /// Unit shoots at specified cell
    /// If unit on said cell looses enough health it's destroyed
    /// </summary>
    /// <param name="cell"></param>
    public void ShootAt(LogicalMapCell cell)
    {
        hasAttacked = true;
        cell.unit.healthPoints -= UnitTypeExtentions.GetAttackPower(type);
        if (cell.unit.healthPoints <= 0)
        {
            cell.unit.DestroyVisually();
        }
    }

    /// <summary>
    /// Removes unit from game
    /// </summary>
    public void DestroyLogically()
    {
        GameMaster.units.Remove(this);
        cell.unit = null;
        Destroy(gameObject);
    }

    /// <summary>
    /// Changes unit's model to destroyed one and marks it for logical destraction
    /// </summary>
    public void DestroyVisually()
    {
        if (cell.country && cell.country.isInvaded)
        {
            isDestroyed = true;
            if (!cell.country.isInvaded)
            {
                cell.country.TriggerLiberation();
            }
        }
        isDestroyed = true;
        GetComponentInChildren<MeshRenderer>().material.color = Color.black;
        
    }

    /// <summary>
    /// Aligns unit's position with relative phisical map terrain
    /// </summary>
    public void ValidatePosition()
    {
        RaycastHit hit;
        Physics.Raycast(transform.position, new Vector3(0f, -1f, 0f), out hit);

        Vector3 position = hit.point;
        position.y += 0.5f;

        this.transform.position = position;
        this.transform.localRotation = Quaternion.Euler(hit.normal);
    }
}
