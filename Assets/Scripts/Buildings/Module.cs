using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Module : Building, IModule
{
    [SerializeField] private BuildingConnectionData m_ConnectionData;
    public BuildingConnectionData ConnectionData => m_ConnectionData;

    [SerializeField] private int m_SortingOrder;

    public override void AddBoostMultiplier(BuildingStats s)
    {
        BonusStats += (ConnectionData.ConnectionBoost + BonusStats) * s;
    }

    //modules should never recharge
    public override bool IsRecharging()
    {
        return false;
    }

    public override int BuildingComparator()
    {
        return m_SortingOrder;
    }

    public override string GetPersonalizedStatsString()
    {
        string result = "";
        string symbol = "";
        if (BuildingType == BuildingType.MULTMODULE)
            symbol += "x";

        if (ConnectionData.ConnectionBoost.power != 0)
            result += "Extra power in cluster: " + symbol + ConnectionData.ConnectionBoost.power + "\n";
        if (ConnectionData.ConnectionBoost.frequency != 0)
            result += "Extra speed in cluster: " + symbol + ConnectionData.ConnectionBoost.frequency + "\n";
        if (ConnectionData.ConnectionBoost.electricUsage != 0)
            result += "Extra energy use in cluster: " + symbol + ConnectionData.ConnectionBoost.electricUsage + "\n";
        if (ConnectionData.ConnectionBoost.resistance != 0)
            result += "Extra resistance in cluster: " + symbol + ConnectionData.ConnectionBoost.resistance + "\n";

        return result;
    }
}
