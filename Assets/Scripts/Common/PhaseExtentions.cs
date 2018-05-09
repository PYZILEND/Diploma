using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Phase { Guerrila, Battle, Recruitment, SecretAllies }

public class PhaseExtentions
{
    public Phase GetNextPhase(Phase phase)
    {
        if (phase != Phase.SecretAllies)
        {
            return phase + 1;
        }
        else
        {
            return Phase.Guerrila;
        }
    }
}