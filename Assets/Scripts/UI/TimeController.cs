using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LinkTD.UI
{
    public class TimeController : MonoBehaviour
    {
        private static float m_TimeScale;
        public static float TimeScale
        {
            get => m_TimeScale;
            private set
            {
                m_TimeScale = value;
                Time.timeScale = m_TimeScale;
            }
        }
        private int m_CurrentGameSpeedIterator;
        [SerializeField] private float[] m_GameSpeedSettings;

        private void Start()
        {
            m_TimeScale = Time.timeScale;
        }

        public void PauseGame()
        {
            if (TimeScale == 0)
                TimeScale = m_GameSpeedSettings[m_CurrentGameSpeedIterator];
            else
                TimeScale = 0;
        }

        public void ChangeGameSpeed()
        {
            m_CurrentGameSpeedIterator++;
            if (m_CurrentGameSpeedIterator >= m_GameSpeedSettings.Length)
                m_CurrentGameSpeedIterator = 0;

            TimeScale = m_GameSpeedSettings[m_CurrentGameSpeedIterator];
        }
    }

}
