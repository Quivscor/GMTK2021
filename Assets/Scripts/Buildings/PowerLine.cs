using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerLine : Building, IEnergetics
{
    protected HashSet<IPathfindingNode> m_NetworkNeighbours;
    public HashSet<IPathfindingNode> NetworkNeighbours => m_NetworkNeighbours;
    protected bool m_IsWalkable = true;
    public bool IsWalkable => m_IsWalkable;
    protected IPathfindingNode m_Parent;
    public IPathfindingNode Parent { get => m_Parent; set => m_Parent = value; }
    public Transform TransformReference => this.transform;

    public event RechargingEvent OnEnterRechargingState;

    protected float m_ConnectionDamageModifier;
    public float ConnectionDamageModifier => m_ConnectionDamageModifier;

    protected float m_ConnectionSpeedModifier;
    public float ConnectionSpeedModifier => m_ConnectionSpeedModifier;

    protected override void Update()
    {
        if (!isBuilt || Time.timeScale == 0)
            return;

        if (IsRecharging())
            return;
    }

    public override void Construct()
    {
        base.Construct();
        m_NetworkNeighbours = new HashSet<IPathfindingNode>();
    }

    public override bool IsRecharging()
    {
        if(base.IsRecharging())
        {
            m_IsWalkable = false;
            OnEnterRechargingState?.Invoke(new BuildingEventData(this, this as IPathfindingNode));
            return true;
        }
        else
        {
            m_IsWalkable = true;
            return false;
        }
    }

    public void AddNetworkNeighbour(IPathfindingNode node)
    {
        NetworkNeighbours.Add(node);
    }

    public void RemoveNetworkNeighbour(IPathfindingNode node)
    {
        NetworkNeighbours.Remove(node);
    }

    protected override void SetBuildingCustomStats()
    {
        base.SetBuildingCustomStats();
        m_ConnectionDamageModifier = BonusStats.power;
        m_ConnectionSpeedModifier = BonusStats.frequency;
    }

    public override string GetPersonalizedStatsString()
    {
        string info = "Connects your active buildings to the electric network";

        return info;
    }

    protected void OnDrawGizmos()
    {
        if (m_NetworkNeighbours == null)
            return;

        foreach (IEnergetics e in m_NetworkNeighbours)
        {
            MonoBehaviour mb = e as MonoBehaviour;
            Gizmos.color = Color.red;
            Gizmos.DrawLine(this.transform.position, mb.transform.position);
        }
    }
}
