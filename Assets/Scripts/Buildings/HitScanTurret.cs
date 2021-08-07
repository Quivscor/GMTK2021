using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitScanTurret : BaseTurret
{
    #region TurretStats
    public float TimeBetweenShots { get; protected set; }
    private float m_TimeBetweenShotsCurrent;
    public float BaseDamage { get; protected set; }

    private float m_ExtraDamage;
    private float m_ExtraTimeBetweenShotsReduction;
    #endregion

    protected override void Start()
    {
        base.Start();

        BaseDamage = BaseStats.power;
        TimeBetweenShots = BaseStats.frequency;

        m_TimeBetweenShotsCurrent = TimeBetweenShots;
    }

    protected override void Update()
    {
        base.Update();

        SanityTargetList();

        if (Targets.Count > 0)
        {
            if (m_TimeBetweenShotsCurrent <= 0)
            {
                if(Fire())
                    m_TimeBetweenShotsCurrent = TimeBetweenShots - m_ExtraTimeBetweenShotsReduction;
            }
            else
                m_TimeBetweenShotsCurrent -= Time.deltaTime;
        }
    }

    protected override void SetBuildingCustomStats()
    {
        m_ExtraDamage = BonusStats.power;
        m_ExtraTimeBetweenShotsReduction = BonusStats.frequency / (32 + BonusStats.frequency);
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
            Targets[0].TakeDamage(BaseDamage + m_ExtraDamage);
            return true;
        }
        else return false;
    }

    public override string ShowInfo()
    {
        string info = base.ShowInfo();

        info += "Damage = " + (BaseDamage + m_ExtraDamage) + "\nFire rate = " + (TimeBetweenShots - m_ExtraTimeBetweenShotsReduction) +
            "\nEnergy use per shot = " + (BaseStats.electricUsage + BonusStats.electricUsage) + "\nCurrent energy = " + Energy + "/" + MaxEnergy;

        return info;
    }
}
