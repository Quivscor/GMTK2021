using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class PowerGenerator : Building, IEnergetics, IGenerator
{
    private float m_EnergyParticleGenerationCooldown;
    private float m_EnergyParticleGenerationCooldownCurrent;

    private float m_ParticleValue;

    public float EnergyParticleGenerationCooldown => m_EnergyParticleGenerationCooldown;
    public float EnergyParticleValue => m_ParticleValue;

    private HashSet<IPathfindingNode> m_NetworkNeighbours;
    public HashSet<IPathfindingNode> NetworkNeighbours => m_NetworkNeighbours;
    protected bool m_IsWalkable = true;
    public bool IsWalkable => m_IsWalkable;
    protected IPathfindingNode m_Parent;
    public IPathfindingNode Parent { get => m_Parent; set => m_Parent = value; }
    public Transform TransformReference => this.transform;

    public event GenerateEvent OnGenerate;

    protected override void Start()
    {
        base.Start();
        SetBuildingCustomStats();

        //Power buildings themselves should not absorb energy
        isPowered = true;

        OnBuildingStatsUpdated += SetBuildingCustomStats;

        m_NetworkNeighbours = new HashSet<IPathfindingNode>();
        m_EnergyParticleGenerationCooldownCurrent = m_EnergyParticleGenerationCooldown;
    }

    protected override void Update()
    {
        base.Update();

        if(m_EnergyParticleGenerationCooldownCurrent <= 0)
        {
            Generate(this);
            m_EnergyParticleGenerationCooldownCurrent = EnergyParticleGenerationCooldown;
        }
        else
        {
            m_EnergyParticleGenerationCooldownCurrent -= Time.deltaTime;
        }
    }

    protected void SetBuildingCustomStats()
    {
        m_EnergyParticleGenerationCooldown = 20f / (BaseStats.frequency + BonusStats.frequency);
        m_ParticleValue = (BaseStats.power + BonusStats.power) / 10f;
    }

    public void Generate(IPathfindingNode origin)
    {
        EnergyParticle particle = new EnergyParticle(origin);

        OnGenerate?.Invoke(new GeneratorEventData(particle, this));
    }

    public void AddNetworkNeighbour(IPathfindingNode node)
    {
        NetworkNeighbours.Add(node as IEnergetics);
    }

    public void RemoveNetworkNeighbour(IPathfindingNode node)
    {
        NetworkNeighbours.Remove(node as IEnergetics);
    }

    private void OnDrawGizmos()
    {
        if (m_NetworkNeighbours == null)
            return;

        foreach(IEnergetics e in m_NetworkNeighbours)
        {
            MonoBehaviour mb = e as MonoBehaviour;
            Gizmos.color = Color.red;
            Gizmos.DrawLine(this.transform.position, mb.transform.position);
        }
    }
}
