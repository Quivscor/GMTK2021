using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoneyGenerator : PowerLine, IActiveBuilding, IGenerator
{
    public int ConsumerCount { get; set; }

    protected float m_Energy;
    public float Energy => m_Energy;

    [SerializeField] protected float m_MaxEnergy = 100f;
    public float MaxEnergy => m_MaxEnergy;
    protected EnergyAnalytics m_Analytics;
    public EnergyAnalytics Analytics => m_Analytics;

    protected float m_GeneratedMoney;
    protected float m_GenerationCooldown;
    public float GenerationCooldown => m_GenerationCooldown;
    protected float m_GenerationCooldownCurrent;
    public float GenerationCooldownCurrent => m_GenerationCooldownCurrent;
    [SerializeField] protected string m_GeneratedObjectName;
    public string GeneratedObjectName => (m_GeneratedObjectName + " (" + m_GeneratedMoney.ToString("F2") + ")");

    [SerializeField] private float m_BonusPowerModifier;
    [SerializeField] private float m_BonusFrequencyModifier;

    public event GenerateEvent OnGenerate;
    public event GenerationProgressEvent OnGenerationProgress;
    public event EnergyEvent OnReceiveEnergy;
    public event EnergyEvent OnUseEnergy;

    public override void Construct()
    {
        base.Construct();

        m_Analytics = new EnergyAnalytics(5f);
    }

    protected override void SetBuildingCustomStats()
    {
        base.SetBuildingCustomStats();
        m_GeneratedMoney = BaseStats.power + BonusStats.power * m_BonusPowerModifier;
        m_GenerationCooldown = BaseStats.frequency / ((BaseStats.frequency / 10) + (BonusStats.frequency * m_BonusFrequencyModifier));
    }

    protected override void Update()
    {
        if (!isBuilt || Time.timeScale == 0)
            return;

        m_Analytics.MeasureTime(Time.deltaTime);

        if (IsRecharging())
            return;

        if (m_GenerationCooldownCurrent <= 0)
        {
            if(TryConsumeEnergy(BaseStats.electricUsage + BonusStats.electricUsage))
                Generate();
        }
        else
        {
            m_GenerationCooldownCurrent -= Time.deltaTime;
            OnGenerationProgress?.Invoke();
        }
    }

    public void AddEnergy(float energy)
    {
        m_Energy += energy;
        if(m_Energy > MaxEnergy)
        {
            m_Energy = MaxEnergy;
        }
        OnReceiveEnergy?.Invoke();
        m_Analytics.MeasureEnergyGain(energy);
    }

    public void Generate()
    {
        OnGenerate?.Invoke(new GeneratorEventData(m_GeneratedMoney));

        m_GenerationCooldownCurrent = m_GenerationCooldown;
    }

    public bool TryConsumeEnergy(float energy)
    {
        if (m_Energy < energy)
            return false;

        m_Energy -= energy;
        OnUseEnergy?.Invoke();
        return true;
    }

    public override string GetPersonalizedStatsString()
    {
        string info = base.GetPersonalizedStatsString(); 
        info += "\nGenerates " + Mathf.RoundToInt(m_GeneratedMoney) + " money every " + m_GenerationCooldown + "s\nCosts " + 
            (BaseStats.electricUsage + BonusStats.electricUsage) + " energy to run";
        info += "\nCurrent energy = " + Energy + "/" + MaxEnergy;

        return info;
    }
}
