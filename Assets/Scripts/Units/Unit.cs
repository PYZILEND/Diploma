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
    /// References unit's transport
    /// </summary>
    public Transport transport;
    public bool isEmbarked
    {
        get
        {
            if (transport)
            {
                return true;
            }
            return false;
        }
    }

    /// <summary>
    /// References unit's platform
    /// </summary>
    public Platform platform;
    public bool isOnPlatform
    {
        get
        {
            if (platform)
            {
                return true;
            }
            return false;
        }
    }

    /// <summary>
    /// Creates unit of specified type on specified cell with said allegiance
    /// </summary>
    /// <param name="type"></param>
    /// <param name="cell"></param>
    /// <param name="allegiance"></param>
    /// <returns></returns>
    public static Unit CreateUnit(Unit unitPrefab, LogicalMapCell cell, Allegiance allegiance)
    {
        Unit unit = Instantiate(unitPrefab);
        GameMaster.units.Add(unit);

        unit.transform.SetParent(cell.transform, false);
        unit.cell = cell;        
        cell.unit = unit;

        unit.allegiance = allegiance;       
        unit.healthPoints = unit.type.maxHealth;
        unit.movePoints = 0;
        unit.hasAttacked = true;
        unit.isDestroyed = false;

        unit.GetComponentInChildren<MeshRenderer>().material.color = AllegianceExtentions.AllegianceToColor(allegiance);
        unit.ValidatePosition();

        unit.AffectCountryes(cell);

        return unit;
    }

    /// <summary>
    /// Resets unit to it's default state
    /// </summary>
    public void ResetUnit()
    {
        healthPoints = type.maxHealth;
        movePoints = type.maxMovePoints;
        hasAttacked = false;
        isDestroyed = false;
    }

    /// <summary>
    /// Restores unit's move points and attack status
    /// If unit was destroyed on prev turn it's removed from game
    /// </summary>
    public void ChangeTurn()
    {
        movePoints = type.maxMovePoints;
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
        AffectCountryes(destination);

        movePoints -= destination.distance;
        cell.unit = null;
        destination.unit = this;
        cell = destination;
        transform.SetParent(destination.transform, false);
        ValidatePosition();        
    }

    /// <summary>
    /// Affect countryes by this unit's movement or placement
    /// </summary>
    /// <param name="destination"></param>
    public void AffectCountryes(LogicalMapCell destination)
    {
        //Save unit in destination to revert changes later
        //Used when we board transport or platform
        Unit destUnit = null;
        if (destination.unit)
        {
            destUnit = destination.unit;
        }

        //If destination cell has a country
        if (destination.country)
        {
            //If we come from another country or from neutral area (ocean, impassable)
            if (cell.country == null || destination.country != cell.country)
            {
                //If this changes invaded status, then country gets invaded
                if (!destination.country.isInvaded)
                {
                    destination.unit = this;
                    if (destination.country.isInvaded)
                    {
                        destination.country.TriggerInvasion(allegiance);
                    }
                }
            }

            //If we moved into capital and it has different allegiance
            if (destination.country.capital == destination && destination.country.allegiance != allegiance)
            {
                destination.country.SwitchAllegiance(allegiance);
            }
        }

        //If leaving old cell changed invaded status
        if (cell.country && cell.country.isInvaded)
        {
            cell.unit = null;
            if (!cell.country.isInvaded)
            {
                cell.country.TriggerLiberation();
            }            
        }

        //Refert values back before leaving method
        if (destUnit)
        {
            destination.unit = destUnit;
        }
        else
        {
            destination.unit = null;
        }
        cell.unit = this;
    }

    /// <summary>
    /// Unit shoots at specified cell
    /// If unit on said cell looses enough health it's destroyed
    /// </summary>
    /// <param name="cell"></param>
    public void ShootAt(LogicalMapCell cell)
    {
        hasAttacked = true;
        cell.unit.healthPoints -= type.attackPower;
        if (cell.unit.healthPoints <= 0)
        {
            cell.unit.DestroyVisually();
        }
        movePoints = 0;
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

    /// <summary>
    /// Method for embarking transport
    /// </summary>
    /// <param name="cell"></param>
    public void Embark(LogicalMapCell cell)
    {
        transport = (Transport) cell.unit;

        AffectCountryes(cell);

        movePoints -= cell.distance;
        this.cell.unit = null;
        this.cell = cell;
        transform.SetParent(transport.transform, false);
        GetComponentInChildren<MeshRenderer>().enabled = false;

        transport.TakeAboard(this);
    }

    /// <summary>
    /// Method for disembarking transport
    /// </summary>
    /// <param name="cell"></param>
    public void Disembark(LogicalMapCell cell)
    {               
        AffectCountryes(cell);

        movePoints -= cell.distance;
        this.cell = cell;
        cell.unit = this;        
        transform.SetParent(cell.transform, false);
        GetComponentInChildren<MeshRenderer>().enabled = true;
        ValidatePosition();

        transport.RemoveFromTransport(this);
        transport = null;
    }

    /// <summary>
    /// Method for boarding platform
    /// </summary>
    /// <param name="cell"></param>
    public void BoardPlatform(LogicalMapCell cell)
    {
        platform = (Platform)cell.unit;
        platform.boardedUnit = this;

        AffectCountryes(cell);

        movePoints -= cell.distance;
        this.cell.unit = null;
        transform.SetParent(platform.transform, false);
        ValidatePosition();
    }

    /// <summary>
    /// Method for leaving platform
    /// </summary>
    /// <param name="cell"></param>
    public void LeavePlatform(LogicalMapCell cell)
    {
        platform.boardedUnit = null;
        platform = null;

        AffectCountryes(cell);

        movePoints -= cell.distance;
        this.cell = cell;
        cell.unit = this;
        transform.SetParent(cell.transform, false);
        ValidatePosition();
    }
}
