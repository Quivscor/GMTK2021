using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPathfinder
{
    public Stack<IPathfindingNode> FindPath(IPathfindingNode startNode, IPathfindingNode endNode);
}
