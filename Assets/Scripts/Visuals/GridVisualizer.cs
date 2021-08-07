using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridVisualizer : MonoBehaviour
{
    private void Start()
    {
        foreach(GridField field in GridController.Grid)
        {
            field.OnHoverEnter += DrawSelectedBuildingMock;
            field.OnHoverExit += HideSelectedBuildingMock;
        }
    }

    public void DrawSelectedBuildingMock(GridFieldEventData e)
    {
        if(SelectionManager.SelectedBuilding != null && !SelectionManager.SelectedBuilding.isBuilt)
            SelectionManager.PlaceMockAt(e.owner);
    }

    public void HideSelectedBuildingMock(GridFieldEventData e)
    {
        if (SelectionManager.SelectedBuilding != null && !SelectionManager.SelectedBuilding.isBuilt)
            SelectionManager.HideMock();
    }
}
