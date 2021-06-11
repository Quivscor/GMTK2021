using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class GridField : MonoBehaviour, IPointerDownHandler
{
    public GridFieldType type = GridFieldType.UNKNOWN;

    [SerializeField] private Building m_Building;
    public Building Building { get => m_Building; private set => m_Building = value; }

    //Init from creating class here
    public void Construct()
    {

    }

    public void AssignBuilding(Building building)
    {
        Building = building;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if(SelectionManager.Instance?.SelectedBuilding != null)
        {
            GridController.Instance.ProcessBuildingPlacement();
        }
    }
}

public enum GridFieldType
{
    UNKNOWN = 0,
    BUILD_FIELD = 1,
    ROAD_FIELD = 2,
}
