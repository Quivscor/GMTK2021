using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class GridField : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler
{
    public GridFieldType type = GridFieldType.UNKNOWN;

    [SerializeField] private Sprite buildPlotSprite;
    [SerializeField] private Sprite roadSprite;

    public Vector2Int OwnCoordinates { get; private set; }

    [SerializeField] private Building m_Building;
    public Building Building { get => m_Building; private set => m_Building = value; }

    private void Awake()
    {
        if (type == GridFieldType.ROAD_FIELD)
            GetComponentInChildren<SpriteRenderer>().sprite = roadSprite;
        else
            GetComponentInChildren<SpriteRenderer>().sprite = buildPlotSprite;
    }

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
        if(SelectionManager.IsSelectedBuildingPlaceable() && Building == null && type != GridFieldType.ROAD_FIELD)
        {
            //place building if possible
            GridController.Instance.ProcessBuildingPlacement(new GridBuildProcessEventData(SelectionManager.SelectedBuilding, OwnCoordinates));
        }
        else if(Building != null)
        {
            //select placed building
            SelectionManager.Select(Building);
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if(SelectionManager.SelectedBuilding != null)
        {
            //GridVisualizer.TryDisplayBuilding()
        }
    }
}

public enum GridFieldType
{
    UNKNOWN = 0,
    BUILD_FIELD = 1,
    ROAD_FIELD = 2,
}
