using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMProText = TMPro.TextMeshProUGUI;

public class BuildingExtraBoostDisplay : MonoBehaviour
{
    [SerializeField] private TMProText m_Text;
    [SerializeField] private Transform m_Holder;

    private void Awake()
    {
        Hide();
    }

    public void Show(string text)
    {
        m_Holder.gameObject.SetActive(true);
        UpdateText(text);
    }

    public void UpdateText(string text)
    {
        m_Text.text = text;
    }

    public void Hide()
    {
        m_Holder.gameObject.SetActive(false);
    }
}
