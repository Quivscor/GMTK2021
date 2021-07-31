using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControllerAudioInterface : AudioInterface
{
    [SerializeField] private string OnBuildingPlacedClipId;
    [SerializeField] private string OnWaveStartClipId;

    private void Awake()
    {
        GridController gridController = GetComponent<GridController>();
        gridController.OnProcessBuildPlacement += PlayPlacementSound;

        EnemyController enemyController = GetComponent<EnemyController>();
        enemyController.OnNewWave += PlayWaveStartSound;
    }

    public void PlayPlacementSound()
    {
        PlaySound(OnBuildingPlacedClipId);
    }

    public void PlayWaveStartSound(WaveData e)
    {
        PlaySound(OnWaveStartClipId);
    }
}
