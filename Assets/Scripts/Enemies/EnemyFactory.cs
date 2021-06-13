using UnityEngine;
using System.Collections.Generic;

public class EnemyFactory
{
    public List<Enemy> enemies;
    public Vector3 spawnPoint;
    public Vector3 directionTowardsSpawnPoint;

    public float distanceOffsetBetweenWaveMembers = 0.75f;

    public EnemyFactory(Vector3 spawnPoint, Vector3 direction)
    {
        enemies = new List<Enemy>();
        this.spawnPoint = spawnPoint - direction;
        this.directionTowardsSpawnPoint = direction;

        Enemy e = Resources.Load<Enemy>("Enemies/TestEnemy");
        enemies.Add(e);
        e = Resources.Load<Enemy>("Enemies/TestEnemy2");
        enemies.Add(e);
    }

    public List<Enemy> CreateWave(int availablePoints, int waveNumber)
    {
        List<Enemy> wave = new List<Enemy>();
        while(availablePoints > 0)
        {
            Enemy e;
            if (TryCreateEnemy(ref availablePoints, out e, wave.Count, waveNumber))
            {
                wave.Add(e);
            }
        }
                
        return wave;
    }

    public bool TryCreateEnemy(ref int availablePoints, out Enemy enemy, int enemyInWaveIndex, int waveNumber)
    {
        List<Enemy> enemiesReversed = new List<Enemy>(enemies);
        enemiesReversed.Reverse();
        foreach(Enemy e in enemiesReversed)
        {
            if(e.pointCost <= availablePoints)
            {
                Enemy localEnemy = GameObject.Instantiate(e, spawnPoint + (enemyInWaveIndex * distanceOffsetBetweenWaveMembers * -1 * directionTowardsSpawnPoint) , Quaternion.identity, null);
                localEnemy.Construct(waveNumber);
                availablePoints -= e.pointCost;
                enemy = localEnemy;
                return true;
            }
        }
        enemy = null;
        return false;
    }
}