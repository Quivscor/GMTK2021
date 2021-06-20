using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Building : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private BuildingType m_BuildingType;
    public BuildingType BuildingType { get => m_BuildingType; }

    [SerializeField] private BuildingStats m_BaseStats;
    public BuildingStats BaseStats { get => m_BaseStats; private set => m_BaseStats = value; }
    public BuildingStats BonusStats { get; private set; }

    public bool isDirty = true;
    public bool isBuilt = false;
    public bool isPowered = false;

    protected virtual void Start()
    {
        BonusStats = new BuildingStats();
    }

    protected virtual void Update()
    {
        if (!isBuilt || !isPowered)
            return;
    }

    public void ResetBuildingBonuses()
    {
        BonusStats.Reset();
    }

    public void AddBoostValue(BuildingStats s)
    {
        BonusStats += s;
    }

    public void AddBoostMultiplier(BuildingStats s)
    {
        BonusStats += BonusStats * s;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        SelectionManager.Select(this);
    }
}
