using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(BoxCollider2D))]
public class Building : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private BuildingType m_BuildingType;
    public BuildingType BuildingType { get => m_BuildingType; }

    [SerializeField] private BuildingStats m_BaseStats;
    public BuildingStats BaseStats { get => m_BaseStats; protected set => m_BaseStats = value; }
    public BuildingStats BonusStats { get; protected set; }

    public Action OnBuildingStatsUpdated;

    public bool isDirty = false;
    public bool isBuilt = false;
    public bool isPowered = false;

    protected virtual void Start()
    {
        BonusStats = new BuildingStats();
    }

    protected virtual void Update()
    {
        if (!isBuilt || !isPowered || Time.timeScale == 0)
            return;
    }

    public void ResetBuildingBonuses()
    {
        BonusStats.Reset();
    }

    public virtual void AddBoostValue(BuildingStats s)
    {
        BonusStats += s;
        OnBuildingStatsUpdated?.Invoke();
    }

    public virtual void AddBoostMultiplier(BuildingStats s)
    {
        BonusStats += (BaseStats + BonusStats) * s;
        OnBuildingStatsUpdated?.Invoke();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        SelectionManager.Select(this);
    }

    public virtual string ShowInfo()
    {
        throw new System.NotImplementedException();
    }
}
