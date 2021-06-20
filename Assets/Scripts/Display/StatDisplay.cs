using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMProText = TMPro.TextMeshProUGUI;

public class StatDisplay : MonoBehaviour, IUIDisplay
{
    [SerializeField] private TMProText text;

    private void Awake()
    {
        SelectionManager.OnBuildingSelected += Show;
        SelectionManager.OnDeselection += Hide;
    }

    public void Show(DisplayData data)
    {
        Building b = SelectionManager.SelectedBuilding;

        text.text = b.ShowInfo();
    }

    public void Hide()
    {
        text.text = "";
    }
}
