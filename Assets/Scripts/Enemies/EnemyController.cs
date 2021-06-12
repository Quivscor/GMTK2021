using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public static EnemyController Instance;

    public EnemyPath EnemyPath { get; private set; }
    public EnemyFactory EnemyFactory { get; private set; }
    
    public List<Enemy> SpawnedEnemies { get; private set; }

    public float timeBetweenWaves;
    public float timeToNextWave;

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
        SpawnedEnemies = EnemyFactory.CreateWave(GetAvailablePoints());
        timeToNextWave = timeBetweenWaves;

        SubscribeEnemies();
    }

    private void SubscribeEnemies()
    {
        foreach(Enemy e in SpawnedEnemies)
        {
            e.OnDeath += DisposeEnemy;

        }
    }

    private void DisposeEnemy(Enemy e)
    {
        SpawnedEnemies.Remove(e);
        Destroy(e.gameObject);
    }

    private int GetAvailablePoints()
    {
        return WaveNumber * 5;
    }

    public Vector3 GetPositionOfNodeAt(int i)
    {
        return EnemyPath.PathNodes[i].transform.position;
    }
}
