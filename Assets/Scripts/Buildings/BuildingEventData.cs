using UnityEngine;

public class BuildingEventData
{
    public Building building;
    public IPathfindingNode node;

    public BuildingEventData(Building building)
    {
        this.building = building;
    }

    public BuildingEventData(Building building, IPathfindingNode node)
    {
        this.building = building;
        this.node = node;
    }
}