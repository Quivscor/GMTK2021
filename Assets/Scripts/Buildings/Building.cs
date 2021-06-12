using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Building : MonoBehaviour, IPointerClickHandler
{
    public BuildingStats BaseStats { get; private set; }
    public BuildingStats BonusStats { get; private set; }

    public void OnPointerClick(PointerEventData eventData)
    {
        SelectionManager.Instance?.Select(this);
    }
}
