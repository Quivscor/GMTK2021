using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretVisuals : MonoBehaviour
{
    [SerializeField] private ParticleSystem m_OnFireParticles;
    public ParticleSystem OnFireParticles => m_OnFireParticles;

    [SerializeField] private GameObject m_RangeDisplay;
    public GameObject RangeDisplay => m_RangeDisplay;

    private void Awake()
    {
        ITurret turret = GetComponent<ITurret>();
        turret.OnTurretFire += PlayFireParticles;

        Building building = GetComponent<Building>();
        building.OnBuildingSelected += ShowRange;
        building.OnBuildingDeselected += HideRange;
    }

    public void PlayFireParticles()
    {
        OnFireParticles.Play();
    }

    public void ShowRange()
    {
        RangeDisplay.SetActive(true);
    }

    public void HideRange()
    {
        RangeDisplay.SetActive(false);
    }
}
