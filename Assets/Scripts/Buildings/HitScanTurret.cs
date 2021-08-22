using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitScanTurret : BaseTurret
{
    #region TurretStats
    private float m_TimeBetweenShots;
    public float TimeBetweenShots => m_TimeBetweenShots;
    private float m_TimeBetweenShotsCurrent;
    public float BaseDamage { get; protected set; }

    private float m_Damage;
    #endregion

    [SerializeField] private float m_BonusPowerModifier;
    [SerializeField] private float m_BonusFrequencyModifier;

    public override void Construct()
    {
        base.Construct();
        BaseDamage = BaseStats.power;
        m_TimeBetweenShots = BaseStats.frequency / BaseStats.frequency;

        m_TimeBetweenShotsCurrent = TimeBetweenShots;
    }

    protected override void Update()
    {
        if (!isBuilt || Time.timeScale == 0)
            return;

        base.Update();

        if (IsRecharging())
            return;

        SanityTargetList();

        if (Targets.Count > 0)
        {
            if (m_TimeBetweenShotsCurrent <= 0)
            {
                if(Fire())
                    m_TimeBetweenShotsCurrent = TimeBetweenShots;
            }
            else
                m_TimeBetweenShotsCurrent -= Time.deltaTime;
        }
    }

    protected override void SetBuildingCustomStats()
    {
        base.SetBuildingCustomStats();
        m_Damage = BaseStats.power + (BonusStats.power * m_BonusPowerModifier);
        m_TimeBetweenShots = BaseStats.frequency / (BaseStats.frequency + (BonusStats.frequency * m_BonusFrequencyModifier));
    }

    public void SanityTargetList()
    {
        foreach(Enemy e in new List<Enemy>(Targets))
        {
            if (e == null)
                RemoveTarget(e);
        }
    }

    public override bool Fire()
    {
        if (base.Fire())
        {
            Targets[0].TakeDamage(m_Damage);
            return true;
        }
        else return false;
    }

    public override string GetPersonalizedStatsString()
    {
        string info = base.GetPersonalizedStatsString();

        info += "Damage = " + (m_Damage) + "\nFire rate = " + (TimeBetweenShots) +
            "\nEnergy use per shot = " + (BaseStats.electricUsage + BonusStats.electricUsage) + "\nCurrent energy = " + Energy + "/" + MaxEnergy;

        return info;
    }
}
