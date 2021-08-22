using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMProText = TMPro.TextMeshProUGUI;

namespace LinkTD.UI
{
    public class StatDisplayResistanceControler : MonoBehaviour
    {
        [SerializeField] private GameObject m_Holder;

        [SerializeField] private Image m_ResistanceBar;
        [SerializeField] private TMProText m_ResistanceUpperBoundText;
        [SerializeField] private Image m_StatusImage;
        [SerializeField] private Color m_StatusActiveColor;
        [SerializeField] private Color m_StatusRechargingColor;
        [SerializeField] private TMProText m_StatusText;

        public void Show(IEnergetics energetics, Building building)
        {
            m_Holder.SetActive(true);

            float maxResistance = building.BaseStats.resistance + building.BonusStats.resistance;

            m_ResistanceBar.fillAmount = energetics.CurrentResistance / maxResistance;
            m_ResistanceUpperBoundText.text = maxResistance.ToString();
            bool isRecharging = building.IsRecharging();
            if(!isRecharging)
            {
                m_StatusImage.color = m_StatusActiveColor;
                m_StatusText.text = "Status: Active";
            }
            else
            {
                m_StatusImage.color = m_StatusRechargingColor;
                m_StatusText.text = "Status: Recharging";
            }
        }

        public void Hide()
        {
            m_Holder.SetActive(false);
        }
    }
}

