using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMProText = TMPro.TextMeshProUGUI;

namespace LinkTD.UI
{
    public class StatDisplayGenerationControler : MonoBehaviour
    {
        [SerializeField] private GameObject m_Holder;

        [SerializeField] private TMProText m_GenerationTimeText;
        [SerializeField] private TMProText m_GeneratedObjectText;
        [SerializeField] private Image m_GenerationSliderFill;

        public void Show(IGenerator generator)
        {
            m_Holder.SetActive(true);

            m_GenerationTimeText.text = "Generation time: " + generator.GenerationCooldown.ToString("F2");
            m_GeneratedObjectText.text = "Producing: " + generator.GeneratedObjectName;

            m_GenerationSliderFill.fillAmount = 1 - (generator.GenerationCooldownCurrent / generator.GenerationCooldown);
        }

        public void Hide()
        {
            m_Holder.SetActive(false);
        }
    }
}

