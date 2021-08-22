using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMProText = TMPro.TextMeshProUGUI;

namespace LinkTD.UI
{
    public class StatDisplayCombatControler : MonoBehaviour
    {
        [SerializeField] private GameObject m_Holder;

        [SerializeField] private TMProText m_DamageText;
        [SerializeField] private TMProText m_RangeText;
        [SerializeField] private TMProText m_FireRateText;

        public void Show(Building b)
        {
            m_Holder.SetActive(true);

            HitScanTurret t = b.GetComponent<HitScanTurret>();

            m_DamageText.text = "Damage: " + (b.BaseStats.power + b.BonusStats.power);
            m_FireRateText.text = "Fire rate: " + (1 / t.TimeBetweenShots).ToString("F2") + " per second";
            m_RangeText.text = "Range: 4 squares";
        }

        public void Hide()
        {
            m_Holder.SetActive(false);
        }
    }
}

