using UnityEngine;

public class ManhattanHeuristic : IHeuristic
{
    public float Evaluate<T>(T n, T endNode) where T : IPathfindingNode
    {
        float result = default;

        result = Vector3.Distance(n.TransformReference.position, endNode.TransformReference.position);

        return result;
    }
}