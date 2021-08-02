using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Module : Building, IModule
{
    [SerializeField] private BuildingConnectionData m_ConnectionData;
    public BuildingConnectionData ConnectionData => m_ConnectionData;

    public override void AddBoostMultiplier(BuildingStats s)
    {
        BonusStats += (BaseStats + ConnectionData.ConnectionBoost + BonusStats) * s;
    }

    //modules should never recharge
    protected override bool IsBuildingRecharging()
    {
        return false;
    }

    public override string ShowInfo()
    {
        string symbol = "+";
        if (!ConnectionData.IsBoostAdditive)
            symbol = "x";
        string info = "Standard issue module.\nPower: " + symbol + (BaseStats.power + BonusStats.power + ConnectionData.ConnectionBoost.power) +
            "\nFrequency: " + symbol + (BaseStats.frequency + BonusStats.frequency + ConnectionData.ConnectionBoost.frequency);

        return info;
    }
}
