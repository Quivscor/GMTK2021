using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridBuildProcessEventData
{
    public GridBuildProcessEventData()
    {

    }

    public GridBuildProcessEventData(Building building, Vector2Int gridFieldCoords)
    {
        this.building = building;
        this.gridFieldCoords = gridFieldCoords;
    }

    public Building building;
    public Vector2Int gridFieldCoords;
}
