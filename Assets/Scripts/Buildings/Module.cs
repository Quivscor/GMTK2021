using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Module : Building, IModule
{
    [SerializeField] private BuildingConnectionData m_ConnectionData;
    public BuildingConnectionData ConnectionData => m_ConnectionData;
}
