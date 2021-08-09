using System.Linq;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class EnemyPath
{
    public EnemyPathNode StartNode { get; private set; }
    public EnemyPathNode[] PathNodes { get; private set; }
    public EnemyPathNode EndNode { get; private set; }

    private IPathfinder m_Pathfinder;

    public EnemyPath()
    {
        m_Pathfinder = new AStarPathfinder();
    }

    //TODO: find a way to call this in constructor
    public void ConstructPath(int pathIndex)
    {
        PopulateNodeList(pathIndex);
    }

    public Stack<IPathfindingNode> GetPath()
    {
        return m_Pathfinder.FindPath(StartNode, EndNode);
    }

    private void PopulateNodeList(int pathIndex)
    {
        //find all objects and keep only those that belong in specific path
        List<EnemyPathNode> nodes = GameObject.FindObjectsOfType<EnemyPathNode>().ToList();
        nodes = nodes.FindAll(x => x.PathIndex == pathIndex);

        PathNodes = nodes.ToArray();
        foreach(EnemyPathNode node in PathNodes)
        {
            if (node.NodeType == EnemyNodeType.START)
                StartNode = node;
            else if (node.NodeType == EnemyNodeType.END)
                EndNode = node;
        }
    }
}