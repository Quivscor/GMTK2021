using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseTurret : Building, ITurret
{
    protected TurretEnemyDetector m_TurretEnemyDetector;

    public List<Enemy> Targets { get; private set; }

    protected HashSet<IPathfindingNode> m_NetworkNeighbours;
    public HashSet<IPathfindingNode> NetworkNeighbours => m_NetworkNeighbours;

    protected bool m_IsWalkable = true;
    public bool IsWalkable => m_IsWalkable;
    protected IPathfindingNode m_Parent;
    public IPathfindingNode Parent { get => m_Parent; set => m_Parent = value; }
    public Transform TransformReference => this.transform;

    public float Energy { get; protected set; }
    [SerializeField] protected float m_MaxEnergy;
    public float MaxEnergy => m_MaxEnergy;
    protected EnergyAnalytics m_Analytics;
    public EnergyAnalytics Analytics => m_Analytics;

    protected float m_ConnectionDamageModifier;
    public float ConnectionDamageModifier => m_ConnectionDamageModifier;

    protected float m_ConnectionSpeedModifier;
    public float ConnectionSpeedModifier => m_ConnectionSpeedModifier;

    public event TurretFireEvent OnTurretFire;
    public event EnergyEvent OnReceiveEnergy;
    public event EnergyEvent OnUseEnergy;
    public event RechargingEvent OnEnterRechargingState;

    public override void Construct()
    {
        base.Construct();

        m_NetworkNeighbours = new HashSet<IPathfindingNode>();

        m_TurretEnemyDetector = GetComponentInChildren<TurretEnemyDetector>();

        m_TurretEnemyDetector.OnEnemyEnterRange += AddTarget;
        m_TurretEnemyDetector.OnEnemyExitRange += RemoveTarget;

        Targets = new List<Enemy>();
        m_Analytics = new EnergyAnalytics(5f);
    }

    protected override void Update()
    {
        m_Analytics.MeasureTime(Time.deltaTime);
    }

    public void AddTarget(Enemy e)
    {
        Targets.Add(e);
    }

    public void RemoveTarget(Enemy e)
    {
        Targets.Remove(e);
    }

    public virtual bool Fire()
    {
        float electricUsage = BaseStats.electricUsage + BonusStats.electricUsage;
        if (TryConsumeEnergy(electricUsage))
        {
            OnTurretFire?.Invoke();
            OnUseEnergy?.Invoke();
            return true;
        }
        else return false;
    }

    public override bool IsRecharging()
    {
        bool result = base.IsRecharging();
        if (result)
            OnEnterRechargingState?.Invoke(new BuildingEventData(this, this as IPathfindingNode));

        return result;
    }

    public void AddNetworkNeighbour(IPathfindingNode node)
    {
        NetworkNeighbours.Add(node);
    }

    public void RemoveNetworkNeighbour(IPathfindingNode node)
    {
        NetworkNeighbours.Remove(node);
    }

    public void AddEnergy(float energy)
    {
        Energy += energy;
        if (Energy > MaxEnergy)
            Energy = MaxEnergy;

        OnReceiveEnergy?.Invoke();
        m_Analytics.MeasureEnergyGain(energy);
    }

    public bool TryConsumeEnergy(float energy)
    {
        if (Energy < energy)
            return false;
        else
            Energy -= energy;

        return true;
    }

    protected override void SetBuildingCustomStats()
    {
        m_ConnectionDamageModifier = BonusStats.power;
        m_ConnectionSpeedModifier = BonusStats.frequency;
    }

    public override string GetPersonalizedStatsString()
    {
        string info = "Basic turret.\n";

        return info;
    }
}
