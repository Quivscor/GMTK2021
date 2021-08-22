using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMProText = TMPro.TextMeshProUGUI;

namespace LinkTD.UI
{
    public class StatDisplayModuleControler : MonoBehaviour
    {
        [SerializeField] private GameObject m_Holder;

        [SerializeField] private TMProText[] m_StatTexts;
        [SerializeField] private GameObject[] m_StatHolders;

        public void Show(IModule module)
        {
            m_Holder.SetActive(true);

            string symbol = "";
            if (!module.ConnectionData.IsBoostAdditive)
                symbol = "x";
            string sign = "";
            
            for(int i = 0; i < m_StatTexts.Length; i++)
            {
                float value = GetStat(module, i);
                if (value != 0)
                {
                    m_StatHolders[i].SetActive(true);
                    if (value > 0)
                        sign = "+";
                    else
                        sign = "";

                    m_StatTexts[i].text = symbol + sign + value + GetStatText(i);
                }
                else
                    m_StatHolders[i].SetActive(false);
            }
        }

        public float GetStat(IModule module, int index)
        {
            switch(index)
            {
                case 0:
                    return module.ConnectionData.ConnectionBoost.power;
                case 1:
                    return module.ConnectionData.ConnectionBoost.frequency;
                case 2:
                    return module.ConnectionData.ConnectionBoost.electricUsage;
                case 3:
                    return module.ConnectionData.ConnectionBoost.resistance;
            }
            return Mathf.NegativeInfinity;
        }

        public string GetStatText(int index)
        {
            switch (index)
            {
                case 0:
                    return " Power";
                case 1:
                    return " Speed";
                case 2:
                    return " Energy consumption";
                case 3:
                    return " Resistance";
            }
            return " Undefined";
        }

        public void Hide()
        {
            m_Holder.SetActive(false);
        }
    }
}

