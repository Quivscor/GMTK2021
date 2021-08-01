using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitScanTurret : BaseTurret
{
    [SerializeField] private GameObject m_TurretHead;

    #region TurretBaseStats
    private float timeBetweenShots;
    private float timeBetweenShotsCurrent;
    private float baseDamage;
    #endregion

    #region TurretExtraStats
    private float extraDamage;
    private float extraTimeBetweenShotsReduction;
    #endregion

    protected override void Start()
    {
        base.Start();

        baseDamage = BaseStats.power;
        timeBetweenShots = BaseStats.frequency;

        timeBetweenShotsCurrent = timeBetweenShots;
    }

    protected override void Update()
    {
        base.Update();

        SanityTargetList();

        if (Targets.Count > 0)
        {
            m_TurretHead.transform.right = -1 * (Targets[0].transform.position - this.transform.position);

            if (timeBetweenShotsCurrent <= 0)
            {
                extraDamage = BonusStats.power;
                extraTimeBetweenShotsReduction = BonusStats.frequency / (32 + BonusStats.frequency);
                if(Fire())
                    timeBetweenShotsCurrent = timeBetweenShots - extraTimeBetweenShotsReduction;
            }
            else
                timeBetweenShotsCurrent -= Time.deltaTime;
        }
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
            Targets[0].TakeDamage(baseDamage + extraDamage);
            return true;
        }
        else return false;
    }

    
}
