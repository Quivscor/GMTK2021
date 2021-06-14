using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class EnemyController : MonoBehaviour
{
    public static EnemyController Instance;

    public EnemyPath EnemyPath { get; private set; }

    [Header("Enemy Factory Settings")]
    [SerializeField] private List<Enemy> m_EnemyTypes;
    public List<Enemy> EnemyTypes { get => m_EnemyTypes; }
    public EnemyFactory EnemyFactory { get; private set; }

    public ResourcesController ResourcesController { get; private set; }
    private AudioSource source;

    [Header("Enemy Wave Settings")]
    [SerializeField] private float m_TimeBetweenWaves;
    public float TimeBetweenWaves { get => m_TimeBetweenWaves; private set => m_TimeBetweenWaves = value; }
    private float m_TimeToNextWave;
    private bool m_IsWaveSpawned;
    public Action<int> OnNewWave;

    public int WaveNumber { get; private set; } = 0;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
    }

    private void Start()
    {
        EnemyPath = new EnemyPath();
        EnemyPath.ConstructPath();

        if (EnemyTypes.Count <= 0)
            Debug.LogError("No enemy types detected in EnemyController!");
        EnemyFactory = new EnemyFactory(EnemyPath.PathNodes[0].transform.position, EnemyTypes);

        EnemyFactory.OnEnemySpawned += SubscribeEnemy;
        EnemyFactory.OnWaveFinishSpawning += HandleFinishWaveSpawn;

        ResourcesController = FindObjectOfType<ResourcesController>();
        source = GetComponent<AudioSource>();

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
        OnNewWave?.Invoke(WaveNumber);

        StartCoroutine(EnemyFactory.CreateWave(WaveNumber));

        source.PlayOneShot(source.clip);
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

    private void DisposeEnemy(Enemy e)
    {
        Destroy(e.GetComponentInChildren<SpriteRenderer>());
        Destroy(e.gameObject, .2f);
    }

    public Vector3 GetPositionOfNodeAt(int i)
    {
        return EnemyPath.PathNodes[i].transform.position;
    }
}
