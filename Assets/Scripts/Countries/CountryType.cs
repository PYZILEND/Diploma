using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "countryType", menuName = "CountryType")]
public class CountryType : ScriptableObject {

    public int income;
    public int guerrilla;
    public int reparations;
    public int secretArmy;
}
