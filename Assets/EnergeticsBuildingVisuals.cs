using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnergeticsBuildingVisuals : MonoBehaviour
{
    [SerializeField] private GameObject m_RangeDisplay;
    public GameObject RangeDisplay => m_RangeDisplay;

    private void Awake()
    {
        Building building = GetComponent<Building>();
        building.OnBuildingSelected += ShowRange;
        building.OnBuildingDeselected += HideRange;
    }

    private void Start()
    {
        float size = EnergeticsController.ConnectionDistance * 2;
        RangeDisplay.transform.localScale = new Vector3(size, size, size);
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
