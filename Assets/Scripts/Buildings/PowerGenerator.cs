using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class PowerGenerator : PowerLine, IEnergetics, IGenerator
{
    [SerializeField] private float m_BonusPowerModifier;
    private float m_EnergyParticleGenerationCooldown;
    public float GenerationCooldown => m_EnergyParticleGenerationCooldown;
    private float m_EnergyParticleGenerationCooldownCurrent;
    public float GenerationCooldownCurrent => m_EnergyParticleGenerationCooldownCurrent;
    [SerializeField] protected string m_GeneratedObjectName;
    public string GeneratedObjectName => (m_GeneratedObjectName + " (" + m_ParticleValue.ToString("F2") + "kW)");

    private float m_ParticleValue;

    public int ConsumerCount { get; set; }

    public float EnergyParticleGenerationCooldown => m_EnergyParticleGenerationCooldown;
    public float EnergyParticleValue => m_ParticleValue;

    public event GenerateEvent OnGenerate;
    public event GenerationProgressEvent OnGenerationProgress;

    public override void Construct()
    {
        base.Construct();

        m_NetworkNeighbours = new HashSet<IPathfindingNode>();
        m_EnergyParticleGenerationCooldownCurrent = m_EnergyParticleGenerationCooldown;
    }

    protected override void Update()
    {
        if (!isBuilt || Time.timeScale == 0)
            return;

        if (IsRecharging())
            return;

        if (m_EnergyParticleGenerationCooldownCurrent <= 0)
        {
            Generate();
            m_EnergyParticleGenerationCooldownCurrent = EnergyParticleGenerationCooldown;
        }
        else
        {
            m_EnergyParticleGenerationCooldownCurrent -= Time.deltaTime;
            OnGenerationProgress?.Invoke();
        }
    }

    protected override void SetBuildingCustomStats()
    {
        base.SetBuildingCustomStats();
        m_EnergyParticleGenerationCooldown = BaseStats.frequency / (BaseStats.frequency + BonusStats.frequency);
        m_ParticleValue = BaseStats.power + (BonusStats.power * m_BonusPowerModifier);
    }

    public void Generate()
    {
        int iterator;

        if (EnergeticsController.MultipleParticleGeneration)
            iterator = ConsumerCount;
        else
            iterator = 1;

        for(int i = 0; i < iterator; i++)
        {
            EnergyParticle particle = new EnergyParticle(EnergyParticleValue, this);

            OnGenerate?.Invoke(new GeneratorEventData(particle, this));
        }
        
    }

    public override string GetPersonalizedStatsString()
    {
        string info = "Power generator.\nEnergy per particle = " + m_ParticleValue + "\nParticle rate = " + m_EnergyParticleGenerationCooldown +
            "s per particle";

        return info;
    }
}
