using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class PowerGenerator : PowerLine, IEnergetics, IGenerator
{
    private float m_EnergyParticleGenerationCooldown;
    private float m_EnergyParticleGenerationCooldownCurrent;

    private float m_ParticleValue;

    public float EnergyParticleGenerationCooldown => m_EnergyParticleGenerationCooldown;
    public float EnergyParticleValue => m_ParticleValue;

    public event GenerateEvent OnGenerate;

    protected override void Start()
    {
        base.Start();

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

    protected override void SetBuildingCustomStats()
    {
        m_EnergyParticleGenerationCooldown = 20f / (BaseStats.frequency + BonusStats.frequency);
        m_ParticleValue = EnergyParticle.DefaultEnergyValue + (BaseStats.power + BonusStats.power) / 10f;
    }

    public void Generate(IPathfindingNode origin)
    {
        EnergyParticle particle = new EnergyParticle(EnergyParticleValue, origin);

        OnGenerate?.Invoke(new GeneratorEventData(particle, this));
    }
}
