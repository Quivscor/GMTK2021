using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMProText = TMPro.TextMeshProUGUI;

public class StatDisplay : MonoBehaviour, IUIDisplay
{
    [SerializeField] private TMProText m_Text;

    private void Awake()
    {
        SelectionManager.OnBuildingSelected += Show;
        SelectionManager.OnDeselection += Hide;
    }

    public void Show(DisplayData data)
    {
        Building b = SelectionManager.SelectedBuilding;

        m_Text.text = b.ShowInfo();
    }

    public void Hide()
    {
        m_Text.text = "";
    }
}
