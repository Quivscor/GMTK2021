using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Turret : Building
{
    [SerializeField] private GameObject m_TurretHead;

    private TurretEnemyDetector m_TurretEnemyDetector;
    private ParticleSystem fireParticles;
    private AudioSource source;

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
        m_TurretEnemyDetector = GetComponentInChildren<TurretEnemyDetector>();
        fireParticles = GetComponentInChildren<ParticleSystem>();
        source = GetComponent<AudioSource>();

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

    protected override void Update()
    {
        if (!isBuilt || Time.timeScale == 0/*|| !isPowered*/) //power to be reintroduced later
            return;

        SanityTargetList();

        if (targets.Count > 0)
        {
            m_TurretHead.transform.right = -1 * (targets[0].transform.position - this.transform.position);

            if (timeBetweenShotsCurrent <= 0)
            {
                extraDamage = BonusStats.power;
                extraTimeBetweenShotsReduction = BonusStats.frequency / (32 + BonusStats.frequency);
                Fire();
                timeBetweenShotsCurrent = timeBetweenShots - extraTimeBetweenShotsReduction;
            }
            else
                timeBetweenShotsCurrent -= Time.deltaTime;
        }
    }

    public void SanityTargetList()
    {
        foreach(Enemy e in new List<Enemy>(targets))
        {
            if (e == null)
                RemoveTarget(e);
        }
    }

    public void Fire()
    {
        targets[0].TakeDamage(baseDamage + extraDamage);
        fireParticles.Play();
        source.PlayOneShot(source.clip);
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
