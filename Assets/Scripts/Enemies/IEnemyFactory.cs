using System.Collections;
using System.Collections.Generic;
using System;

public delegate void FactoryEvent();
public delegate void FactoryCreateEvent(Enemy e);

public interface IEnemyFactory
{
    List<Enemy> EnemyTypes { get;}
    EnemyPath EnemyPath { get; }

    IEnumerator CreateWave(WaveData data);

    bool CreateEnemy(WaveData data);

    event FactoryCreateEvent OnEnemySpawned;
    event FactoryEvent OnWaveFinishSpawning;
}