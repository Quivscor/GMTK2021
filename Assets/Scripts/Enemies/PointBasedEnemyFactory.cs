using UnityEngine;
using System.Collections.Generic;
using System;
using System.Collections;
using System.Linq;

public class PointBasedEnemyFactory : IEnemyFactory
{
    public List<Enemy> EnemyTypes { get; private set; }
    public EnemyPath EnemyPath { get; protected set; }
    public Vector3 SpawnPoint { get; private set; }

    private float m_TimeOffsetBetweenWaveMembers = 0.75f;
    private int m_AvailablePoints;

    public event FactoryEvent OnWaveFinishSpawning;
    public event FactoryCreateEvent OnEnemySpawned;

    public PointBasedEnemyFactory(List<Enemy> enemyTypes, int pathIndex)
    {
        EnemyTypes = new List<Enemy>();

        //constructing path per factory
        EnemyPath = new EnemyPath();
        EnemyPath.ConstructPath(pathIndex);
        SpawnPoint = EnemyPath.StartNode.transform.position;

        //most expensive enemy is first
        EnemyTypes = enemyTypes;
        EnemyTypes = EnemyTypes.OrderByDescending(i => i.pointCost).ToList<Enemy>();
    }

    public IEnumerator CreateWave(WaveData data)
    {
        m_AvailablePoints = GetAvailablePoints(data.WaveNumber);
        
        while(m_AvailablePoints > 0)
        {
            if (CreateEnemy(data))
            {
                yield return new WaitForSeconds(m_TimeOffsetBetweenWaveMembers);
            }
            else
            {
                //in case there aren't EnemyTypes of cost 1, fail safe option
                Debug.LogError("Create wave cannot spend all points, using fail safe option!");
                break;
            }
        }
        FinalizeWaveSpawn();
    }

    private int GetAvailablePoints(int waveNumber)
    {
        return (10 + (int)Mathf.Log(waveNumber * 2f, 2));
    }

    private void FinalizeWaveSpawn()
    {
        OnWaveFinishSpawning?.Invoke();
    }

    public bool CreateEnemy(WaveData data)
    {
        foreach(Enemy e in EnemyTypes)
        {
            if(e.pointCost <= m_AvailablePoints)
            {
                Enemy enemy = GameObject.Instantiate(e, SpawnPoint, Quaternion.identity, null);

                //scale enemy with wave number
                enemy.Construct(EnemyPath.StartNode, data.WaveNumber, EnemyPath.GetPath());
                m_AvailablePoints -= enemy.pointCost;

                //subscribe enemy to handlers for enemy death and enemy reaching base
                OnEnemySpawned?.Invoke(enemy);
                return true;
            }
        }
        return false;
    }
}