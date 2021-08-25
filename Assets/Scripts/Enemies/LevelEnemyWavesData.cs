using UnityEngine;

[CreateAssetMenu(fileName = "EnemyWaveData", menuName = "Level/Enemy Wave Data")]
public class LevelEnemyWavesData : ScriptableObject
{
    [System.Serializable]
    public class WaveData
    {
        public Enemy[] enemyTypes;
        public int[] enemyCounts;

        public float OverrideScaling;
    }
    public WaveData[] Waves;
}