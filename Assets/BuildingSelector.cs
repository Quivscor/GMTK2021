using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMProText = TMPro.TextMeshProUGUI;
using System;

namespace LinkTD.UI
{
    public class BuildingSelector : MonoBehaviour
    {
        [SerializeField] private Image m_Image;
        [SerializeField] private TMProText m_TitleText;
        [SerializeField] private TMProText m_DescriptionText;

        private int m_BuildingIndex;

        private void Awake()
        {
            if (m_Image == null || m_DescriptionText == null || m_TitleText == null)
                Debug.LogError("Missing ref!");
        }

        public void Construct(BuildingShopData data, int index, Action<int> selectBuilding)
        {
            m_Image.sprite = data.UIBuildingSprite;
            m_TitleText.text = data.UIBuildingName;
            m_DescriptionText.text = data.UIBuildingDescription;

            m_BuildingIndex = index;

            RegisterListener(selectBuilding);
        }

        private void RegisterListener(Action<int> selectBuilding)
        {
            Button b = GetComponent<Button>();
            b.onClick.AddListener(() => selectBuilding(m_BuildingIndex));
        }

        public void SelectBuilding()
        {
            //callback to shop manager
        }
    }
}

