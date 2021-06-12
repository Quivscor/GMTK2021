using System.Linq;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class EnemyPath
{
    public List<GridField> PathNodes { get; private set; }

    public EnemyPath()
    {
        
    }

    //TODO: find a way to call this in constructor
    public void ConstructPath()
    {
        PathNodes = new List<GridField>();
        PathNodes.Add(FindStartingNode());
        bool noNeighbours = false;

        while (!noNeighbours)
        {
            GridField neighbour = null;
            for (int i = 0; i < 4; i++)
            {
                neighbour = FindRoadNeighbour(i);
                if (neighbour != null)
                    break;
            }

            if (neighbour == null)
                noNeighbours = true;
            else
                PathNodes.Add(neighbour);
        }
    }

    public GridField FindRoadNeighbour(int index)
    {
        GridField current = PathNodes[PathNodes.Count - 1];
        GridField previous = null;
        if (PathNodes.Count > 1)
            previous = PathNodes[PathNodes.Count - 2];
        switch(index)
        {
            //north
            case 0:
                if (current.OwnCoordinates.y + 1 >= GridController.GridSize.y - 1)
                    break;
                if (GridController.Instance.Grid[current.OwnCoordinates.x, current.OwnCoordinates.y + 1].type == GridFieldType.ROAD_FIELD &&
                    GridController.Instance.Grid[current.OwnCoordinates.x, current.OwnCoordinates.y + 1] != previous)
                    return GridController.Instance.Grid[current.OwnCoordinates.x, current.OwnCoordinates.y + 1];
                break;
            //south
            case 1:
                if (current.OwnCoordinates.y - 1 <= 0)
                    break;
                if (GridController.Instance.Grid[current.OwnCoordinates.x, current.OwnCoordinates.y - 1].type == GridFieldType.ROAD_FIELD &&
                    GridController.Instance.Grid[current.OwnCoordinates.x, current.OwnCoordinates.y - 1] != previous)
                    return GridController.Instance.Grid[current.OwnCoordinates.x, current.OwnCoordinates.y - 1];
                break;
            //east
            case 2:
                if (current.OwnCoordinates.x + 1 >= GridController.GridSize.x - 1)
                    break;
                if (GridController.Instance.Grid[current.OwnCoordinates.x + 1, current.OwnCoordinates.y].type == GridFieldType.ROAD_FIELD &&
                    GridController.Instance.Grid[current.OwnCoordinates.x + 1, current.OwnCoordinates.y] != previous)
                    return GridController.Instance.Grid[current.OwnCoordinates.x + 1, current.OwnCoordinates.y];
                break;
            //west
            case 3:
                if (current.OwnCoordinates.x - 1 <= 0)
                    break;
                if (GridController.Instance.Grid[current.OwnCoordinates.x - 1, current.OwnCoordinates.y].type == GridFieldType.ROAD_FIELD &&
                    GridController.Instance.Grid[current.OwnCoordinates.x - 1, current.OwnCoordinates.y] != previous)
                    return GridController.Instance.Grid[current.OwnCoordinates.x - 1, current.OwnCoordinates.y];
                break;
        }

        //no neighbour found
        return null;
    }

    //Checking Grid for path node on the edge of the map
    public GridField FindStartingNode()
    {
        for(int i = 0; i < 4; i++)
        {
            switch(i)
            {
                case 0:
                    for(int j = 0; j < GridController.GridSize.x; j++)
                    {
                        if (GridController.Instance.Grid[j, 0].type == GridFieldType.ROAD_FIELD)
                            return GridController.Instance.Grid[j, 0];
                    }
                    break;
                case 1:
                    for (int j = 0; j < GridController.GridSize.x; j++)
                    {
                        if (GridController.Instance.Grid[j, GridController.GridSize.y - 1].type == GridFieldType.ROAD_FIELD)
                            return GridController.Instance.Grid[j, GridController.GridSize.y - 1];
                    }
                    break;
                case 2:
                    for (int j = 0; j < GridController.GridSize.y; j++)
                    {
                        if (GridController.Instance.Grid[0, j].type == GridFieldType.ROAD_FIELD)
                            return GridController.Instance.Grid[0, j];
                    }
                    break;
                case 3:
                    for (int j = 0; j < GridController.GridSize.y; j++)
                    {
                        if (GridController.Instance.Grid[GridController.GridSize.x - 1, j].type == GridFieldType.ROAD_FIELD)
                            return GridController.Instance.Grid[GridController.GridSize.x - 1, j];
                    }
                    break;
            }
        }

        Debug.LogError("No ROAD_FIELD on the edge of the map!");
        return null;
    }
}