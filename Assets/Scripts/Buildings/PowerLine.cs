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

    protected override void Start()
    {
        base.Start();
        m_NetworkNeighbours = new HashSet<IPathfindingNode>();
    }

    protected override bool IsBuildingRecharging()
    {
        if(base.IsBuildingRecharging())
        {
            m_IsWalkable = false;
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
