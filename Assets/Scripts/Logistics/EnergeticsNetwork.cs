using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class EnergeticsNetwork
{
    public HashSet<IEnergetics> Nodes { get; private set; }
    public HashSet<IGenerator> Origins { get; private set; }
    public HashSet<IActiveBuilding> Consumers { get; private set; }
    private int m_ConsumerIterator = 0;

    private AStarPathfinder pathFinder;

    public List<EnergyParticle> Particles { get; private set; }

    public EnergeticsNetwork() 
    {
        Init();
    }

    public EnergeticsNetwork(IEnergetics node)
    {
        Init();
        AddNode(node);
    }

    public EnergeticsNetwork(List<IEnergetics> nodes)
    {
        Init();
        foreach (IEnergetics node in nodes)
            AddNode(node);
    }

    private void Init()
    {
        Nodes = new HashSet<IEnergetics>();
        Origins = new HashSet<IGenerator>();
        Consumers = new HashSet<IActiveBuilding>();
        Particles = new List<EnergyParticle>();

        pathFinder = new AStarPathfinder();
    }

    public void AddNode(IEnergetics node)
    {
        if (node is IGenerator generator)
            AddOrigin(generator);
        else if (node is IActiveBuilding active)
            Consumers.Add(active);

        Nodes.Add(node);
    }

    public void AddOrigin(IGenerator origin)
    {
        Origins.Add(origin);
        origin.OnGenerate += AddParticle;
    }

    public void AddParticle(GeneratorEventData e)
    {
        if (Consumers.Count == 0)
            return;

        Particles.Add(e.GeneratedObject as EnergyParticle);

        if (m_ConsumerIterator >= Consumers.Count - 1)
            m_ConsumerIterator = 0;
        else
            m_ConsumerIterator++;

        List<IPathfindingNode> pathfindingNodes = Nodes.ToList<IPathfindingNode>();
        Particles[Particles.Count - 1].Construct(pathFinder.FindPath(e.GenerationOrigin, Consumers.ElementAt(m_ConsumerIterator), pathfindingNodes));
        Particles[Particles.Count - 1].OnNoAvailablePath += RemoveParticle;
        Particles[Particles.Count - 1].OnReachDestination += ConsumeParticle;
    }

    //remove particle that can't traverse the network anymore
    public void RemoveParticle(EnergyParticleEventData e)
    {
        Particles.Remove(e.Particle);
        Debug.Log("Removed particle");
    }

    //deal with particle that reached it's destination
    public void ConsumeParticle(EnergyParticleEventData e)
    {
        e.Target.AddEnergy(e.Particle.EnergyValue);
        RemoveParticle(e);
        Debug.Log("Consumed particle");
    }

    public void UpdateNetwork(float deltaTime)
    {
        //copy list to avoid collection changed errors
        List<EnergyParticle> particles = new List<EnergyParticle>(Particles);
        foreach (EnergyParticle p in particles)
        {
            p.Update(deltaTime);
        }
    }

    public static EnergeticsNetwork MergeNetworks(EnergeticsNetwork networkA, EnergeticsNetwork networkB)
    {
        EnergeticsNetwork network = new EnergeticsNetwork();

        networkA.Nodes.UnionWith(networkB.Nodes);
        networkA.Origins.UnionWith(networkB.Origins);
        networkA.Consumers.UnionWith(networkB.Consumers);

        network.Nodes = networkA.Nodes;
        network.Origins = networkA.Origins;
        network.Consumers = networkA.Consumers;
        network.Particles = MergeLists(networkA.Particles, networkB.Particles);

        foreach(IGenerator origin in network.Origins)
        {
            origin.OnGenerate -= networkA.AddParticle;
            origin.OnGenerate -= networkB.AddParticle;

            origin.OnGenerate += network.AddParticle;
        }

        foreach(EnergyParticle particle in network.Particles)
        {
            particle.OnNoAvailablePath -= networkA.RemoveParticle;
            particle.OnReachDestination -= networkA.ConsumeParticle;
            particle.OnNoAvailablePath -= networkB.RemoveParticle;
            particle.OnReachDestination -= networkB.ConsumeParticle;

            particle.OnNoAvailablePath += network.RemoveParticle;
            particle.OnReachDestination += network.ConsumeParticle;
        }

        return network;
    }

    //Concats 2 lists without duplicates
    private static List<T> MergeLists<T>(List<T> listA, List<T> listB)
    {
        HashSet<T> set = new HashSet<T>();
        foreach(T t in listA)
        {
            set.Add(t);
        }
        foreach(T t in listB)
        {
            set.Add(t);
        }
        return set.ToList();
    }
}