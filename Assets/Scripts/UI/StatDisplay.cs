using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMProText = TMPro.TextMeshProUGUI;

namespace LinkTD.UI
{
    public class StatDisplay : MonoBehaviour
    {
        [SerializeField] private GameObject m_Holder;

        private StatDisplayGeneralStatsControler m_GeneralStatsDisplay;
        private StatDisplayEnergyConsumptionControler m_EnergyConsumptionDisplay;
        private StatDisplayGenerationControler m_GenerationDisplay;
        private StatDisplayModuleControler m_ModuleDisplay;
        private StatDisplayCombatControler m_CombatDisplay;
        private StatDisplayResistanceControler m_ResistanceControler;
        private StatDisplayNetworkNodeControler m_NetworkControler;

        private void Awake()
        {
            m_GeneralStatsDisplay = GetComponentInChildren<StatDisplayGeneralStatsControler>();
            m_EnergyConsumptionDisplay = GetComponentInChildren<StatDisplayEnergyConsumptionControler>();
            m_GenerationDisplay = GetComponentInChildren<StatDisplayGenerationControler>();
            m_ModuleDisplay = GetComponentInChildren<StatDisplayModuleControler>();
            m_CombatDisplay = GetComponentInChildren<StatDisplayCombatControler>();
            m_ResistanceControler = GetComponentInChildren<StatDisplayResistanceControler>();
            m_NetworkControler = GetComponentInChildren<StatDisplayNetworkNodeControler>();

            SelectionManager.Data.OnBuildingSelected += Show;
            SelectionManager.Data.OnDeselection += Hide;

            Hide();
        }

        public void Show()
        {
            m_Holder.SetActive(true);

            Building b = SelectionManager.Data.SelectedBuilding;
            BuildingShopData data = b.BuildingShopData;

            m_GeneralStatsDisplay.Show(data);
            if (b is IActiveBuilding active)
                m_EnergyConsumptionDisplay.Show(active, b);
            else
                m_EnergyConsumptionDisplay.Hide();

            if (b is ITurret turret)
                m_CombatDisplay.Show(b);
            else
                m_CombatDisplay.Hide();

            if (b is IGenerator generator)
                m_GenerationDisplay.Show(generator);
            else
                m_GenerationDisplay.Hide();

            if (b is IEnergetics energetics)
            {
                m_ResistanceControler.Show(energetics, b);
                m_NetworkControler.Show(energetics);
            }
            else
            {
                m_ResistanceControler.Hide();
                m_NetworkControler.Hide();
            }

            if (b is IModule module)
                m_ModuleDisplay.Show(module);
            else
                m_ModuleDisplay.Hide();
        }

        public void Hide()
        {
            m_Holder.SetActive(false);
        }   
    }       
}           

