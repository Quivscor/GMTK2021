public interface INodeTraverser
{
    public IPathfindingNode NextNode { get; set; }
    public IPathfindingNode CurrentNode { get; set; }

    public float SingleUnitTraversalTime { get; }

    public void MoveBetweenNodes(IPathfindingNode current, IPathfindingNode next, float deltaTime);
}