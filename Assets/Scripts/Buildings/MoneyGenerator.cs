using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoneyGenerator : PowerLine, IActiveBuilding, IGenerator
{
    public int ConsumerCount { get; set; }

    protected float m_Energy;
    public float Energy => m_Energy;

    protected float m_MaxEnergy = 100f;
    public float MaxEnergy => m_MaxEnergy;

    protected float m_GeneratedMoney;
    protected float m_GenerationCooldown;
    protected float m_GenerationCooldownCurrent;

    public event GenerateEvent OnGenerate;
    public event EnergyEvent OnReceiveEnergy;
    public event EnergyEvent OnUseEnergy;

    protected override void SetBuildingCustomStats()
    {
        m_GeneratedMoney = 5 + Mathf.Log(BaseStats.power + BonusStats.power, 5);
        m_GenerationCooldown = 10 / (1 + Mathf.Log(BaseStats.frequency + BonusStats.frequency, 3));
    }

    protected override void Update()
    {
        base.Update();

        if (m_GenerationCooldownCurrent <= 0)
        {
            if(TryConsumeEnergy(BaseStats.electricUsage + BonusStats.electricUsage))
                Generate();
        }
        else
        {
            m_GenerationCooldownCurrent -= Time.deltaTime;
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

    public override string ShowInfo()
    {
        string info = base.ShowInfo(); 
        info += "\nGenerates " + Mathf.RoundToInt(m_GeneratedMoney) + " money every " + m_GenerationCooldown + "s\nCosts " + 
            (BaseStats.electricUsage + BonusStats.electricUsage) + " energy to run";
        info += "\nCurrent energy = " + Energy + "/" + MaxEnergy;

        return info;
    }
}
