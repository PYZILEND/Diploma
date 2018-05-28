using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

/// <summary>
/// Used to create objects that represent unit types
/// </summary>
[CreateAssetMenu(fileName = "unit", menuName = "UnitType")]
public class UnitType : ScriptableObject {

    public string unitName;

    public byte maxHealth;
    public byte maxMovePoints;

    public byte attackPower;
    public byte attackRange;

    public byte cost;

    public TravelType travelType;

    public bool canEmbark;
    public bool canGoOverUnits;

    //For special units
    public byte capacity;
    public byte secondAttackPower;

    public float travelSpeed = 2f;
}
