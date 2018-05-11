using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Used to display information about unit in UI
/// </summary>
public class UnitInfo : MonoBehaviour {

    //Links to UI elements
    public Text unitName;
    public Text unitHP;
    public Text unitMP;
    public Text unitDamage;
    public Text unitRange;
    public Text unitCost;

    /// <summary>
    /// Shows information panel and fills it with unit's stats
    /// </summary>
    /// <param name="unit"></param>
    public void ShowUnitInfo(Unit unit)
    {
        this.gameObject.SetActive(true);
        unitName.text = "Not implemented";
        unitHP.text = unit.healthPoints.ToString();
        unitMP.text = unit.movePoints.ToString();
        unitDamage.text = UnitTypeExtentions.GetAttackPower(unit.type).ToString();
        unitRange.text = UnitTypeExtentions.GetAttackRange(unit.type).ToString();
        unitCost.text = UnitTypeExtentions.GetCost(unit.type).ToString();
    }

    /// <summary>
    /// Hides unit information panel
    /// </summary>
    public void Hide()
    {
        this.gameObject.SetActive(false);
    }
}
