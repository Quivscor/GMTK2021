using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class SelectionManager : MonoBehaviour
{
    public static Action OnBuildingSelected;
    public static Action OnDeselection;

    public static Building SelectedBuilding { get; private set; }

    public static bool IsSelectedBuildingPlaceable()
    {
        return (SelectedBuilding != null && !SelectedBuilding.isBuilt);
    }

    public static void Select(Building selectTarget)
    {
        if (SelectedBuilding != null)
            SelectedBuilding.transform.localScale = Vector3.one;
        SelectedBuilding = selectTarget;
        SelectedBuilding.transform.localScale = new Vector3(1.2f, 1.2f, 1.2f);
        OnBuildingSelected?.Invoke();
    }

    public static void Deselect()
    {
        SelectedBuilding.transform.localScale = Vector3.one;
        SelectedBuilding = null;
        OnDeselection?.Invoke();
    }
}
