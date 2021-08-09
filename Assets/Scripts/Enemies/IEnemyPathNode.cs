using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IEnemyPathNode : IPathfindingNode
{
    public int PathIndex { get; }
}
