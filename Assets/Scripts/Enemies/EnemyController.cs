using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class EnemyController : MonoBehaviour
{
    public static EnemyController Instance;

    public EnemyPath EnemyPath { get; private set; }
    public EnemyFactory EnemyFactory { get; private set; }
    public ResourcesController ResourcesController { get; private set; }
    private AudioSource source;

    public List<Enemy> SpawnedEnemies { get; private set; }

    public float timeBetweenWaves;
    public float timeToNextWave;
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

        //this works assuming that first 2 nodes are in a straight line
        EnemyFactory = new EnemyFactory(EnemyPath.PathNodes[0].transform.position, 
            EnemyPath.PathNodes[1].transform.position - EnemyPath.PathNodes[0].transform.position);

        ResourcesController = FindObjectOfType<ResourcesController>();
        source = GetComponent<AudioSource>();
        
        timeToNextWave = timeBetweenWaves;
    }


    void Update()
    {
        timeToNextWave -= Time.deltaTime;

        if(timeToNextWave <= 0)
        {
            CallNextWave();
        }
    }

    private void CallNextWave()
    {
        WaveNumber++;
        OnNewWave?.Invoke(WaveNumber);
        source.PlayOneShot(source.clip);
        SpawnedEnemies = EnemyFactory.CreateWave(GetAvailablePoints(), WaveNumber);
        //time to next wave is time between waves + time when last guy reaches start
        timeToNextWave = timeBetweenWaves - SpawnedEnemies[SpawnedEnemies.Count - 1].CurrentTime;

        SubscribeEnemies();
    }

    private void SubscribeEnemies()
    {
        foreach(Enemy e in SpawnedEnemies)
        {
            e.OnDeath += DisposeEnemy;
            e.OnDeath += ResourcesController.GainMoney;

            e.OnReachDestination += ResourcesController.RemoveLife;
            e.OnReachDestination += DisposeEnemy;
        }
    }

    private void DisposeEnemy(Enemy e)
    {
        SpawnedEnemies.Remove(e);
        Destroy(e.GetComponentInChildren<SpriteRenderer>());
        Destroy(e.gameObject, .2f);
    }

    private int GetAvailablePoints()
    {
        return (int)(WaveNumber * 3.5f);
    }

    public Vector3 GetPositionOfNodeAt(int i)
    {
        return EnemyPath.PathNodes[i].transform.position;
    }
}
