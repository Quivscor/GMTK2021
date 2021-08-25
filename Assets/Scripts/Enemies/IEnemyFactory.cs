using System.Collections;
using System.Collections.Generic;
using System;

public delegate void FactoryEvent();
public delegate void FactoryCreateEvent(Enemy e);

public interface IEnemyFactory
{
    EnemyPath EnemyPath { get; }

    IEnumerator CreateWave(WaveData data);

    bool CreateEnemy(WaveData data);

    event FactoryCreateEvent OnEnemySpawned;
    event FactoryEvent OnWaveFinishSpawning;
}

public enum FactoryType
{
    POINT_BASED,
    WAVE_DATA_BASED,
}