﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CountryControls : MonoBehaviour {

    public static Country selectedCountry;
    static UnitType selectedUnit;    

    /// <summary>
    /// Processes player's input
    /// </summary>
    /// <param name="cell"></param>
    public static void ProcessInput(LogicalMapCell cell)
    {
        if (cell.isSelectable || cell.canPlaceUnit)
        {
            if (cell.canPlaceUnit)
            {
                PurchaseUnit(cell);
            }

            if (selectedCountry == null || cell.country != selectedCountry)
            {
                SelectCountry(cell);
            }
        }
    }

    /// <summary>
    /// Drops previous selection and selects new country
    /// </summary>
    /// <param name="cell"></param>
    static void SelectCountry(LogicalMapCell cell)
    {
        if (selectedCountry)
        {
            DropSelection();
        }
        selectedCountry = cell.country;
        selectedCountry.MakeUnselectable();
        selectedCountry.ValidatePossibleUnitPlacements(selectedUnit);
        if(GameMaster.turnPhase == Phase.SecretAllies)
        {
            MakeAllUnselectable();
            selectedCountry.Disclosure();
        }
    }

    /// <summary>
    /// Deselects currently selected country
    /// </summary>
    public static void DropSelection()
    {
        selectedCountry.DropUnitPlacements();
        ValidateSelectableCountry(selectedCountry);
        selectedCountry = null;       
    }

    /// <summary>
    /// Must be called when game changes phase
    /// </summary>
    public static void ChangePhase()
    {
        if (selectedCountry)
        {
            selectedCountry.DropUnitPlacements();
            selectedCountry = null;
        }
        ValidateSelectableCountryes();
    }

    /// <summary>
    /// Purchases currently selected unit at specified cell
    /// </summary>
    /// <param name="cell"></param>
    public static void PurchaseUnit(LogicalMapCell cell)
    {
        switch (GameMaster.turnPhase)
        {
            case Phase.Guerrila:
                selectedCountry.BuyWithGuerrilla(selectedUnit, cell);
                break;

            case Phase.Recruitment:
                selectedCountry.BuyWithTresury(selectedUnit, cell);
                break;

            case Phase.SecretAllies:
                selectedCountry.BuyWithSecretArmy(selectedUnit, cell);
                break;
        }
    }

    /// <summary>
    /// Validate if country can be selected
    /// </summary>
    /// <param name="country"></param>
    public static void ValidateSelectableCountry(Country country)
    {
        switch (GameMaster.turnPhase)
        {
            case Phase.Guerrila:
                if (country.willSpawnGuerrilla)
                {
                    country.MakeSelectable();
                }
                else
                {
                    country.MakeUnselectable();
                }                
                break;

            case Phase.Recruitment:
                if (country.CanBuyUnits)
                {
                    country.MakeSelectable();
                }
                else
                {
                    country.MakeUnselectable();
                }                
                break;

            case Phase.SecretAllies:
                if (country.CanBeDisclosured)
                {
                    country.MakeSelectable();
                }
                else
                {
                    country.MakeUnselectable();
                }                
                break;

            case Phase.Battle:
                country.MakeUnselectable();
                break;
        }
    }

    /// <summary>
    /// Validates all countryes for being able to select them
    /// </summary>
    /// <param name="allegiance"></param>
    public static void ValidateSelectableCountryes()
    {
        foreach (Country country in GameMaster.countries)
        {
            ValidateSelectableCountry(country);
        }
    }

    /// <summary>
    /// Renders all countryes unselectable
    /// </summary>
    static void MakeAllUnselectable()
    {
        foreach (Country country in GameMaster.countries)
        {
            country.MakeUnselectable();
        }
    }

    /// <summary>
    /// Recieves selected unit from UI
    /// </summary>
    /// <param name="value"></param>
    public void GetSelectedUnit(int value)
    {
        selectedUnit = (UnitType)value;
        if (selectedCountry)
        {
            selectedCountry.ValidatePossibleUnitPlacements(selectedUnit);
        }
    }
}
