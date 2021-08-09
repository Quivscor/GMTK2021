using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AStarPathfinder : IPathfinder
{
    public IHeuristic heuristic;

    public AStarPathfinder()
    {
        heuristic = new ManhattanHeuristic();
    }

    public Stack<IPathfindingNode> FindPath(IPathfindingNode startNode, IPathfindingNode endNode)
    {
        Stack<IPathfindingNode> path = new Stack<IPathfindingNode>();
        List<IPathfindingNode> openList = new List<IPathfindingNode>();
        List<IPathfindingNode> closedList = new List<IPathfindingNode>();

        IPathfindingNode current = startNode;
        openList.Add(current);

        while (openList.Count != 0 && !closedList.Contains(endNode))
        {
            current = openList[0];
            openList.Remove(current);
            closedList.Add(current);

            foreach(IPathfindingNode node in current.NetworkNeighbours)
            {
                if(!closedList.Contains(node) && node.IsWalkable)
                {
                    if(!openList.Contains(node))
                    {
                        node.Parent = current;
                        openList.Add(node);
                        openList.OrderBy(n => heuristic.Evaluate(n, endNode));
                    }
                }
            }
        }

        if (!closedList.Contains(endNode))
            return null;

        IPathfindingNode temp = closedList[closedList.IndexOf(current)];
        if (temp == null)
            return null;

        do
        {
            path.Push(temp);
            temp = temp.Parent;
        }
        while (temp != startNode && temp != null);

        return path;
    }
}
