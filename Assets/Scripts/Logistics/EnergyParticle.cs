using System.Collections.Generic;
using System;
using UnityEngine;

public class EnergyParticle : INodeTraverser
{
    public static readonly float DefaultEnergyValue = 10.0f;
    public static readonly float DefaultTraversalTime = .5f;

    private readonly float m_EnergyValue;
    public float EnergyValue => m_EnergyValue;

    public Vector3 Position { get; protected set; }

    public Stack<IPathfindingNode> Path { get; private set; }
    public IPathfindingNode CurrentNode { get; set; }
    public IPathfindingNode NextNode { get; set; }
    public IActiveBuilding Target { get; set; }
    public float SingleUnitTraversalTime => DefaultTraversalTime;
    protected float m_CurrentTraversalTime;

    public Action<EnergyParticleEventData> OnReachDestination;
    public Action<EnergyParticleEventData> OnNoAvailablePath;

    public EnergyParticle()
    {
        m_EnergyValue = DefaultEnergyValue;
    }

    public EnergyParticle(IPathfindingNode origin)
    {
        m_EnergyValue = DefaultEnergyValue;

        //set start node to generation origin
        CurrentNode = origin;
        Position = origin.TransformReference.position;
    }

    public EnergyParticle(float value)
    {
        m_EnergyValue = value;
    }

    public EnergyParticle(float value, IPathfindingNode origin)
    {
        m_EnergyValue = value;

        //set start node to generation origin
        CurrentNode = origin;
        Position = origin.TransformReference.position;
    }

    public void Construct(Stack<IPathfindingNode> path)
    {
        Path = path;
    }

    public void UpdatePath(Stack<IPathfindingNode> path)
    {
        Path = path;
    }

    public void Update(float deltaTime)
    {
        if (Path == null)
        {
            OnNoAvailablePath?.Invoke(new EnergyParticleEventData(this));
            return;
        }

        if (NextNode == null)
        {
            //no more to travel through
            if (Path.Count == 0)
            {
                OnReachDestination?.Invoke(new EnergyParticleEventData(this, Target));
                return;
            }    
            else
            {
                //if last node in path, get the target reference to power up the building
                if (Path.Count == 1)
                    Target = Path.Peek() as IActiveBuilding;
                NextNode = Path.Pop();
            }
                
        }    

        MoveBetweenNodes(CurrentNode, NextNode, deltaTime);
    }

    public void MoveBetweenNodes(IPathfindingNode current, IPathfindingNode next, float deltaTime)
    {
        m_CurrentTraversalTime += deltaTime;
        float travelTime = GetTravelTimeBetweenNodes(current, next);
        float nodeDistance = Vector3.Distance(current.TransformReference.position, next.TransformReference.position);
        if (m_CurrentTraversalTime >= travelTime * nodeDistance)
        {
            Position = next.TransformReference.position;
            CurrentNode = NextNode;
            NextNode = null;
            m_CurrentTraversalTime = 0;
        }
        else
            Position = Vector3.Lerp(current.TransformReference.position, next.TransformReference.position, m_CurrentTraversalTime / (travelTime * nodeDistance));
    }

    private float GetTravelTimeBetweenNodes(IPathfindingNode current, IPathfindingNode next)
    {
        float result = SingleUnitTraversalTime;

        float extraFrequency = (next as IEnergetics).ConnectionSpeedModifier;

        result = SingleUnitTraversalTime / (1 + Mathf.Log(1 + extraFrequency, 5));

        return result;
    }
}