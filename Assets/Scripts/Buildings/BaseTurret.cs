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

    public event TurretFireEvent OnTurretFire;
    public event ReceiveEnergyEvent OnReceiveEnergy;

    protected override void Start()
    {
        base.Start();

        m_NetworkNeighbours = new HashSet<IPathfindingNode>();

        m_TurretEnemyDetector = GetComponentInChildren<TurretEnemyDetector>();

        m_TurretEnemyDetector.OnEnemyEnterRange += AddTarget;
        m_TurretEnemyDetector.OnEnemyExitRange += RemoveTarget;

        Targets = new List<Enemy>();
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
            return true;
        }
        else return false;
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
    }

    public bool TryConsumeEnergy(float energy)
    {
        if (Energy < energy)
            return false;
        else
            Energy -= energy;

        return true;
    }

    public override string ShowInfo()
    {
        string info = "Basic turret.\n";

        return info;
    }
}
