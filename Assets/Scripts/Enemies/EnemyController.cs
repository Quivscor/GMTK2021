using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class EnemyController : MonoBehaviour
{
    [Header("Enemy Factory Settings")]
    [SerializeField] private List<Enemy> m_EnemyTypes;
    public List<Enemy> EnemyTypes { get => m_EnemyTypes; }
    public IEnemyFactory EnemyFactory { get; private set; }

    public ResourcesController ResourcesController { get; private set; }

    [Header("Enemy Wave Settings")]
    [SerializeField] private float m_TimeBetweenWaves;
    public float TimeBetweenWaves { get => m_TimeBetweenWaves; private set => m_TimeBetweenWaves = value; }
    private float m_TimeToNextWave;
    private bool m_IsWaveSpawned;
    public Action<WaveData> OnNewWave;

    public int WaveNumber { get; private set; } = 0;

    private void Start()
    {
        if (EnemyTypes.Count <= 0)
            Debug.LogError("No enemy types detected in EnemyController!");

        EnemyFactory = new PointBasedEnemyFactory(EnemyTypes, 0);

        EnemyFactory.OnEnemySpawned += SubscribeEnemy;
        EnemyFactory.OnWaveFinishSpawning += HandleFinishWaveSpawn;

        ResourcesController = FindObjectOfType<ResourcesController>();

        HandleFinishWaveSpawn();
    }


    void Update()
    {
        m_TimeToNextWave -= Time.deltaTime;

        if(m_TimeToNextWave <= 0 && m_IsWaveSpawned)
        {
            CallNextWave();
        }
    }

    private void CallNextWave()
    {
        WaveNumber++;
        m_IsWaveSpawned = false;
        OnNewWave?.Invoke(new WaveData(WaveNumber));

        StartCoroutine(EnemyFactory.CreateWave(new WaveData(WaveNumber)));
    }

    public void HandleFinishWaveSpawn()
    {
        //time to next wave is time between waves + time when last guy reaches start
        m_TimeToNextWave = TimeBetweenWaves;
        m_IsWaveSpawned = true;
    }

    public void SubscribeEnemy(Enemy e)
    {
        e.OnDeath += DisposeEnemy;
        e.OnDeath += ResourcesController.GainMoney;

        e.OnReachDestination += ResourcesController.RemoveLife;
        e.OnReachDestination += DisposeEnemy;
    }

    private void DisposeEnemy(EnemyEventData e)
    {
        Destroy(e.Enemy.GetComponentInChildren<SpriteRenderer>());
        Destroy(e.Enemy.gameObject, .2f);
        Destroy(e.Enemy);
    }
}
