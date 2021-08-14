using System.Collections.Generic;
using System;
using UnityEngine;

public class Enemy : MonoBehaviour, INodeTraverser
{
    public int pointCost = 1; //default to 1, so any enemy costs at least something
    [SerializeField] private int money = 2;
    [HideInInspector] public int currentMoney;

    #region EnemyData
    [SerializeField] private float hp;
    private float currentHp;
    public int damage;
    [SerializeField] private float perWaveExponent = 0.05f;
    #endregion

    #region TraversalData
    public float CurrentTime { get; private set; } = 0;
    public IPathfindingNode NextNode { get; set; }
    public IPathfindingNode CurrentNode { get; set; }
    public Stack<IPathfindingNode> Path { get; protected set; }

    protected float m_CurrentTime;
    [SerializeField] protected float m_MoveSpeed = 1;
    public float SingleUnitTraversalTime => m_MoveSpeed;
    #endregion

    #region Events
    public Action<EnemyEventData> OnReachDestination;
    public Action<EnemyEventData> OnTakeDamage;
    public Action<EnemyEventData> OnDeath;
    public Action<EnemyEventData> OnSpawn;
    #endregion

    public void Construct(IPathfindingNode start, int waveNumber, Stack<IPathfindingNode> path)
    {
        currentHp = hp + (Mathf.Pow(waveNumber, perWaveExponent));
        currentMoney = money + (int)(1 * Mathf.Log(waveNumber, 5));

        Path = path;
        CurrentNode = start;
        NextNode = Path.Pop();
    }

    private void Update()
    {
        if (Path == null)
        {
            Debug.LogError("Enemy has no path!");
            return;
        }

        if (NextNode == null)
        {
            //no more to travel through
            if (Path.Count == 0)
            {
                OnReachDestination?.Invoke(new EnemyEventData(this));
                return;
            }
            else
            {
                NextNode = Path.Pop();
            }
        }
        MoveBetweenNodes(CurrentNode, NextNode, Time.deltaTime);
    }

    public void TakeDamage(float damage)
    {
        currentHp -= damage;
        OnTakeDamage?.Invoke(new EnemyEventData());

        if (currentHp <= 0)
            Die();
    }

    public void Die()
    {
        OnDeath?.Invoke(new EnemyEventData(this));
    }

    public void MoveBetweenNodes(IPathfindingNode current, IPathfindingNode next, float deltaTime)
    {
        m_CurrentTime += deltaTime;
        float nodeDistance = Vector3.Distance(current.TransformReference.position, next.TransformReference.position);
        if (m_CurrentTime >= SingleUnitTraversalTime * nodeDistance)
        {
            this.transform.position = next.TransformReference.position;
            CurrentNode = NextNode;
            NextNode = null;
            m_CurrentTime = 0;
        }
        else
            this.transform.position = Vector3.Lerp(current.TransformReference.position, next.TransformReference.position, m_CurrentTime / (SingleUnitTraversalTime * nodeDistance));
    }
}