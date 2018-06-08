using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MultipleSelection : MonoBehaviour {

    public Button[] selections;
    Unit[] units;
    Unit unit;

    bool showingForPlanes
    {
        get
        {
            for(int i=0; i<units.Length; i++)
            {
                if(units[i] is Plane)
                {
                    return true;
                }
            }
            return false;
        }
    }

    public void ShowForTransport(Vector3 position, Unit unit)
    {
        this.gameObject.SetActive(true);
        GetComponent<RectTransform>().transform.position = position;
        GetComponent<RectTransform>().sizeDelta = new Vector2(10 + 60 * (unit.type.capacity + 1), 60);

        for(int i = 0; i <= unit.type.capacity; i++)
        {
            selections[i].gameObject.SetActive(true);
        }

        this.unit = unit;
        units = new Unit[unit.type.capacity];

        selections[0].GetComponentInChildren<Text>().text = unit.type.unitName;
        if (unit is Transport)
        {
            Transport transport = (Transport)unit;

            for (int i = 0; i < transport.embarkedUnits.Length; i++)
            {
                if (transport.embarkedUnits[i])
                {
                    units[i] = transport.embarkedUnits[i];
                }
                else
                {
                    units[i] = null;
                }
            }
        }
        else if(unit is Platform)
        {
            Platform platform = (Platform)unit;
            units[0] = platform.boardedUnit;
        }
        else if(unit is Carrier)
        {
            Carrier carrier = (Carrier)unit;
            for (int i = 0; i < carrier.planes.Length; i++)
            {                
                if (carrier.planes[i])
                {
                    units[i] = carrier.planes[i];
                }
                else
                {
                    units[i] = null;
                }
            }
        }
        ButtonsText();
    }

    public void ShowForCapital(Vector3 position, LogicalMapCell cell)
    {
        this.gameObject.SetActive(true);
        if (cell.unit)
        {
            GetComponent<RectTransform>().transform.position = position;
            GetComponent<RectTransform>().sizeDelta = new Vector2(10 + 60 * (cell.capital.planes.Length + 1), 60);
            unit = cell.unit;
            selections[0].GetComponentInChildren<Text>().text = unit.type.unitName;
            for (int i = 0; i <= cell.capital.planes.Length; i++)
            {
                selections[i].gameObject.SetActive(true);
            }
        }
        else
        {
            GetComponent<RectTransform>().transform.position = position;
            GetComponent<RectTransform>().sizeDelta = new Vector2(10 + 60 * (cell.capital.planes.Length), 60);
            for (int i = 1; i <= cell.capital.planes.Length; i++)
            {
                selections[i].gameObject.SetActive(true);
                selections[i].GetComponent<RectTransform>().transform.localPosition = new Vector2(5 + 60 * (i - 1), -5);
            }
        }

        units = new Unit[cell.capital.planes.Length];
        for(int i = 0; i < cell.capital.planes.Length; i++)
        {
            units[i] = cell.capital.planes[i];
        }
        ButtonsText();
    }

    public void ButtonsText()
    {
        int i = 1;
        if (unit)
        {
            selections[0].GetComponentInChildren<Text>().text = unit.type.unitName;
           // i++;
        }
        for (int j=0; j < units.Length; j++,i++)
        {
            if (units[j])
            {
                //selections[i].gameObject.SetActive(true);
                selections[i].GetComponentInChildren<Text>().text = units[j].type.unitName;
            }
            else
            {
                selections[i].gameObject.SetActive(false);
            }
        }
        

    }

    public void ShowForSuperUnit(Vector3 position, Unit unit, Unit otherUnit)
    {
        this.gameObject.SetActive(true);
        GetComponent<RectTransform>().transform.position = position;
        GetComponent<RectTransform>().sizeDelta = new Vector2(130, 60);

        selections[0].gameObject.SetActive(true);
        selections[1].gameObject.SetActive(true);

        this.unit = unit;
        units = new Unit[1];
        units[0] = otherUnit;
        selections[0].GetComponentInChildren<Text>().text = "attack 1";
        selections[1].GetComponentInChildren<Text>().text = "attack 2";
    }

    public void Hide()
    {
        this.gameObject.SetActive(false);
        for(int i = 0; i<selections.Length; i++)
        {
            selections[i].gameObject.SetActive(false);
            selections[i].GetComponent<RectTransform>().transform.localPosition = new Vector2(5 + 60 * i, -5);
        }
        unit = null;
        units = new Unit[0];
    }

    public void Select(int index)
    {
        if (unit is SuperUnit)
        {
            SuperUnit super = (SuperUnit)unit;
            if (index == -1 && !super.hasAttacked)
            {                
                super.primaryWeaponSelected = true;
                super.ShootAt(units[0].cell);
                Hide();
                return;
            }
            if(index == 0 && !super.hasAttackedWithSecondaryWeapon)
            {
                super.primaryWeaponSelected = false;
                super.ShootAt(units[0].cell);
                Hide();
            }
            
        }
        else
        {
            if (index == -1)
            {
                if (unit is Transport && showingForPlanes)
                {
                    Hide();
                    ShowForTransport(Input.mousePosition, unit);
                }
                else
                {
                    UnitControls.SelectUnit(unit);
                    Hide();
                }
            }
            else
            {
                if (units[index])
                {
                    UnitControls.SelectUnit(units[index]);
                    Hide();
                }
            }
        }
    }
}
