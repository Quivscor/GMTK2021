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
            GridField neighbour = GridController.Instance.FindNeighbourWhich(PathNodes[PathNodes.Count - 1].OwnCoordinates, FindRoadNeighbour);

            if (neighbour == null)
                noNeighbours = true;
            else
                PathNodes.Add(neighbour);
        }

        Building hq = Resources.Load<Building>("HQ");
        PathNodes[PathNodes.Count - 1].AssignBuilding(GameObject.Instantiate<Building>(hq, PathNodes[PathNodes.Count - 1].transform.position, Quaternion.identity, null));
    }

    private bool FindRoadNeighbour(Vector2Int coords)
    {
        return GridController.Grid[coords.x, coords.y].type == GridFieldType.ROAD_FIELD &&
            !PathNodes.Contains(GridController.Grid[coords.x, coords.y]);
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
                        if (GridController.Grid[j, 0].type == GridFieldType.ROAD_FIELD)
                            return GridController.Grid[j, 0];
                    }
                    break;
                case 1:
                    for (int j = 0; j < GridController.GridSize.x; j++)
                    {
                        if (GridController.Grid[j, GridController.GridSize.y - 1].type == GridFieldType.ROAD_FIELD)
                            return GridController.Grid[j, GridController.GridSize.y - 1];
                    }
                    break;
                case 2:
                    for (int j = 0; j < GridController.GridSize.y; j++)
                    {
                        if (GridController.Grid[0, j].type == GridFieldType.ROAD_FIELD)
                            return GridController.Grid[0, j];
                    }
                    break;
                case 3:
                    for (int j = 0; j < GridController.GridSize.y; j++)
                    {
                        if (GridController.Grid[GridController.GridSize.x - 1, j].type == GridFieldType.ROAD_FIELD)
                            return GridController.Grid[GridController.GridSize.x - 1, j];
                    }
                    break;
            }
        }

        Debug.LogError("No ROAD_FIELD on the edge of the map!");
        return null;
    }
}