using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretVisuals : MonoBehaviour
{
    [SerializeField] private ParticleSystem m_OnFireParticles;
    public ParticleSystem OnFireParticles => m_OnFireParticles;

    [SerializeField] private GameObject m_RangeDisplay;
    public GameObject RangeDisplay => m_RangeDisplay;

    [SerializeField] private GameObject m_EnergeticsRangeDisplay;
    public GameObject EnergeticsRangeDisplay => m_EnergeticsRangeDisplay;

    private void Awake()
    {
        ITurret turret = GetComponent<ITurret>();
        turret.OnTurretFire += PlayFireParticles;

        Building building = GetComponent<Building>();
        building.OnBuildingSelected += ShowRange;
        building.OnBuildingDeselected += HideRange;
    }

    private void Start()
    {
        float size = EnergeticsController.ConnectionDistance * 2;
        EnergeticsRangeDisplay.transform.localScale = new Vector3(size, size, size);
    }

    public void PlayFireParticles()
    {
        OnFireParticles.Play();
    }

    public void ShowRange()
    {
        RangeDisplay.SetActive(true);
        EnergeticsRangeDisplay.SetActive(true);
    }

    public void HideRange()
    {
        RangeDisplay.SetActive(false);
        EnergeticsRangeDisplay.SetActive(false);
    }
}
