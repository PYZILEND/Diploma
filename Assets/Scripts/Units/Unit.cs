using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour {

    public UnitType type;

    public int healthPoints;
    public int movePoints;

    public LogicalMapCell cell;

    public bool isDominion;

    public bool hasAttacked;
    public bool isDestroyed;

    public void Initialize(UnitType type)
    {
        this.type = type;
        healthPoints = UnitTypeExtentions.GetMaxHealth(type);
        movePoints = 0;
        hasAttacked = true;
        isDestroyed = false;
        if (isDominion)
        {
            GetComponentInChildren<MeshRenderer>().material.color = Color.red;
         }
        else
        {
            GetComponentInChildren<MeshRenderer>().material.color = Color.blue;
        }
    }

    public void ChangeTurn()
    {
        movePoints = UnitTypeExtentions.GetMaxMovePoints(type);
        hasAttacked = false;
        if (isDestroyed)
        {
            DestroyLogically();
        }
    }

    public void MoveToCell(LogicalMapCell destination)
    {
        movePoints -= destination.distance;
        cell.unit = null;
        destination.unit = this;
        cell = destination;        
        transform.SetParent(destination.transform, false);
        ValidatePosition();
    }

    public void ShootAt(LogicalMapCell cell)
    {
        hasAttacked = true;
        cell.unit.healthPoints -= UnitTypeExtentions.GetAttackPower(type);
        if (cell.unit.healthPoints <= 0)
        {
            cell.unit.DestroyVisually();
        }
    }

    public void DestroyLogically()
    {
        GetComponentInParent<GameMaster>().units.Remove(this);
        cell.unit = null;
        Destroy(gameObject);
    }

    public void DestroyVisually()
    {
        isDestroyed = true;
        GetComponentInChildren<MeshRenderer>().material.color = Color.black;
    }

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
