using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelDataBasedEnemyFactory : IEnemyFactory
{
    [SerializeField] private LevelEnemyWavesData m_Data;
    public LevelEnemyWavesData Data => m_Data; 

    public EnemyPath EnemyPath { get; protected set; }
    public Vector3 SpawnPoint { get; protected set; }

    public event FactoryCreateEvent OnEnemySpawned;
    public event FactoryEvent OnWaveFinishSpawning;

    public LevelDataBasedEnemyFactory(LevelEnemyWavesData wavesData)
    {
        m_Data = wavesData;

        EnemyPath = new EnemyPath();
        EnemyPath.ConstructPath(0);
        SpawnPoint = EnemyPath.StartNode.transform.position;
    }

    int m_CurrentTypeIndex;
    int m_WaveNumber;

    public bool CreateEnemy(WaveData data)
    {
        Enemy enemy = GameObject.Instantiate(m_Data.Waves[m_WaveNumber - 1].enemyTypes[m_CurrentTypeIndex], 
            SpawnPoint, Quaternion.identity, null);


        if(m_Data.Waves[m_WaveNumber - 1].OverrideScaling != 0)
        {
            enemy.Construct(EnemyPath.StartNode, m_WaveNumber, m_Data.Waves[m_WaveNumber - 1].OverrideScaling, EnemyPath.GetPath());
        }
        else
        {
            //scale enemy with wave number
            enemy.Construct(EnemyPath.StartNode, m_WaveNumber, EnemyPath.GetPath());
        }

        //subscribe enemy to handlers for enemy death and enemy reaching base
        OnEnemySpawned?.Invoke(enemy);
        return true;
    }

    public IEnumerator CreateWave(WaveData data)
    {
        m_WaveNumber = data.WaveNumber;

        //if wave number is higher than data anticipates, start looping
        while(m_WaveNumber > m_Data.Waves.Length)
        {
            m_WaveNumber -= m_Data.Waves.Length;
        }

        for (int i = 0; i < m_Data.Waves[m_WaveNumber - 1].enemyTypes.Length; i++)
        {
            m_CurrentTypeIndex = i;
            for(int j = 0; j < m_Data.Waves[m_WaveNumber - 1].enemyCounts[i]; j++)
            {
                if (CreateEnemy(data))
                {
                    yield return new WaitForSeconds(m_Data.Waves[m_WaveNumber - 1].enemyTypes[i].GetSpawnTime());
                }
                else
                    Debug.LogError("Failed to create enemy!");
            }
        }

        OnWaveFinishSpawning?.Invoke();
    }
}
