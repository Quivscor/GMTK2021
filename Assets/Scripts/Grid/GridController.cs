using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class GridController : MonoBehaviour
{
    public static Vector2Int GridSize;
    public static Vector2 GridFieldSize;

    public static GridController Instance;
    [Header("Global settings")]
    public static bool ModuleEfficiencyInClustersReduced = false;
    [SerializeField] private bool m_ModuleEfficiencyInClustersReduced;
    public static bool ModuleEfficiencyFallsOffWithDistance = false;
    [SerializeField] private bool m_ModuleEfficiencyFallsOffWithDistance;

    public GridVisualizer GridVisualizer { get; private set; }
    public ResourcesController ResourcesController { get; private set; }
    public EnergeticsController EnergeticsController { get; private set; }
    private AudioSource source;

    //public bool ConstructGrid = false;

    public static GridField[,] Grid { get; private set; }
    public List<GridField> ActiveBuildingFields { get; private set; }

    [Header("Local values")]
    [SerializeField] private Transform m_GridContainer;
    [SerializeField] private Vector2Int m_GridSize;
    [SerializeField] private Vector2 m_GridFieldSize;

    public Action OnProcessBuildPlacement;

    private void Awake()
    {
        Init();

        ResourcesController = FindObjectOfType<ResourcesController>();
        EnergeticsController = FindObjectOfType<EnergeticsController>();
        GridVisualizer = FindObjectOfType<GridVisualizer>();
    }

    private void Init()
    {
        if (Instance == null)
            Instance = this;

        GridSize = new Vector2Int(m_GridSize.x, m_GridSize.y);
        GridFieldSize = new Vector2(m_GridFieldSize.x, m_GridFieldSize.y);

        Grid = new GridField[GridSize.x, GridSize.y];
        ActiveBuildingFields = new List<GridField>();

        List<GridField> gridFields = new List<GridField>();
        Bounds bounds = new Bounds();
        for (int i = 0; i < m_GridContainer.childCount; i++)
        {
            gridFields.Add(m_GridContainer.GetChild(i).GetComponent<GridField>());
            bounds.Encapsulate(gridFields[i].transform.position);
        }
        if (gridFields.Count > GridSize.x * GridSize.y)
            Debug.LogError("Size doesn't match grid fields count!");
        var gridFieldsOrdered = gridFields.OrderBy(x => x.transform.position.x * GridSize.y + x.transform.position.y);

        //Assign grid fields to controller
        int x = 0, y = 0;
        foreach(GridField field in gridFieldsOrdered)
        {
            bool assigned = false;
            while(!assigned)
            {
                Vector3 expectedPosition = new Vector3(bounds.min.x + (GridFieldSize.x * x),
                bounds.min.y + (GridFieldSize.y * y), 0);

                if(field.transform.position == expectedPosition)
                {
                    Grid[x, y] = field;
                    Grid[x, y].name = "GridField[" + x + "][" + y + "]";
                    Grid[x, y].Construct(new Vector2Int(x, y));
                    assigned = true;
                }

                y++;
                if(y > GridSize.y)
                {
                    x++;
                    y = 0;
                }

                //if GridSize is bigger than the map, infinite loop happens, this proteccs
                if (x >= GridSize.x && y >= GridSize.y)
                    break;
            }
        }

        ModuleEfficiencyInClustersReduced = m_ModuleEfficiencyInClustersReduced;
        ModuleEfficiencyFallsOffWithDistance = m_ModuleEfficiencyFallsOffWithDistance;
    }

    public void ProcessBuildingPlacement(GridFieldEventData data)
    {
        if (!ResourcesController.TrySpendMoney(data.building.BaseStats.cost))
            return;

        //we can't use Awake or Start anymore, so create and construct object here
        Building b = Instantiate(data.building, Grid[data.gridFieldCoords.x, data.gridFieldCoords.y].transform.position,
            Quaternion.identity, Grid[data.gridFieldCoords.x, data.gridFieldCoords.y].transform);
        b.Construct();

        Grid[data.gridFieldCoords.x, data.gridFieldCoords.y].AssignBuilding(b);
        b.name += "(" + data.gridFieldCoords.x + ", " + data.gridFieldCoords.y + ")";
        b.isBuilt = true;
        b.isDirty = true;

        if (!b.BuildingType.HasFlag(BuildingType.ADDMODULE) && !b.BuildingType.HasFlag(BuildingType.MULTMODULE))
            ActiveBuildingFields.Add(Grid[data.gridFieldCoords.x, data.gridFieldCoords.y]);

        if (b is IEnergetics energetics)
            EnergeticsController.ProcessEnergeticsBuildingPlacement(b);

        if (b.BuildingType == BuildingType.MONEYMAKER && b is IGenerator generator)
            ResourcesController.SubscribeGenerator(generator);

        RecalculateGrid();
        
        OnProcessBuildPlacement?.Invoke();
        SelectionManager.Deselect();
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
    bool hasIsDirty;

    private bool CheckCluster(Vector2Int coords)
    {
        checkedFields = new HashSet<GridField>();
        checkedFields.Add(Grid[coords.x, coords.y]);

        hasIsDirty = false;
        ForNeighboursDo(coords, CheckIfBuildingDirty);

        return hasIsDirty;
    }

    private bool CheckIfBuildingDirty(Vector2Int coords)
    {
        if (checkedFields.Contains(Grid[coords.x, coords.y]) || Grid[coords.x, coords.y].Building == null)
            return false;

        checkedFields.Add(Grid[coords.x, coords.y]);
        //gather all buildings in cluster
        ForNeighboursDo(coords, CheckIfBuildingDirty);

        if (Grid[coords.x, coords.y].Building.isDirty)
            hasIsDirty = true;
        return false;
    }

    static HashSet<GridField> clusterFields;

    public static HashSet<GridField> GetCluster(Vector2Int coords)
    {
        clusterFields = new HashSet<GridField>();
        clusterFields.Add(Grid[coords.x, coords.y]);

        ForNeighboursDo(coords, CheckNeighboursInCluster);

        return clusterFields;
    }

    private static bool CheckNeighboursInCluster(Vector2Int coords)
    {
        if (clusterFields.Contains(Grid[coords.x, coords.y]) || Grid[coords.x, coords.y].Building == null)
            return false;

        clusterFields.Add(Grid[coords.x, coords.y]);
        ForNeighboursDo(coords, CheckNeighboursInCluster);
        return true;
    }

    private void RecalculateCluster(Vector2Int clusterCenterCoords)
    {
        Building clusterBuilding = Grid[clusterCenterCoords.x, clusterCenterCoords.y].Building;
        var checkedFieldsOrdered = checkedFields.OrderByDescending(x => x.Building.BuildingComparator());

        int activeBuildingsInCluster;
        if (ModuleEfficiencyInClustersReduced)
            activeBuildingsInCluster = CountActiveBuildingsInCluster(checkedFieldsOrdered);
        else
            activeBuildingsInCluster = 1;

        clusterBuilding.ResetBuildingBonuses();
        RecalculateBuilding(ref clusterBuilding, checkedFieldsOrdered, activeBuildingsInCluster);
        checkedFields.Clear();
    }

    private float GetDistanceFromBuilding(Vector3 building, Vector3 target)
    {
        float distance;

        if (ModuleEfficiencyFallsOffWithDistance)
            distance = Mathf.Ceil(Vector3.Distance(building, target));
        else
            distance = 1;

        return distance;
    }

    //Recalculate buildings using actual building placement
    public void RecalculateBuilding(ref Building building, IEnumerable<GridField> fields, int activeBuildingsInCluster = 1)
    {
        float distanceFromActiveBuilding;

        foreach (GridField field in fields)
        {
            distanceFromActiveBuilding = GetDistanceFromBuilding(building.transform.position, field.transform.position);

            if (field.Building is IModule module)
            {
                IncreaseBuildingStats(building, module, activeBuildingsInCluster, distanceFromActiveBuilding);
            }
        }
    }

    //Recalculate building using fake building to store data and mock building to obtain difference in stats
    public void RecalculateBuilding(ref Building building, Vector3 buildingPosition, IEnumerable<GridField> fields, int activeBuildingsInCluster = 1)
    {
        float distanceFromActiveBuilding;
        bool hasCalculatedMock = false;

        foreach (GridField field in fields)
        {
                distanceFromActiveBuilding = GetDistanceFromBuilding(buildingPosition, field.transform.position);

            if (field.Building is IModule module)
            {
                IncreaseBuildingStats(building, module, activeBuildingsInCluster, distanceFromActiveBuilding);
            }
            //check based on item held over checked field, not placed on it
            else if (field.Building == null && SelectionManager.Data.SelectedBuilding is IModule selectedModule && !hasCalculatedMock)
            {
                IncreaseBuildingStats(building, selectedModule, activeBuildingsInCluster, distanceFromActiveBuilding);
                hasCalculatedMock = true;
            }
        }
    }

    private void IncreaseBuildingStats(Building building, IModule module, int activeBuildingsInCluster, float distanceFromActiveBuilding)
    {
        if (module.ConnectionData.ConnectingTypes.HasFlag(building.BuildingType))
        {
            if (module.ConnectionData.IsBoostAdditive)
                building.AddBoostValue((module.ConnectionData.ConnectionBoost / activeBuildingsInCluster) / distanceFromActiveBuilding);
            else
                building.AddBoostMultiplier((module.ConnectionData.ConnectionBoost / activeBuildingsInCluster) / distanceFromActiveBuilding);
        }
    }

    public int CountActiveBuildingsInCluster(IEnumerable<GridField> fields)
    {
        int result = 0;
        foreach(GridField field in fields)
        {
            if (field.Building == null)
                continue;

            if (field.Building.IsActiveBuilding())
            {
                result++;
            }
        }
        return result;
    }

    private static T ForNeighboursDo<T>(Vector2Int coords, Func<Vector2Int, T> Act)
    {
        T result = default(T);
        for (int i = 0; i < 4; i++)
        {
            switch (i)
            {
                //north
                case 0:
                    //check if not illegal position
                    if (coords.y + 1 > GridController.GridSize.y - 1 || Grid[coords.x, coords.y + 1] == null)
                        break;

                    result = Act(new Vector2Int(coords.x, coords.y + 1));
                    break;
                //south
                case 1:
                    //check if not illegal position
                    if (coords.y - 1 < 0 || Grid[coords.x, coords.y - 1] == null)
                        break;

                    result = Act(new Vector2Int(coords.x, coords.y - 1));
                    break;
                //east
                case 2:
                    //check if not illegal position
                    if (coords.x + 1 > GridController.GridSize.x - 1 || Grid[coords.x + 1, coords.y] == null)
                        break;

                    result = Act(new Vector2Int(coords.x + 1, coords.y));
                    break;
                //west
                case 3:
                    //check if not illegal position
                    if (coords.x - 1 < 0 || Grid[coords.x - 1, coords.y] == null)
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
