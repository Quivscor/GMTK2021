using UnityEngine;

public interface IHeuristic
{
    public float Evaluate<T>(T n, T endGoal) where T : IPathfindingNode;
}