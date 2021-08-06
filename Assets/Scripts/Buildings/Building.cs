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
    public BuildingType BuildingType { get => m_BuildingType; }

    [SerializeField] private BuildingStats m_BaseStats;
    public BuildingStats BaseStats { get => m_BaseStats; protected set => m_BaseStats = value; }
    public BuildingStats BonusStats { get; protected set; }

    //actual hp of building
    public float CurrentResistance { get; protected set; }
    //is building inactive and regenerating
    public bool IsRecharging { get; protected set; }
    private float m_CurrentRechargeTime;

    public Action OnBuildingStatsUpdated;
    public Action OnDamageReceived;
    public Action<BuildingEventData> OnEnterRechargingState;

    public bool isDirty = false;
    public bool isBuilt = false;

    protected virtual void Start()
    {
        BonusStats = new BuildingStats();

        SetBuildingCustomStats();
        OnBuildingStatsUpdated += SetBuildingCustomStats;
    }

    protected virtual void Update()
    {
        if (!isBuilt || Time.timeScale == 0)
            return;

        if (IsBuildingRecharging())
            return;
    }

    protected virtual bool IsBuildingRecharging()
    {
        float maxResistance = BaseStats.resistance + BonusStats.resistance;
        if (CurrentResistance <= 0 && !IsRecharging)
        {
            IsRecharging = true;
            m_CurrentRechargeTime = 0;
            OnEnterRechargingState?.Invoke(new BuildingEventData(this, this as IPathfindingNode));
            return true;
        }
        else if(IsRecharging)
        {
            m_CurrentRechargeTime += Time.deltaTime;
            float rechargeTime = BaseStats.rechargeRate + BonusStats.rechargeRate;
            if (m_CurrentRechargeTime >= rechargeTime)
                IsRecharging = false;
            CurrentResistance = Mathf.Lerp(0, maxResistance, m_CurrentRechargeTime / rechargeTime);
            return true;
        }
        return false;
    }

    public void ResetBuildingBonuses()
    {
        BonusStats.Reset();
        isDirty = true;
    }

    protected virtual void SetBuildingCustomStats()
    {
        
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

    public virtual int BuildingComparator() { return 10000; }

    public void OnPointerClick(PointerEventData eventData)
    {
        SelectionManager.Select(this);
    }

    public void ReceiveDamage(float value)
    {
        if (BaseStats.resistance == Mathf.Infinity)
            return;
        CurrentResistance -= value;
        OnDamageReceived?.Invoke();
    }

    public virtual string ShowInfo()
    {
        throw new System.NotImplementedException();
    }
}
