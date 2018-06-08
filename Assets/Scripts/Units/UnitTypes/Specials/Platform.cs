using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Platform is a one place transport
/// that allows embarked unit to shoot while aboard
/// </summary>
public class Platform : Unit
{
    /// <summary>
    /// References boareded unit
    /// </summary>
    public Unit boardedUnit;

    /// <summary>
    /// Initializes unique field
    /// </summary>
    /// <param name="type"></param>
    /// <param name="cell"></param>
    /// <param name="allegiance"></param>
    /// <returns></returns>
    public override void InitializeUnit(LogicalMapCell cell, Allegiance allegiance)
    {
        base.InitializeUnit(cell, allegiance);
        this.boardedUnit = null;
    }

    /// <summary>
    /// Also moves boarded unit with it
    /// </summary>
    /// <param name="destination"></param>
    public override void MoveToCell(LogicalMapCell destination)
    {
        base.MoveToCell(destination);
        boardedUnit.cell = destination;
    }

    /// <summary>
    /// If destroyed, boarded unit is also destroyed
    /// </summary>
    public override void DestroyVisually()
    {
        base.DestroyVisually();
        boardedUnit.DestroyVisually();
    }

    public void DisplayEP()
    {
            if (transform.childCount < 2) return;
            if (boardedUnit)
        {
            transform.GetChild(1).GetChild(4).GetComponentInChildren<Text>().text ="0";
            transform.Find("UnitCanvas(Clone)").GetChild(4).GetComponent<RawImage>().color = Color.gray;
                transform.Find("UnitCanvas(Clone)").GetChild(4).GetComponentInChildren<Text>().color = Color.gray;
            }
            else
        {
            transform.GetChild(1).GetChild(4).GetComponentInChildren<Text>().text = "1";
            transform.Find("UnitCanvas(Clone)").GetChild(4).GetComponent<RawImage>().color = AllegianceExtentions.AllegianceToColor(allegiance);
                transform.Find("UnitCanvas(Clone)").GetChild(4).GetComponentInChildren<Text>().color = AllegianceExtentions.AllegianceToColor(allegiance);
            }
        
    }
}
