using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitScanTurret : Building, ITurret
{
    [SerializeField] private GameObject m_TurretHead;

    private TurretEnemyDetector m_TurretEnemyDetector;
    private ParticleSystem fireParticles;
    private AudioSource source;

    public List<Enemy> Targets { get; private set; }

    public event TurretFireEvent OnTurretFire;

    #region TurretBaseStats
    private float timeBetweenShots;
    private float timeBetweenShotsCurrent;
    private float baseDamage;
    #endregion

    #region TurretExtraStats
    private float extraDamage;
    private float extraTimeBetweenShotsReduction;
    #endregion

    private void Awake()
    { 
        m_TurretEnemyDetector = GetComponentInChildren<TurretEnemyDetector>();
        fireParticles = GetComponentInChildren<ParticleSystem>();
        source = GetComponent<AudioSource>();

        m_TurretEnemyDetector.OnEnemyEnterRange += AddTarget;
        m_TurretEnemyDetector.OnEnemyExitRange += RemoveTarget;

        Targets = new List<Enemy>();
    }

    protected override void Start()
    {
        base.Start();

        baseDamage = BaseStats.power;
        timeBetweenShots = BaseStats.frequency;

        timeBetweenShotsCurrent = timeBetweenShots;
    }

    protected override void Update()
    {
        if (!isBuilt || Time.timeScale == 0/*|| !isPowered*/) //power to be reintroduced later
            return;

        SanityTargetList();

        if (Targets.Count > 0)
        {
            m_TurretHead.transform.right = -1 * (Targets[0].transform.position - this.transform.position);

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
        foreach(Enemy e in new List<Enemy>(Targets))
        {
            if (e == null)
                RemoveTarget(e);
        }
    }

    public void Fire()
    {
        Targets[0].TakeDamage(baseDamage + extraDamage);
        OnTurretFire?.Invoke();
        fireParticles.Play();
        source.PlayOneShot(source.clip);
    }

    public void AddTarget(Enemy e)
    {
        Targets.Add(e);
    }

    public void RemoveTarget(Enemy e)
    {
        Targets.Remove(e);
    }
}
