using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UnitInfo : MonoBehaviour {

    public Text unitName;
    public Text unitHP;
    public Text unitMP;
    public Text unitDamage;
    public Text unitRange;
    public Text unitCost;

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

    public void Hide()
    {
        this.gameObject.SetActive(false);
    }
}
