using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMProText = TMPro.TextMeshProUGUI;

namespace LinkTD.UI 
{
    public class IterativeDisplayChanger : MonoBehaviour
    {
        private int m_Iterator;

        [SerializeField] private string[] m_TextValues;
        [SerializeField] private Sprite[] m_ImageValues;

        [SerializeField] private Image m_Image;
        private TMProText m_Text;

        protected void Awake()
        {
            m_Iterator = -1;

            if (m_TextValues.Length > 0)
                m_Text = GetComponentInChildren<TMProText>();
            if (m_ImageValues.Length > 0 && m_Image == null)
                m_Image = GetComponentInChildren<Image>();
        }

        protected void Start()
        {
            ChangeDisplay();
        }

        public void ChangeDisplay()
        {
            m_Iterator++;
            if ((m_Iterator >= m_TextValues.Length && m_TextValues.Length > 0) ||
                (m_Iterator >= m_ImageValues.Length && m_ImageValues.Length > 0))
            {
                m_Iterator = 0;
            }

            if(m_Image != null)
                m_Image.sprite = m_ImageValues[m_Iterator];
            if(m_Text != null)
                m_Text.text = m_TextValues[m_Iterator];
        }
    }
}

