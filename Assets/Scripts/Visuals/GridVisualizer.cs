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

        SelectionManager.Data.OnDeselection += HideClusterHighlight;
        SelectionManager.Data.OnDeselection += EnergeticsNetworkVisualizer.ClearMockConnections;

        foreach(GridField field in GridController.Grid)
        {
            //null fields are allowed now, just skip them
            if (field == null)
                continue;

            field.OnHoverEnter += DrawSelectedBuildingMock;
            field.OnHoverExit += HideSelectedBuildingMock;
        }

        //init recalculation dummy
        m_DummyBuilding.Construct();
    }

    public void DrawSelectedBuildingMock(GridFieldEventData e)
    {
        if(SelectionManager.Data.SelectedBuilding != null && !SelectionManager.Data.SelectedBuilding.isBuilt)
        {
            SelectionManager.PlaceMockAt(e.owner);

            if(SelectionManager.Data.SelectedBuilding is IPathfindingNode energeticsNode)
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
        var clusterOrdered = cluster.OrderByDescending(x =>
        {
            if (x.Building == null)
                return SelectionManager.Data.SelectedBuilding.BuildingComparator();
            return x.Building.BuildingComparator();
        });

        foreach(GridField field in activeList)
        {
            BuildingStats stats = GetClusterStatDifference(clusterOrdered, field.Building);
            IStatDisplayer display;
            if(field.Building.TryGetComponent(out display))
            {
                ActiveStatDisplayers.Add(display);
                //for preview purposes currently, UI requires rework
                string text = "";
                if (stats.power != 0)
                    text += "Power + " + stats.power.ToString("F") + "\n";
                if (stats.frequency != 0)
                    text += "Frequency + " + stats.frequency.ToString("F") + "\n";
                if (stats.resistance != 0)
                    text += "Resistance + " + stats.resistance.ToString("F") + "\n";
                if (stats.electricUsage != 0)
                    text += "Energy use + " + stats.electricUsage.ToString("F") + "\n";

                if (text != "")
                    display.BoostDisplay.Show(text);
            }
        }
        //for mock only
        if(SelectionManager.Data.SelectedBuilding.IsActiveBuilding())
        {
            BuildingStats stats = GetClusterStatDifference(clusterOrdered, SelectionManager.Data.SelectedBuilding);
            IStatDisplayer display;
            if (SelectionManager.Data.SelectedBuildingMock.TryGetComponent(out display))
            {
                //for preview purposes currently, UI requires rework
                string text = "";
                if (stats.power != 0)
                    text += "Power + " + stats.power.ToString("F") + "\n";
                if (stats.frequency != 0)
                    text += "Frequency + " + stats.frequency.ToString("F") + "\n";
                if (stats.resistance != 0)
                    text += "Resistance + " + stats.resistance.ToString("F") + "\n";
                if (stats.electricUsage != 0)
                    text += "Energy use + " + stats.electricUsage.ToString("F") + "\n";

                if (text != "")
                    display.BoostDisplay.Show(text);
            }
        }
    }

    private BuildingStats GetClusterStatDifference(IEnumerable<GridField> cluster, Building activeInCluster)
    {
        int activeBuildingsInClusterCount = GridController.Instance.CountActiveBuildingsInCluster(cluster);
        if(SelectionManager.Data.SelectedBuilding.IsActiveBuilding())
            activeBuildingsInClusterCount++;

        RecalculationBuilding.ResetBuildingBonuses();
        RecalculationBuilding.DebugSetBaseStats(activeInCluster.BaseStats);
        RecalculationBuilding.DebugSetBuildingType(activeInCluster.BuildingType);

        GridController.Instance.RecalculateBuilding(ref m_DummyBuilding, activeInCluster.transform.position, 
            cluster, activeBuildingsInClusterCount);

        return ((RecalculationBuilding.BaseStats + RecalculationBuilding.BonusStats) - 
            (activeInCluster.BaseStats + activeInCluster.BonusStats));
    }

    public void HideSelectedBuildingMock(GridFieldEventData e)
    {
        if (SelectionManager.Data.SelectedBuilding != null && !SelectionManager.Data.SelectedBuilding.isBuilt)
        {
            SelectionManager.HideMock();
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
        if(SelectionManager.Data.SelectedBuilding != null)
        {
            if (SelectionManager.Data.SelectedBuilding.IsActiveBuilding())
            {
                IStatDisplayer display = SelectionManager.Data.SelectedBuildingMock.GetComponentInChildren<IStatDisplayer>();
                display.BoostDisplay.Hide();
            }
        }

        ActiveStatDisplayers.Clear();
    }
}
