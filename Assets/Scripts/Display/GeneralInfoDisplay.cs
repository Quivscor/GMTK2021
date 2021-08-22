using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMProText = TMPro.TextMeshProUGUI;

public class GeneralInfoDisplay : MonoBehaviour
{
    [SerializeField] private TMProText livesText;
    [SerializeField] private TMProText waveText;

    private void Awake()
    {
        ResourcesController r = FindObjectOfType<ResourcesController>();
        r.OnLivesChange += UpdateLives;
        EnemyController e = FindObjectOfType<EnemyController>();
        e.OnNewWave += UpdateWave;
    }

    public void UpdateWave(WaveData e)
    {
        UpdateText(waveText, "Wave: ", e.WaveNumber);
    }

    public void UpdateLives(int value)
    {
        UpdateText(livesText, "Lives: ", value);
    }

    public void UpdateText(TMProText text, string entryString, int value)
    {
        text.text = entryString + value;
    }
}
