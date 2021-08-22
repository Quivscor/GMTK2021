using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using System;

public class GridField : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    public GridFieldType type = GridFieldType.UNKNOWN;
    public Vector2Int OwnCoordinates { get; private set; }

    [SerializeField] private Building m_Building;
    public Building Building { get => m_Building; private set => m_Building = value; }

    public Action<GridFieldEventData> OnHoverEnter;
    public Action<GridFieldEventData> OnHoverExit;

    //Init from creating class here
    public void Construct(Vector2Int coordinates)
    {
        OwnCoordinates = new Vector2Int(coordinates.x, coordinates.y);
    }

    public void AssignBuilding(Building building)
    {
        Building = building;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if(eventData.button == PointerEventData.InputButton.Left)
        {
            if (SelectionManager.IsSelectedBuildingPlaceable() && Building == null 
                && type == SelectionManager.Data.SelectedBuilding.AllowedPlacementFieldType)
            {
                //place building if possible
                GridController.Instance.ProcessBuildingPlacement(new GridFieldEventData(SelectionManager.Data.SelectedBuilding, OwnCoordinates));
            }
            else if (Building != null)
            {
                //select placed building
                SelectionManager.Select(Building);
            }
        }
        else if(eventData.button == PointerEventData.InputButton.Right)
        {
            SelectionManager.Deselect();
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        OnHoverEnter?.Invoke(new GridFieldEventData(this));
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        OnHoverExit?.Invoke(new GridFieldEventData(this));
    }
}

public enum GridFieldType
{
    UNKNOWN = 0,
    BUILD_FIELD = 1,
    ROAD_FIELD = 2,
    GENERATOR_FIELD = 3,

}
