using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMProText = TMPro.TextMeshProUGUI;

namespace LinkTD.UI
{
    public class StatDisplayEnergyConsumptionControler : MonoBehaviour
    {
        [SerializeField] private GameObject m_Holder;

        [SerializeField] private TMProText m_ConsumptionText;
        [SerializeField] private TMProText m_EnergySliderText;
        [SerializeField] private TMProText m_AverageEnergyConsumption;

        [SerializeField] private Image m_EnergySlider;
        [SerializeField] private RectTransform m_EnergyConsumptionSlider;
        [SerializeField] private Vector2 m_EnergyConsumptionSliderMaxOffsets;

        public void Show(IActiveBuilding active, Building building)
        {
            m_Holder.SetActive(true);

            m_ConsumptionText.text = "Energy consumption: " + (building.BaseStats.electricUsage + building.BonusStats.electricUsage) + "kW per action";
            m_EnergySliderText.text = active.Energy.ToString("F2") + "kW / " + active.MaxEnergy.ToString() + "kW";
            if (building.isBuilt)
            {
                m_AverageEnergyConsumption.gameObject.SetActive(true);
                m_AverageEnergyConsumption.text = "Average energy gain: " + active.Analytics.CurrentMeasuredAverage.ToString("F2") +
                " kW/s";
            }
            else
                m_AverageEnergyConsumption.gameObject.SetActive(false);

            m_EnergySlider.fillAmount = active.Energy / active.MaxEnergy;
            float value = Mathf.Clamp(((active.Energy - (building.BaseStats.electricUsage + building.BonusStats.electricUsage)) / active.MaxEnergy) - (building.BaseStats.electricUsage + building.BonusStats.electricUsage),
                (building.BaseStats.electricUsage + building.BonusStats.electricUsage) / active.MaxEnergy, active.MaxEnergy);
            float xPos = MapValue(value, 0, 1,
                m_EnergyConsumptionSliderMaxOffsets.x, m_EnergyConsumptionSliderMaxOffsets.y);
            m_EnergyConsumptionSlider.anchoredPosition = new Vector2(xPos, 0);
        }

        public float MapValue(float value, float from1, float from2, float to1, float to2)
        {
            float normal = Mathf.InverseLerp(from1, from2, value);
            float bValue = Mathf.Lerp(to1, to2, normal);
            return bValue;
        }

        public void Hide()
        {
            m_Holder.SetActive(false);
        }
    }
}

