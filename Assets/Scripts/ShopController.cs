using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LinkTD.UI
{
    public class ShopController : MonoBehaviour
    {
        private Dictionary<int, Building> m_ShopInventory;

        [SerializeField] private BuildingSelector m_SelectorPrefab;

        [SerializeField] private Building[] m_EditorList;
        [SerializeField] private Transform[] m_ShopTabs;

        private void Awake()
        {
            BuildDictionary();
            InstantiateBuildingSelectors();
            HideShopTabs();
        }

        private void BuildDictionary()
        {
            m_ShopInventory = new Dictionary<int, Building>();

            for(int i = 0; i < m_EditorList.Length; i++)
            {
                BuildingShopData data = m_EditorList[i].BuildingShopData;
                //buildingShopCategory increments by 1000, there shouldn't be any overlap in the future
                m_ShopInventory.Add((int)data.BuildingShopCategory + i, m_EditorList[i]);
            }
        }

        private void InstantiateBuildingSelectors()
        {
            foreach(var item in m_ShopInventory)
            {
                int index = Mathf.FloorToInt(item.Key / 1000f); //again, buildingShopCategory increments
                BuildingSelector selector = Instantiate(m_SelectorPrefab, m_ShopTabs[index]);
                selector.Construct(item.Value.BuildingShopData, item.Key, SelectShopBuilding);
            }
        }

        private void HideShopTabs()
        {
            for(int i = 1; i < m_ShopTabs.Length; i++)
            {
                m_ShopTabs[i].gameObject.SetActive(false);
            }
        }

        public void ChangeTabs(int targetTabIndex)
        {
            for(int i = 0; i < m_ShopTabs.Length; i++)
            {
                if (i == targetTabIndex)
                    m_ShopTabs[i].gameObject.SetActive(true);
                else
                    m_ShopTabs[i].gameObject.SetActive(false);
            }
        }

        public void SelectShopBuilding(int key)
        {
            Building b;
            if (m_ShopInventory.TryGetValue(key, out b))
                SelectionManager.Select(b);
        }
    }
}