using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MultipleSelection : MonoBehaviour {

    public Button[] selections;
    Unit[] units;
    Unit unit;

    public void Show(Vector3 position, Unit unit)
    {
        this.gameObject.SetActive(true);
        GetComponent<RectTransform>().transform.position = position;
        GetComponent<RectTransform>().sizeDelta = new Vector2(10 + 60 * (unit.type.capacity + 1), 60);

        for(int i = 0; i<unit.type.capacity + 1; i++)
        {
            selections[i].gameObject.SetActive(true);
        }

        this.unit = unit;
        units = new Unit[unit.type.capacity];

        if (unit is Transport)
        {
            Transport transport = (Transport)unit;
            
            for(int i = 0; i < transport.embarkedUnits.Length; i++)
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
    }

    public void Hide()
    {
        this.gameObject.SetActive(false);
        for(int i = 0; i<selections.Length; i++)
        {
            selections[i].gameObject.SetActive(false);
        }
    }

    public void SelectUnit(int index)
    {
        if (index == -1)
        {
            UnitControls.SelectUnit(unit);
            Hide();
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
