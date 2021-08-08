using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class GridVisualizer : MonoBehaviour
{
    [SerializeField] private CustomShapeGenerator m_CustomShapeRenderer;
    public CustomShapeGenerator CustomShapeRenderer => m_CustomShapeRenderer;

    [SerializeField] private Building m_DummyBuilding;
    public Building RecalculationBuilding => m_DummyBuilding;

    [SerializeField] private List<IStatDisplayer> m_ActiveStatDisplayer;
    public List<IStatDisplayer> ActiveStatDisplayers => m_ActiveStatDisplayer;

    private void Start()
    {
        m_ActiveStatDisplayer = new List<IStatDisplayer>();

        GridController.Instance.OnProcessBuildPlacement += HideClusterHighlight;
        GridController.Instance.OnProcessBuildPlacement += EnergeticsNetworkVisualizer.ClearMockConnections;

        foreach(GridField field in GridController.Grid)
        {
            field.OnHoverEnter += DrawSelectedBuildingMock;
            field.OnHoverExit += HideSelectedBuildingMock;
        }
    }

    public void DrawSelectedBuildingMock(GridFieldEventData e)
    {
        if(SelectionManager.SelectedBuilding != null && !SelectionManager.SelectedBuilding.isBuilt)
        {
            SelectionManager.PlaceMockAt(e.owner);

            if(SelectionManager.SelectedBuilding is IPathfindingNode energeticsNode)
                EnergeticsNetworkVisualizer.UpdateMockConnections(e.owner.transform.position);

            HashSet<GridField> cluster = GridController.GetCluster(e.owner.OwnCoordinates);

            DrawClusterHighlight(cluster);
            DrawActiveBuildingStatChange(cluster);
        }
    }

    private void DrawClusterHighlight(HashSet<GridField> cluster)
    {
        if (cluster.Count <= 1)
        {
            HideClusterHighlight();
            return;
        }

        Bounds[] bounds = new Bounds[cluster.Count];
        int index = 0;
        foreach (GridField field in cluster)
        {
            bounds[index] = field.GetComponent<BoxCollider2D>().bounds;
            index++;
        }
        CustomShapeRenderer.GenerateNewShape(bounds);
    }

    private void DrawActiveBuildingStatChange(HashSet<GridField> cluster)
    {
        var activeList = cluster.Where(x =>
        {
            if (x.Building == null)
                return false;
            return x.Building.IsActiveBuilding();
        });
        cluster.OrderByDescending(x =>
        {
            if (x.Building == null)
                return SelectionManager.SelectedBuilding.BuildingComparator();
            return x.Building.BuildingComparator();
        });

        foreach(GridField field in activeList)
        {
            BuildingStats stats = GetClusterStatDifference(cluster, field.Building);
            IStatDisplayer display;
            if(field.Building.TryGetComponent(out display))
            {
                ActiveStatDisplayers.Add(display);
                //for preview purposes currently, UI requires rework
                string text = "";
                if (stats.power != 0)
                    text += "Power + " + stats.power + "\n";
                if (stats.frequency != 0)
                    text += "Frequency + " + stats.frequency + "\n";
                if (stats.electricUsage != 0)
                    text += "Energy use + " + stats.electricUsage + "\n";

                if(text != "")
                    display.BoostDisplay.Show(text);
            }
        }
        //for mock only
        if(SelectionManager.SelectedBuilding.IsActiveBuilding())
        {
            BuildingStats stats = GetClusterStatDifference(cluster, SelectionManager.SelectedBuilding);
            IStatDisplayer display;
            if (SelectionManager.SelectedBuildingMock.TryGetComponent(out display))
            {
                //for preview purposes currently, UI requires rework
                string text = "";
                if (stats.power != 0)
                    text += "Power + " + stats.power + "\n";
                if (stats.frequency != 0)
                    text += "Frequency + " + stats.frequency + "\n";
                if (stats.electricUsage != 0)
                    text += "Energy use + " + stats.electricUsage + "\n";

                if (text != "")
                    display.BoostDisplay.Show(text);
            }
        }
    }

    private BuildingStats GetClusterStatDifference(IEnumerable<GridField> cluster, Building activeInCluster)
    {
        int activeBuildingsInClusterCount = GridController.Instance.CountActiveBuildingsInCluster(cluster);
        if(SelectionManager.SelectedBuilding.IsActiveBuilding())
            activeBuildingsInClusterCount++;

        RecalculationBuilding.ResetBuildingBonuses();
        RecalculationBuilding.DebugSetBaseStats(activeInCluster.BaseStats);
        RecalculationBuilding.DebugSetBuildingType(activeInCluster.BuildingType);

        GridController.Instance.RecalculateBuilding(ref m_DummyBuilding, activeInCluster.transform.position, 
            cluster, activeBuildingsInClusterCount);

        return (RecalculationBuilding.BonusStats - activeInCluster.BonusStats);
    }

    public void HideSelectedBuildingMock(GridFieldEventData e)
    {
        if (SelectionManager.SelectedBuilding != null && !SelectionManager.SelectedBuilding.isBuilt)
        {
            SelectionManager.HideMock();
            if (SelectionManager.SelectedBuilding is IPathfindingNode energeticsNode)
                EnergeticsNetworkVisualizer.ClearMockConnections();
        }
    }

    private void HideClusterHighlight()
    {
        CustomShapeRenderer.GenerateNewShape(new Bounds[0]);

        foreach (IStatDisplayer display in ActiveStatDisplayers)
        {
            display.BoostDisplay.Hide();
        }

        //mock works differently, special case for it here
        if(SelectionManager.SelectedBuilding.IsActiveBuilding())
        {
            IStatDisplayer display = SelectionManager.SelectedBuildingMock.GetComponentInChildren<IStatDisplayer>();
            display.BoostDisplay.Hide();
        }

        ActiveStatDisplayers.Clear();
    }
}
