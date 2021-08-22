using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMProText = TMPro.TextMeshProUGUI;

namespace LinkTD.UI
{
    public class StatDisplayGeneralStatsControler : MonoBehaviour
    {
        [SerializeField] private GameObject m_Holder;

        [SerializeField] private Image m_BuildingImage;
        [SerializeField] private TMProText m_BuildingTitle;
        [SerializeField] private TMProText m_BuildingDescription;

        public void Show(BuildingShopData data)
        {
            m_Holder.SetActive(true);

            m_BuildingImage.sprite = data.UIBuildingSprite;
            m_BuildingTitle.text = data.UIBuildingName;
            m_BuildingDescription.text = data.UIBuildingDescription;
        }

        public void Hide()
        {
            m_Holder.SetActive(false);
        }
    }
}

