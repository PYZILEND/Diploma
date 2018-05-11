using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Used to display information about country in UI
/// </summary>
public class CountryInfo : MonoBehaviour {

    //Links to UI elements
    public Text countryName;
    public Text countryTreasury;
    public Text countryGuerrilla;
    public Text countrySecretArmy;
    public Text countryIncomeTurns;
    public Text countryIncome;

    /// <summary>
    /// Used to show information about specified country
    /// </summary>
    /// <param name="country"></param>
    public void ShowCountryInfo(Country country)
    {
        countryName.text = country.countryName;
        countryTreasury.text = country.treasury.ToString();
        countryGuerrilla.text = country.guerilla.ToString();
        countrySecretArmy.text = country.secretArmy.ToString();
        countryIncomeTurns.text = country.incomeTurnsLeft.ToString();
        countryIncome.text = CountryTypeExtentions.GetIncome(country.type).ToString();
    }

    /// <summary>
    /// Used to clear info panel's UI fields
    /// </summary>
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
