using System;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable, CreateAssetMenu(fileName = "BuildingData", menuName = "Building Data")]
public class BuildingConnectionData : ScriptableObject
{
    public BuildingType type;

    public BuildingStats connectionBoost;

    public BuildingType connectingTypes;

    public bool isBoostAdditive;
}

[Flags]
public enum BuildingType
{
    UNKNOWN = 0,
    TURRET = 1,
    PWR = 2,
    FREQ = 4,
    ECO = 8,
    MULT = 16,
    POWER_SUPPLY = 32,
    POWER_CONNECTOR = 64,
    HQ = 128,
}