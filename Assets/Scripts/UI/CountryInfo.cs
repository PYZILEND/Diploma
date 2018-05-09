using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CountryInfo : MonoBehaviour {

    public Text countryName;
    public Text countryTreasury;
    public Text countryGuerrilla;
    public Text countrySecretArmy;
    public Text countryIncomeTurns;
    public Text countryIncome;

    public void ShowCountryInfo(Country country)
    {
        countryName.text = country.countryName;
        countryTreasury.text = country.treasury.ToString();
        countryGuerrilla.text = country.guerilla.ToString();
        countrySecretArmy.text = country.secretArmy.ToString();
        countryIncomeTurns.text = country.incomeTurnsLeft.ToString();
        countryIncome.text = CountryTypeExtentions.GetIncome(country.type).ToString();
    }

    public void Clear()
    {
        countryName.text = "";
        countryTreasury.text = "";
        countryGuerrilla.text = "";
        countrySecretArmy.text = "";
        countryIncomeTurns.text = "";
        countryIncome.text = "";
    }
}
