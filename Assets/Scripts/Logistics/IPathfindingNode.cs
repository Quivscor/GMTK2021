using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPathfindingNode
{
    public HashSet<IPathfindingNode> NetworkNeighbours { get; }

    public void AddNetworkNeighbour(IPathfindingNode node);
    public void RemoveNetworkNeighbour(IPathfindingNode node);

    public Transform TransformReference { get; }

    public bool IsWalkable { get; }
    public IPathfindingNode Parent { get; set; }
}
