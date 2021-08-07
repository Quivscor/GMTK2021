using UnityEngine;

public class GridFieldEventData
{
    public GridField owner;
    public Building building;
    public Vector2Int gridFieldCoords;

    public GridFieldEventData() { }

    public GridFieldEventData(GridField owner)
    {
        this.owner = owner;
    }

    public GridFieldEventData(Building building, Vector2Int gridFieldCoords)
    {
        this.building = building;
        this.gridFieldCoords = gridFieldCoords;
    }
}