using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Turret : Building
{
    [SerializeField] private GameObject m_TurretHead;

    private TurretEnemyDetector m_TurretEnemyDetector;

    private List<Enemy> targets;

    #region TurretBaseStats
    private float timeBetweenShots;
    private float timeBetweenShotsCurrent;
    private float baseDamage;
    #endregion

    #region TurretExtraStats
    private float extraDamage;
    private float extraTimeBetweenShotsReduction;
    #endregion

    protected override void Awake()
    {
        base.Awake();

        m_TurretEnemyDetector = GetComponentInChildren<TurretEnemyDetector>();

        m_TurretEnemyDetector.OnEnemyEnterRange += AddTarget;
        m_TurretEnemyDetector.OnEnemyExitRange += RemoveTarget;

        targets = new List<Enemy>();
    }

    private void Start()
    {
        baseDamage = BaseStats.power;
        timeBetweenShots = BaseStats.frequency;

        timeBetweenShotsCurrent = timeBetweenShots;
    }

    private void Update()
    {
        if(isBuilt)
        {
            if (targets.Count > 0)
            {
                m_TurretHead.transform.up = targets[0].transform.position - this.transform.position;

                if (timeBetweenShotsCurrent <= 0)
                {
                    Fire();
                    timeBetweenShotsCurrent = timeBetweenShots - extraTimeBetweenShotsReduction;
                }
                else
                    timeBetweenShotsCurrent -= Time.deltaTime;
            }
        }
    }

    public void Fire()
    {
        targets[0].TakeDamage(baseDamage + extraDamage);
    }

    public void AddTarget(Enemy target)
    {
        targets.Add(target);
    }

    public void RemoveTarget(Enemy target)
    {
        targets.Remove(target);
    }
}
