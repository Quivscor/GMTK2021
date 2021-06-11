using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class SelectionManager : MonoBehaviour
{
    public static SelectionManager Instance;

    public Action OnBuildingSelected;

    public Building SelectedBuilding { get; private set; }

    private void Awake()
    {
        if( Instance == null )
            Instance = this;
    }

    public void Select(Building selectTarget)
    {
        SelectedBuilding = selectTarget;
        OnBuildingSelected?.Invoke();
    }
}
