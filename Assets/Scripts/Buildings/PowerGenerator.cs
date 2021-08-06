using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class PowerGenerator : PowerLine, IEnergetics, IGenerator
{
    private float m_EnergyParticleGenerationCooldown;
    private float m_EnergyParticleGenerationCooldownCurrent;

    private float m_ParticleValue;

    public int ConsumerCount { get; set; }

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
        m_EnergyParticleGenerationCooldown = 10f / Mathf.Log(BaseStats.frequency + BonusStats.frequency, 2);
        m_ParticleValue = EnergyParticle.DefaultEnergyValue + (BaseStats.power + BonusStats.power) / 10f;
    }

    public void Generate(IPathfindingNode origin)
    {
        int iterator;

        if (EnergeticsController.MultipleParticleGeneration)
            iterator = ConsumerCount;
        else
            iterator = 1;

        for(int i = 0; i < iterator; i++)
        {
            EnergyParticle particle = new EnergyParticle(EnergyParticleValue, origin);

            OnGenerate?.Invoke(new GeneratorEventData(particle, this));
        }
        
    }

    public override string ShowInfo()
    {
        string info = "Power generator.\nEnergy per particle = " + m_ParticleValue + "\nParticle rate = " + m_EnergyParticleGenerationCooldown +
            "s per particle";

        return info;
    }
}
