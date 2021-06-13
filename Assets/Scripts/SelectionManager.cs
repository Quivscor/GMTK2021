using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class SelectionManager : MonoBehaviour
{
    public static SelectionManager Instance;

    public Action OnBuildingSelected;
    public Action OnDeselection;

    public Building SelectedBuilding { get; private set; }

    private void Awake()
    {
        if( Instance == null )
            Instance = this;
    }

    public void Select(Building selectTarget)
    {
        if (SelectedBuilding != null)
            SelectedBuilding.transform.localScale = Vector3.one;
        SelectedBuilding = selectTarget;
        SelectedBuilding.transform.localScale = new Vector3(1.2f, 1.2f, 1.2f);
        OnBuildingSelected?.Invoke();
    }

    public void Deselect()
    {
        SelectedBuilding.transform.localScale = Vector3.one;
        SelectedBuilding = null;
        OnDeselection?.Invoke();
    }
}
