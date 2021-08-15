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
    private static Building m_SelectedBuildingCopy;
    private static Building m_SelectedBuildingMock;
    public static Building SelectedBuildingMock => m_SelectedBuildingMock;
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
        {
            activeBuilding.OnReceiveEnergy += UpdateDisplay;
            activeBuilding.OnUseEnergy += UpdateDisplay;
        }

        OnBuildingSelected?.Invoke(new DisplayData());
    }

    private static void InstantiateMock()
    {
        m_SelectedBuildingMock = GameObject.Instantiate(SelectedBuilding, m_DefaultMockPosition, Quaternion.identity, null);
        m_SelectedBuildingCopy = GameObject.Instantiate(SelectedBuilding, m_DefaultMockPosition, Quaternion.identity, null);
        //required to avoid null errors, shouldn't matter to those buildings
        m_SelectedBuildingMock.Construct();
        m_SelectedBuildingCopy.Construct();
        m_SelectedBuildingCopy.name = "SelectionCopy";
        m_SelectedBuildingMock.name = "BuildingMock";
        m_SelectedBuildingMock.gameObject.layer = IgnoreRaycastLayerID;
        m_SelectedBuildingMock.OnBuildingSelected?.Invoke();
        m_SelectedBuildingCopy.OnBuildingSelected?.Invoke();
    }

    private static void RecolorMock(bool retrieveOriginalColor = false)
    {
        if (SelectedBuildingMock == null)
            return;

        SpriteRenderer[] renderers = SelectedBuildingMock.GetComponentsInChildren<SpriteRenderer>();
        if(retrieveOriginalColor)
        {
            SpriteRenderer[] originalColors = m_SelectedBuildingCopy.GetComponentsInChildren<SpriteRenderer>();
            for(int i = 0; i < renderers.Length; i++)
            {
                renderers[i].color = originalColors[i].color;
            }
        }
        else
        {
            foreach (SpriteRenderer sr in renderers)
            {
                sr.color = new Color(1, 0, 0, .35f);
            }
        }    
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
        {
            activeBuilding.OnReceiveEnergy -= UpdateDisplay;
            activeBuilding.OnUseEnergy -= UpdateDisplay;
        }
            

        SelectedBuilding.DeselectBuilding();
        SelectedBuilding = null;
        DestroyMock();

        OnDeselection?.Invoke();
    }

    private static void DestroyMock()
    {
        GameObject.Destroy(m_SelectedBuildingMock.gameObject);
        GameObject.Destroy(m_SelectedBuildingCopy.gameObject);
    }

    public static void PlaceMockAt(GridField field)
    {
        if (field.Building != null)
            return;

        if (SelectedBuilding.AllowedPlacementFieldType != field.type)
            RecolorMock();
        else
            RecolorMock(true);

        m_SelectedBuildingMock.transform.position = field.transform.position;
        m_SelectedBuildingMockField = field;
    }

    public static void HideMock()
    {
        m_SelectedBuildingMock.transform.position = m_DefaultMockPosition;
        m_SelectedBuildingMockField = null;
    }
}
