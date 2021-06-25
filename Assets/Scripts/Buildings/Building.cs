using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Building : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private BuildingType m_BuildingType;
    public BuildingType BuildingType { get => m_BuildingType; }

    [SerializeField] private BuildingStats m_BaseStats;
    public BuildingStats BaseStats { get => m_BaseStats; protected set => m_BaseStats = value; }
    public BuildingStats BonusStats { get; protected set; }

    public bool isDirty = false;
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

    public virtual void AddBoostValue(BuildingStats s)
    {
        BonusStats += s;
    }

    public virtual void AddBoostMultiplier(BuildingStats s)
    {
        BonusStats += (BaseStats + BonusStats) * s;
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
