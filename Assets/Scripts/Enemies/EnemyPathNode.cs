using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPathNode : MonoBehaviour, IEnemyPathNode
{
    [SerializeField] private EnemyNodeType m_NodeType;
    public EnemyNodeType NodeType => m_NodeType;

    [SerializeField] private int m_PathIndex;
    public int PathIndex => m_PathIndex;

    protected HashSet<IPathfindingNode> m_NetworkNeighbours;
    public HashSet<IPathfindingNode> NetworkNeighbours => m_NetworkNeighbours;
    [SerializeField] private EnemyPathNode m_NextNode;

    public Transform TransformReference => this.transform;

    public bool IsWalkable => true;

    public IPathfindingNode Parent { get; set; }

    private void Awake()
    {
        m_NetworkNeighbours = new HashSet<IPathfindingNode>();
        if(m_NextNode != null)
            m_NetworkNeighbours.Add(m_NextNode);
    }

    public void AddNetworkNeighbour(IPathfindingNode node)
    {
        NetworkNeighbours.Add(node);
    }

    public void RemoveNetworkNeighbour(IPathfindingNode node)
    {
        NetworkNeighbours.Remove(node);
    }
}
