using System;

[Flags]
public enum BuildingType
{
    UNKNOWN = 0,
    TURRET = 1,
    ADDMODULE = 2,
    MULTMODULE = 4,
    HQ = 8,
    POWERSOURCE = 16,
    POWERLINE = 32,
}
