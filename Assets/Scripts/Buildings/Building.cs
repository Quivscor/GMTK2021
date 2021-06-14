using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Building : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private BuildingStats m_BaseStats;
    public BuildingStats BaseStats { get => m_BaseStats; private set => m_BaseStats = value; }
    [SerializeField] private BuildingStats m_BonusStats;
    public BuildingStats BonusStats { get => m_BonusStats; private set => m_BonusStats = value; }

    public BuildingConnectionData BuildingData;

    public bool isDirty = true;
    public bool isBuilt = false;
    public bool isPowered = false;

    protected virtual void Awake()
    {
        ResetBonusStats();
    }

    public void ResetBonusStats()
    {
        BonusStats = BuildingData.connectionBoost;
    }

    protected virtual void Update()
    {
        if (!isBuilt || !isPowered)
            return;
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
