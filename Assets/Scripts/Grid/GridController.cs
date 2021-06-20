using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class GridController : MonoBehaviour
{
    public static Vector2Int GridSize = new Vector2Int(16, 16);
    public static Vector2Int GridFieldSize = new Vector2Int(1, 1);
    public static Vector2 GridOffset = new Vector2(0.5f, 0.5f);

    public static GridController Instance;

    public ResourcesController ResourcesController { get; private set; }
    private AudioSource source;

    public bool ConstructGrid = false;

    public static GridField[,] Grid { get; private set; }
    public List<GridField> ActiveBuildingFields { get; private set; }

    [SerializeField] private GridField m_GridFieldPrefab;

    public Action OnProcessBuildPlacement;

    private void Awake()
    {
        Init();

        ResourcesController = FindObjectOfType<ResourcesController>();
        source = GetComponent<AudioSource>();
    }

    private void OnValidate()
    {
        if(ConstructGrid)
        {
            BuildGrid();
            ConstructGrid = false;
        }
    }

    private void Init()
    {
        if (Instance == null)
            Instance = this;

        Grid = new GridField[GridSize.x, GridSize.y];
        ActiveBuildingFields = new List<GridField>();

        //Assign grid fields to controller
        int x = 0, y = 0;
        for (int i = 0; i < transform.childCount; i++)
        {
            x = i % GridSize.x;
            y = i / GridSize.y;
            Grid[x, y] = transform.GetChild(i).GetComponent<GridField>();
            Grid[x, y].name = "Grid Field (" + x + ", " + y + ")";
            Grid[x, y].Construct(new Vector2Int(x, y));
        }
    }

    public void ProcessBuildingPlacement(GridBuildProcessEventData data)
    {
        if (!ResourcesController.TrySpendMoney(data.building.BaseStats.cost))
            return;

        Grid[data.gridFieldCoords.x, data.gridFieldCoords.y].AssignBuilding(data.building);
        data.building.transform.position = Grid[data.gridFieldCoords.x, data.gridFieldCoords.y].transform.position;
        data.building.isBuilt = true;
        data.building.isDirty = true;

        if (data.building is IActiveBuilding active)
            ActiveBuildingFields.Add(Grid[data.gridFieldCoords.x, data.gridFieldCoords.y]);

        RecalculateGrid();
        SelectionManager.Deselect();
        source.PlayOneShot(source.clip);
        OnProcessBuildPlacement?.Invoke();
    }

    private void RecalculateGrid()
    {
        foreach(GridField field in ActiveBuildingFields)
        {
            //look for dirty buildings in the cluster
            if(CheckCluster(field.OwnCoordinates))
            {
                //recalculate cluster
                RecalculateCluster(field.OwnCoordinates);
            }
        }
    }

    HashSet<GridField> checkedFields;

    private bool CheckCluster(Vector2Int coords)
    {
        checkedFields = new HashSet<GridField>();
        checkedFields.Add(Grid[coords.x, coords.y]);

        return ForNeighboursDo(coords, CheckIfBuildingDirty);
    }

    private bool CheckIfBuildingDirty(Vector2Int coords)
    {
        if (checkedFields.Contains(Grid[coords.x, coords.y]) || Grid[coords.x, coords.y].Building == null)
            return false;

        checkedFields.Add(Grid[coords.x, coords.y]);
        //gather all buildings in cluster
        ForNeighboursDo(coords, CheckIfBuildingDirty);

        if (Grid[coords.x, coords.y].Building.isDirty)
            return true;

        return false;
    }

    private void RecalculateCluster(Vector2Int clusterCenterCoords)
    {
        List<GridField> clusterFields = 
            checkedFields.OrderByDescending(x => Vector3.Distance(x.transform.position, 
            Grid[clusterCenterCoords.x, clusterCenterCoords.y].transform.position)).ToList<GridField>();
        checkedFields.Clear();

        foreach(GridField field in clusterFields)
        {
            //clear previous calcs before recalculating
            field.Building.ResetBuildingBonuses();
        }

        foreach(GridField field in clusterFields)
        {
            //we can call building without checking because CheckCluster did that earlier
            RecalculateBuilding(field.OwnCoordinates);
        }
    }

    Building recalculatedBuilding;

    private void RecalculateBuilding(Vector2Int coords)
    {
        recalculatedBuilding = Grid[coords.x, coords.y].Building;
        ForNeighboursDo(coords, UpdateBuilding);
        recalculatedBuilding.isDirty = false;
        recalculatedBuilding = null;
    }

    private bool UpdateBuilding(Vector2Int coords)
    {
        if (Grid[coords.x, coords.y].Building == null)
            return false;

        if(Grid[coords.x, coords.y].Building is IModule module)
        {
            if(module.ConnectionData.ConnectingTypes.HasFlag(recalculatedBuilding.BuildingType))
            {
                if (module.ConnectionData.IsBoostAdditive)
                    recalculatedBuilding.AddBoostValue(module.ConnectionData.ConnectionBoost + 
                        Grid[coords.x, coords.y].Building.BonusStats);
                else
                    recalculatedBuilding.AddBoostMultiplier(module.ConnectionData.ConnectionBoost + 
                        Grid[coords.x, coords.y].Building.BonusStats);
            }

            return true;
        }

        return false;
    }

    private void BuildGrid()
    {
        int destroyCount = transform.childCount - 1;
        //clear field before reinstantiating
        for (int i = destroyCount; i > 0; i--)
        {
            Destroy(transform.GetChild(i));
        }

        for(int x = 0; x < GridSize.x; x++)
        {
            for(int y = 0; y < GridSize.y; y++)
            {
                GridField field = Instantiate<GridField>(m_GridFieldPrefab, new Vector3(GridOffset.x + x * GridFieldSize.x, GridOffset.y + y * GridFieldSize.y),
                    Quaternion.identity, this.transform);
                field.Construct(new Vector2Int(x,y));
            }
        }

        Debug.Log("Building grid complete.");
    }

    public T ForNeighboursDo<T>(Vector2Int coords, Func<Vector2Int, T> Act)
    {
        T result = default(T);
        for (int i = 0; i < 4; i++)
        {
            switch (i)
            {
                //north
                case 0:
                    //check if not illegal position
                    if (coords.y + 1 >= GridController.GridSize.y - 1)
                        break;

                    result = Act(new Vector2Int(coords.x, coords.y + 1));
                    break;
                //south
                case 1:
                    //check if not illegal position
                    if (coords.y - 1 <= 0)
                        break;

                    result = Act(new Vector2Int(coords.x, coords.y - 1));
                    break;
                //east
                case 2:
                    //check if not illegal position
                    if (coords.x + 1 >= GridController.GridSize.x - 1)
                        break;

                    result = Act(new Vector2Int(coords.x + 1, coords.y));
                    break;
                //west
                case 3:
                    //check if not illegal position
                    if (coords.x - 1 <= 0)
                        break;

                    result = Act(new Vector2Int(coords.x - 1, coords.y));
                    break;
            }
        }
        return result;
    }

    public GridField FindNeighbourWhich(Vector2Int coords, Func<Vector2Int, bool> Act)
    {
        for (int i = 0; i < 4; i++)
        {
            switch (i)
            {
                //north
                case 0:
                    //check if not illegal position
                    if (coords.y + 1 >= GridController.GridSize.y - 1)
                        break;

                    if (Act(new Vector2Int(coords.x, coords.y + 1)))
                        return Grid[coords.x, coords.y + 1];
                    break;
                //south
                case 1:
                    //check if not illegal position
                    if (coords.y - 1 <= 0)
                        break;

                    if (Act(new Vector2Int(coords.x, coords.y - 1)))
                        return Grid[coords.x, coords.y - 1];
                    break;
                //east
                case 2:
                    //check if not illegal position
                    if (coords.x + 1 >= GridController.GridSize.x - 1)
                        break;

                    if (Act(new Vector2Int(coords.x + 1, coords.y)))
                        return Grid[coords.x + 1, coords.y];
                    break;
                //west
                case 3:
                    //check if not illegal position
                    if (coords.x - 1 <= 0)
                        break;

                    if (Act(new Vector2Int(coords.x - 1, coords.y)))
                        return Grid[coords.x - 1, coords.y];
                    break;
            }
        }

        return null;
    }
}
