using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public static class SelectionManager
{
    public static Action<DisplayData> OnBuildingSelected;
    public static Action OnDeselection;
    public static Action OnSelectedBuildingUpdated;

    public static Building SelectedBuilding { get; private set; }
    private static Building m_SelectedBuildingMock;
    private static GridField m_SelectedBuildingMockField;
    private static Vector3 m_DefaultMockPosition = Vector3.one * 100;

    public static readonly int IgnoreRaycastLayerID = 2;

    public static bool IsSelectedBuildingPlaceable()
    {
        return (SelectedBuilding != null && !SelectedBuilding.isBuilt);
    }

    public static void Select(Building selectTarget)
    {
        //when trying to assign already selected building
        if (SelectedBuilding == selectTarget)
            return;

        if (SelectedBuilding != null)
            Deselect();

        SelectedBuilding = selectTarget;
        InstantiateMock();
        SelectedBuilding.SelectBuilding();

        SelectedBuilding.OnBuildingStatsUpdated += UpdateDisplay;
        SelectedBuilding.OnDamageReceived += UpdateDisplay;
        if (SelectedBuilding is IActiveBuilding activeBuilding)
            activeBuilding.OnReceiveEnergy += UpdateDisplay;

        OnBuildingSelected?.Invoke(new DisplayData());
    }

    private static void InstantiateMock()
    {
        m_SelectedBuildingMock = GameObject.Instantiate(SelectedBuilding, m_DefaultMockPosition, Quaternion.identity, null);
        m_SelectedBuildingMock.gameObject.layer = IgnoreRaycastLayerID;
    }

    public static void UpdateDisplay()
    {
        OnBuildingSelected?.Invoke(new DisplayData());
    }

    public static void Deselect()
    {
        SelectedBuilding.OnBuildingStatsUpdated -= UpdateDisplay;
        SelectedBuilding.OnDamageReceived -= UpdateDisplay;
        if (SelectedBuilding is IActiveBuilding activeBuilding)
            activeBuilding.OnReceiveEnergy -= UpdateDisplay;

        SelectedBuilding.DeselectBuilding();
        SelectedBuilding = null;
        DestroyMock();

        OnDeselection?.Invoke();
    }

    private static void DestroyMock()
    {
        GameObject.Destroy(m_SelectedBuildingMock.gameObject);
    }

    public static void PlaceMockAt(GridField field)
    {
        m_SelectedBuildingMock.transform.position = field.transform.position;
        m_SelectedBuildingMockField = field;
    }

    public static void HideMock()
    {
        m_SelectedBuildingMock.transform.position = m_DefaultMockPosition;
        m_SelectedBuildingMockField = null;
    }
}
