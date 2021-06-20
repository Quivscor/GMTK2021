using System;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class BuildingConnectionData
{
    [SerializeField] private BuildingStats m_ConnectionBoost;
    public BuildingStats ConnectionBoost => m_ConnectionBoost;

    [SerializeField] private BuildingType m_ConnectingTypes;
    public BuildingType ConnectingTypes => m_ConnectingTypes;

    [SerializeField] private bool m_IsBoostAdditive;
    public bool IsBoostAdditive => m_IsBoostAdditive;
}