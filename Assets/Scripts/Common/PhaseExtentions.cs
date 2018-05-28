using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Possible game phases
/// </summary>
public enum Phase { Guerrila, Battle, Recruitment, SecretAllies }

//This class should probably be reworked
public static class PhaseExtentions
{
    /// <summary>
    /// Checks current game state to verify next game phase
    /// Also gives turn to enemy player and checks if they lost game
    /// </summary>
    /// <param name="phase"></param>
    /// <returns></returns>
    public static Phase VerifyNextPhase(Phase phase, out bool opponentLost)
    {
        opponentLost = false;//Return that opponent didn't loose by default
        switch (phase)
        {
            case Phase.Guerrila:
                return Phase.Battle; //You always go to battle phase after guerrilla


            case Phase.Battle:
                bool canBuyUnits = false;
                //If can buy units go to recruitment phase
                foreach(Country country in GameMaster.countries)
                {
                    //Should find a better place for this
                    country.GetIncome();//Countryes must recieve income before they are checked
                    if (country.CanBuyUnits)
                    {
                        canBuyUnits = true;
                    }
                }
                if (canBuyUnits)
                {
                    return Phase.Recruitment;
                }
                //If can disclosure go to secret allies phase
                foreach (Country country in GameMaster.countries)
                {
                    if (country.CanBeDisclosured)
                    {
                        return Phase.SecretAllies;
                    }
                }
                //If none give turn to opponent and check his game phase
                return checkEnemyTurn(out opponentLost);


            case Phase.Recruitment:
                //If can disclosure go to secret allies phase
                foreach (Country country in GameMaster.countries)
                {
                    if (country.CanBeDisclosured)
                    {
                        return Phase.SecretAllies;
                    }
                }
                //If none give turn to opponent and check his game phase
                return checkEnemyTurn(out opponentLost);


            case Phase.SecretAllies:
                return checkEnemyTurn(out opponentLost);
        }
        return checkEnemyTurn(out opponentLost);
    }


    /// <summary>
    /// Changes game's turn and checks what game phase enemy will play
    /// Returns true for opponentLost parameter if enemy has nothing left to play with
    /// Also returns turn back if enemy lost
    /// </summary>
    /// <param name="opponentLost"></param>
    /// <returns></returns>
    static Phase checkEnemyTurn(out bool opponentLost)
    {
        opponentLost = false;
        //Change turn and check if enemy has guerrilla
        GameMaster.allegianceTurn = AllegianceExtentions.Opposite(GameMaster.allegianceTurn);
        foreach (Country country in GameMaster.countries)
        {
            if (country.willSpawnGuerrilla)
            {
                return Phase.Guerrila;
            }
        }
        //If enemy has any units
        foreach (Unit unit in GameMaster.units)
        {
            if (unit.allegiance == GameMaster.allegianceTurn)
            {
                return Phase.Battle;
            }
        }
        //If enemy can purchase any units
        foreach (Country country in GameMaster.countries)
        {
            //Should find a better place for this
            country.GetIncome(); //Countryes must recieve income before they are checked
            if (country.CanBuyUnits)
            {
                return Phase.Recruitment;
            }
        }
        //If enemy has secret allyes
        foreach (Country country in GameMaster.countries)
        {
            if (country.CanBeDisclosured)
            {
                return Phase.SecretAllies;
            }
        }
        //If nothing above applyes then enemy must have lost
        //We return turn back to victorious player
        opponentLost = true;
        GameMaster.allegianceTurn = AllegianceExtentions.Opposite(GameMaster.allegianceTurn);
        return Phase.Battle;
    }
}