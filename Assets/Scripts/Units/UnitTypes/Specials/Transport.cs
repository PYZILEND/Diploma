using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
    public override void InitializeUnit(LogicalMapCell cell, Allegiance allegiance)
    {
        base.InitializeUnit(cell, allegiance);
        this.remainingCapacity = this.type.capacity;
        this.embarkedUnits = new Unit[this.type.capacity];
    }

    /// <summary>
    /// When moving must also move embarked units
    /// In this case it's enough to just reassign
    /// their cell property
    /// </summary>
    /// <param name="destination"></param>
    public override void MoveToCell(LogicalMapCell destination)
    {
        base.MoveToCell(destination);
        for(int i=0; i<embarkedUnits.Length; i++)
        {
            if (embarkedUnits[i])
            {
                embarkedUnits[i].cell = destination;
            }
        }
    }

    /// <summary>
    /// When transport is destroyed
    /// All embarked units are also destroyed
    /// </summary>
    public override void DestroyVisually()
    {
        base.DestroyVisually();

        foreach (Unit unit in embarkedUnits)
        {
            if (unit) unit.isDestroyed = true;
        }
    }

    void DisplayEP()
    {
        if (transform.childCount < 2) return;
        transform.Find("UnitCanvas(Clone)").Find("Icon4").GetComponentInChildren<Text>().text = remainingCapacity.ToString();
        if (remainingCapacity == 0)
        {
            transform.Find("UnitCanvas(Clone)").Find("Icon4").GetComponent<RawImage>().color = Color.gray;
            transform.Find("UnitCanvas(Clone)").Find("Icon4").GetComponentInChildren<Text>().color = Color.gray;
        }
        else
        {
            transform.Find("UnitCanvas(Clone)").Find("Icon4").GetComponent<RawImage>().color = AllegianceExtentions.AllegianceToColor(allegiance);
            transform.Find("UnitCanvas(Clone)").Find("Icon4").GetComponentInChildren<Text>().color = AllegianceExtentions.AllegianceToColor(allegiance);
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
        DisplayEP();
    }

    /// <summary>
    /// Method for removing unit from transport
    /// </summary>
    /// <param name="unit"></param>
    public void RemoveFromTransport(Unit unit)
    {
        remainingCapacity++;
        embarkedUnits[FindUnitIndex(unit)] = null;
        DisplayEP();
    }

    /// <summary>
    /// Finds index of first free space
    /// returns -1 if no free space left
    /// </summary>
    /// <returns></returns>
    public int FindFreeSpace()
    {
        int i = 0;
        while(embarkedUnits[i] != null)
        {
            i++;          
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
