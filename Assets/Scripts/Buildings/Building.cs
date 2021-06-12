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

    public bool isBuilt = false;

    protected virtual void Awake()
    {

    }

    public void OnPointerClick(PointerEventData eventData)
    {
        SelectionManager.Instance?.Select(this);
    }
}
