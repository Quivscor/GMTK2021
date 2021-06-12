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

    public bool ConstructGrid = false;

    public GridField[,] Grid { get; private set; }

    [SerializeField] private GridField m_GridFieldPrefab;

    public Action OnProcessBuildPlacement;

    private void Awake()
    {
        Init();
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

    public void ProcessBuildingPlacement(GridBuildProcessEventData data)
    {
        Grid[data.gridFieldCoords.x, data.gridFieldCoords.y].AssignBuilding(data.building);
        data.building.transform.position = Grid[data.gridFieldCoords.x, data.gridFieldCoords.y].transform.position;
        SelectionManager.Instance.Deselect();
        OnProcessBuildPlacement?.Invoke();
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
