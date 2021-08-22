using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public static class SelectionManager
{
    public static SelectionManagerData Data = new SelectionManagerData();
    
    private static Vector3 m_DefaultMockPosition = Vector3.one * 100;
    public static readonly int IgnoreRaycastLayerID = 2;

    public static void Clear()
    {
        Data = new SelectionManagerData();
    }

    public static bool IsSelectedBuildingPlaceable()
    {
        return (Data.SelectedBuilding != null && !Data.SelectedBuilding.isBuilt);
    }

    public static void Select(Building selectTarget)
    {
        //when trying to assign already selected building
        if (Data.SelectedBuilding == selectTarget)
            return;

        if (Data.SelectedBuilding != null)
            Deselect();

        Data.SelectedBuilding = selectTarget;
        InstantiateMock();
        Data.SelectedBuilding.SelectBuilding();

        Data.SelectedBuilding.OnBuildingUpdated += UpdateDisplay;
        Data.SelectedBuilding.OnDamageReceived += UpdateDisplay;
        if (Data.SelectedBuilding is IActiveBuilding activeBuilding)
        {
            activeBuilding.OnReceiveEnergy += UpdateDisplay;
            activeBuilding.OnUseEnergy += UpdateDisplay;
        }
        if(Data.SelectedBuilding is IGenerator generator)
        {
            generator.OnGenerationProgress += UpdateDisplay;
        }

        Data.OnBuildingSelected?.Invoke();
    }

    private static void InstantiateMock()
    {
        Data.SelectedBuildingMock = GameObject.Instantiate(Data.SelectedBuilding, m_DefaultMockPosition, Quaternion.identity, null);
        Data.SelectedBuildingCopy = GameObject.Instantiate(Data.SelectedBuilding, m_DefaultMockPosition, Quaternion.identity, null);
        //required to avoid null errors, shouldn't matter to those buildings
        Data.SelectedBuildingMock.Construct();
        Data.SelectedBuildingCopy.Construct();
        Data.SelectedBuildingCopy.name = "SelectionCopy";
        Data.SelectedBuildingMock.name = "BuildingMock";
        Data.SelectedBuildingMock.gameObject.layer = IgnoreRaycastLayerID;
        Data.SelectedBuildingMock.OnBuildingSelected?.Invoke();
        Data.SelectedBuildingCopy.OnBuildingSelected?.Invoke();
    }

    private static void RecolorMock(bool retrieveOriginalColor = false)
    {
        if (Data.SelectedBuildingMock == null)
            return;

        SpriteRenderer[] renderers = Data.SelectedBuildingMock.GetComponentsInChildren<SpriteRenderer>();
        if(retrieveOriginalColor)
        {
            SpriteRenderer[] originalColors = Data.SelectedBuildingCopy.GetComponentsInChildren<SpriteRenderer>();
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
        Data.OnBuildingSelected?.Invoke();
    }

    public static void Deselect()
    {
        Data.SelectedBuilding.OnBuildingUpdated -= UpdateDisplay;
        Data.SelectedBuilding.OnDamageReceived -= UpdateDisplay;
        if (Data.SelectedBuilding is IActiveBuilding activeBuilding)
        {
            activeBuilding.OnReceiveEnergy -= UpdateDisplay;
            activeBuilding.OnUseEnergy -= UpdateDisplay;
        }
        if (Data.SelectedBuilding is IGenerator generator)
        {
            generator.OnGenerationProgress -= UpdateDisplay;
        }

        Data.SelectedBuilding.DeselectBuilding();
        Data.SelectedBuilding = null;
        DestroyMock();

        Data.OnDeselection?.Invoke();
    }

    private static void DestroyMock()
    {
        GameObject.Destroy(Data.SelectedBuildingMock.gameObject);
        GameObject.Destroy(Data.SelectedBuildingCopy.gameObject);
    }

    public static void PlaceMockAt(GridField field)
    {
        if (field.Building != null)
            return;

        if (Data.SelectedBuilding.AllowedPlacementFieldType != field.type)
            RecolorMock();
        else
            RecolorMock(true);

        Data.SelectedBuildingMock.transform.position = field.transform.position;
        Data.SelectedBuildingMockField = field;
    }

    public static void HideMock()
    {
        Data.SelectedBuildingMock.transform.position = m_DefaultMockPosition;
        Data.SelectedBuildingMockField = null;
    }
}
