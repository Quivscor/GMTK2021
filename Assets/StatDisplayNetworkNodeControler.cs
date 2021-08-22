using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMProText = TMPro.TextMeshProUGUI;

namespace LinkTD.UI
{
    public class StatDisplayNetworkNodeControler : MonoBehaviour
    {
        [SerializeField] private GameObject m_Holder;

        [SerializeField] private TMProText m_ParticleSpeedText;
        [SerializeField] private TMProText m_NetworkDamageText;

        public void Show(IEnergetics energetics)
        {
            m_Holder.SetActive(true);

            m_ParticleSpeedText.text = "Network speed towards here: " + (1/(EnergyParticle.DefaultTraversalTime / (1 + Mathf.Log(1 + energetics.ConnectionSpeedModifier, 5)))).ToString("F2") + " square/s";
            m_NetworkDamageText.text = "Network damage from here: " + (EnergeticsNetwork.EnergyNetworkContactEnemyDamage * energetics.ConnectionDamageModifier * 6).ToString("F2") + "/s";
        }

        public void Hide()
        {
            m_Holder.SetActive(false);
        }
    }
}

