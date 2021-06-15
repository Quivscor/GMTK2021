using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class GridController : MonoBehaviour
{
    public static Vector2Int GridSize = new Vector2Int(16, 16);
    public static Vector2Int GridFieldSize = new Vector2Int(1, 1);
    public static Vector2 GridOffset = new Vector2(0.5f, 0.5f);

    public static GridController Instance;
    public ResourcesController ResourcesController { get; private set; }
    private AudioSource source;

    public bool ConstructGrid = false;

    public GridField[,] Grid { get; private set; }

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

    public void RecalculateGrid()
    {
        Stack<GridField> buildings = new Stack<GridField>();
        Stack<GridField> turrets = new Stack<GridField>();

        for (int x = 0; x < GridSize.x; x++)
        {
            for (int y = 0; y < GridSize.y; y++)
            {
                if (Grid[x, y].Building == null)
                    continue;

                if(Grid[x,y].Building is ITurret t)
                    turrets.Push(Grid[x, y]);

                if (Grid[x, y].Building.isDirty && !GetIsNeighboursDirty(new Vector2Int(x, y)))
                {
                    RecalculateBuilding(new Vector2Int(x, y));
                }
                else if(Grid[x, y].Building.isDirty)
                    buildings.Push(Grid[x, y]);
            }
        }

        //should be fine here
        while(buildings.Count > 0)
        {
            GridField b = buildings.Pop();
            RecalculateBuilding(b.OwnCoordinates);
        }

        //will help updating turrets
        while(turrets.Count > 0)
        {
            GridField b = turrets.Pop();
            RecalculateBuilding(b.OwnCoordinates);
        }
    }

    public void RecalculateBuilding(Vector2Int coords)
    {
        Building b = Grid[coords.x, coords.y].Building;
        b.ResetBonusStats();
        b.isDirty = false;
        for (int i = 0; i < 4; i++)
        {
            switch (i)
            {
                //north
                case 0:
                    if (coords.y + 1 >= GridController.GridSize.y - 1 || Grid[coords.x, coords.y + 1].Building == null)
                        break;
                    if (!b.BuildingData.connectingTypes.HasFlag(Grid[coords.x, coords.y + 1].Building.BuildingData.type))
                        break;

                    if (Grid[coords.x, coords.y + 1].Building.BuildingData.isBoostAdditive == true)
                        b.AddBoostValue(Grid[coords.x, coords.y + 1].Building.BonusStats);
                    else
                        b.AddBoostMultiplier(Grid[coords.x, coords.y + 1].Building.BonusStats);

                    break;
                //south
                case 1:
                    if (coords.y - 1 <= 0 || Grid[coords.x, coords.y - 1].Building == null)
                        break;
                    if (!b.BuildingData.connectingTypes.HasFlag(Grid[coords.x, coords.y - 1].Building.BuildingData.type))
                        break;

                    if (Grid[coords.x, coords.y - 1].Building.BuildingData.isBoostAdditive == true)
                        b.AddBoostValue(Grid[coords.x, coords.y - 1].Building.BonusStats);
                    else
                        b.AddBoostMultiplier(Grid[coords.x, coords.y - 1].Building.BonusStats);
                    break;
                //east
                case 2:
                    if (coords.x + 1 >= GridController.GridSize.x - 1 || Grid[coords.x + 1, coords.y].Building == null)
                        break;
                    if (!b.BuildingData.connectingTypes.HasFlag(Grid[coords.x + 1, coords.y].Building.BuildingData.type))
                        break;

                    if (Grid[coords.x + 1, coords.y].Building.BuildingData.isBoostAdditive == true)
                        b.AddBoostValue(Grid[coords.x + 1, coords.y].Building.BonusStats);
                    else
                        b.AddBoostMultiplier(Grid[coords.x + 1, coords.y].Building.BonusStats);
                    break;
                //west
                case 3:
                    if (coords.x - 1 <= 0 || Grid[coords.x - 1, coords.y].Building == null)
                        break;
                    if (!b.BuildingData.connectingTypes.HasFlag(Grid[coords.x - 1, coords.y].Building.BuildingData.type))
                        break;

                    if (Grid[coords.x - 1, coords.y].Building.BuildingData.isBoostAdditive == true)
                        b.AddBoostValue(Grid[coords.x - 1, coords.y].Building.BonusStats);
                    else
                        b.AddBoostMultiplier(Grid[coords.x - 1, coords.y].Building.BonusStats);
                    break;
            }
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
        SetNeighboursDirty(data.gridFieldCoords);
        RecalculateGrid();
        SelectionManager.Deselect();
        source.PlayOneShot(source.clip);
        OnProcessBuildPlacement?.Invoke();
    }

    public void SetNeighboursDirty(Vector2Int coords)
    {
        BuildingType connectionTypes = Grid[coords.x, coords.y].Building.BuildingData.connectingTypes;
        for (int i = 0; i < 4; i++)
        {
            switch (i)
            {
                //north
                case 0:
                    if (coords.y + 1 >= GridController.GridSize.y - 1 || Grid[coords.x, coords.y + 1].Building == null)
                        break;
                    if(connectionTypes.HasFlag(Grid[coords.x, coords.y + 1].Building.BuildingData.type))
                        Grid[coords.x, coords.y + 1].Building.isDirty = true;
                    break;
                //south
                case 1:
                    if (coords.y - 1 <= 0 || Grid[coords.x, coords.y - 1].Building == null)
                        break;
                    if (connectionTypes.HasFlag(Grid[coords.x, coords.y - 1].Building.BuildingData.type))
                        Grid[coords.x, coords.y - 1].Building.isDirty = true;
                    break;
                //east
                case 2:
                    if (coords.x + 1 >= GridController.GridSize.x - 1 || Grid[coords.x + 1, coords.y].Building == null)
                        break;
                    if (connectionTypes.HasFlag(Grid[coords.x + 1, coords.y].Building.BuildingData.type))
                        Grid[coords.x + 1, coords.y].Building.isDirty = true;
                    break;
                //west
                case 3:
                    if (coords.x - 1 <= 0 || Grid[coords.x - 1, coords.y].Building == null)
                        break;
                    if (connectionTypes.HasFlag(Grid[coords.x - 1, coords.y].Building.BuildingData.type))
                        Grid[coords.x - 1, coords.y].Building.isDirty = true;
                    break;
            }
        }
    }

    public bool GetIsNeighboursDirty(Vector2Int coords)
    {
        bool result = false;
        for (int i = 0; i < 4; i++)
        {
            switch (i)
            {
                //north
                case 0:
                    if (coords.y + 1 >= GridController.GridSize.y - 1 || Grid[coords.x, coords.y + 1].Building == null)
                        break;
                    if (Grid[coords.x, coords.y + 1].Building.isDirty == true)
                        result = true;
                    break;
                //south
                case 1:
                    if (coords.y - 1 <= 0 || Grid[coords.x, coords.y - 1].Building == null)
                        break;
                    if (Grid[coords.x, coords.y - 1].Building.isDirty == true)
                        result = true;
                    break;
                //east
                case 2:
                    if (coords.x + 1 >= GridController.GridSize.x - 1 || Grid[coords.x + 1, coords.y].Building == null)
                        break;
                    if (Grid[coords.x + 1, coords.y].Building.isDirty == true)
                        result = true;
                    break;
                //west
                case 3:
                    if (coords.x - 1 <= 0 || Grid[coords.x - 1, coords.y].Building == null)
                        break;
                    if (Grid[coords.x - 1, coords.y].Building.isDirty == true)
                        result = true;
                    break;
            }
        }
        return result;
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
}
