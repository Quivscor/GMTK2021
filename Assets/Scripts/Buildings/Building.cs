using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(BoxCollider2D))]
public class Building : MonoBehaviour, IPointerClickHandler
{
    //setting more than one causes problems with adjacency
    [SerializeField] private BuildingType m_BuildingType;
    public BuildingType BuildingType => m_BuildingType;

    [SerializeField] private BuildingStats m_BaseStats;
    public BuildingStats BaseStats { get => m_BaseStats; protected set => m_BaseStats = value; }
    public BuildingStats BonusStats { get; protected set; }
    [SerializeField] private GridFieldType m_AllowedPlacementFieldType;
    public GridFieldType AllowedPlacementFieldType => m_AllowedPlacementFieldType;

    //actual hp of building
    public float CurrentResistance { get; protected set; }
    //is building inactive and regenerating
    protected bool m_IsRecharging;
    private float m_CurrentRechargeTime;

    public Action OnBuildingSelected;
    public Action OnBuildingDeselected;
    public Action OnBuildingUpdated;
    public Action OnDamageReceived;
    //public Action<BuildingEventData> OnEnterRechargingState;

    [HideInInspector] public bool isDirty = false;
    [HideInInspector] public bool isBuilt = false;

    public virtual void Construct()
    {
        BonusStats = new BuildingStats();

        SetBuildingCustomStats();
        OnBuildingUpdated += SetBuildingCustomStats;
        CurrentResistance = BaseStats.resistance + BonusStats.resistance;
    }

    protected virtual void Update()
    {
        
    }

    public virtual bool IsRecharging()
    {
        float maxResistance = BaseStats.resistance + BonusStats.resistance;
        if (CurrentResistance <= 0 && !m_IsRecharging)
        {
            m_IsRecharging = true;
            m_CurrentRechargeTime = 0;
            OnBuildingUpdated?.Invoke();
            return true;
        }
        else if(m_IsRecharging)
        {
            m_CurrentRechargeTime += Time.deltaTime;
            float rechargeTime = BaseStats.rechargeRate + BonusStats.rechargeRate;
            if (m_CurrentRechargeTime >= rechargeTime)
            {
                m_IsRecharging = false;
                OnBuildingUpdated?.Invoke();
            }              
            CurrentResistance = Mathf.Lerp(0, maxResistance, m_CurrentRechargeTime / rechargeTime);
            return true;
        }
        return false;
    }
    
    public virtual bool IsActiveBuilding()
    {
        if (this is IModule module)
            return false;
        else
            return true;
    }

    public void ResetBuildingBonuses()
    {
        BonusStats.Reset();
        isDirty = true;
    }

    /// <summary>
    /// Use with caution, used to change building type for the purpose of connection boost checking
    /// </summary>
    public void DebugSetBaseStats(BuildingStats stats)
    {
        m_BaseStats = stats;
    }

    /// <summary>
    /// Use with caution, used to change building type for the purpose of connection boost checking
    /// </summary>
    public void DebugSetBuildingType(BuildingType type)
    {
        m_BuildingType = type;
    }

    protected virtual void SetBuildingCustomStats()
    {
        
    }

    public virtual void AddBoostValue(BuildingStats s)
    {
        BonusStats += s;
        OnBuildingUpdated?.Invoke();
    }

    public virtual void AddBoostMultiplier(BuildingStats s)
    {
        BonusStats += BonusStats * s;
        OnBuildingUpdated?.Invoke();
    }

    public virtual int BuildingComparator() { return 10000; }

    public virtual void OnPointerClick(PointerEventData eventData)
    {
        SelectionManager.Select(this);
    }

    public virtual void SelectBuilding()
    {
        OnBuildingSelected?.Invoke();
    }

    public virtual void DeselectBuilding()
    {
        OnBuildingDeselected?.Invoke();
    }

    public void ReceiveDamage(float value)
    {
        if (BaseStats.resistance == Mathf.Infinity)
            return;
        CurrentResistance -= value;
        OnDamageReceived?.Invoke();
    }

    [SerializeField] private BuildingShopData m_BuildingShopData;
    public BuildingShopData BuildingShopData => m_BuildingShopData;

    public virtual string GetPersonalizedStatsString()
    {
        throw new System.NotImplementedException();
    }
}
