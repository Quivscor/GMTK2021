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

    [SerializeField] private GameObject m_TurretHead;
    public GameObject TurretHead => m_TurretHead;

    private List<Enemy> m_TargetListRef;

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
        ITurret turret = GetComponent<ITurret>();
        m_TargetListRef = turret.Targets;

        float size = EnergeticsController.ConnectionDistance * 2;
        EnergeticsRangeDisplay.transform.localScale = new Vector3(size, size, size);
    }

    private void LateUpdate()
    {
        if(TurretHead != null)
            if(m_TargetListRef.Count > 0 && m_TargetListRef[0] != null)
                TurretHead.transform.right = -1 * (m_TargetListRef[0].transform.position - this.transform.position);
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
