using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMProText = TMPro.TextMeshProUGUI;

public class StatDisplay : MonoBehaviour
{
    [SerializeField] private TMProText text;

    private void Awake()
    {
        SelectionManager.OnBuildingSelected += DisplaySelectedBuilding;
        SelectionManager.OnDeselection += HideDisplay;
    }

    public void DisplaySelectedBuilding()
    {
        Building b = SelectionManager.SelectedBuilding;
        if(!b.isBuilt)
        {
            string operationSign = "x";
            if (b.BuildingData.isBoostAdditive)
                operationSign = "+";
            text.text = "Placing this will grant neighbouring buildings following bonuses: " +
                "\nPower: " + operationSign + b.BonusStats.power +
                "\nFrequency: " + operationSign + b.BonusStats.frequency;
            return;
        }

        if(b is ITurret t)
        {
            //display combat values
            text.text = "Damage: " + (b.BaseStats.power + b.BonusStats.power) +
                "\nFire rate: " + (b.BaseStats.frequency - (b.BonusStats.frequency / (32 + b.BonusStats.frequency))).ToString("n2") + " seconds per shot"; 
        }
        else
        {
            //display comboing values
            string operationSign = "x";
            if (b.BuildingData.isBoostAdditive)
                operationSign = "+";
            text.text = "Placing this will grant neighbouring buildings following bonuses: " +
                "\nPower: " + operationSign + b.BonusStats.power +
                "\nFrequency: " + operationSign + b.BonusStats.frequency;
            return;
        }
    }

    public void HideDisplay()
    {
        text.text = "";
    }
}
