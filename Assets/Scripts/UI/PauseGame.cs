using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace LinkTD.UI
{
    public class PauseGame : MonoBehaviour
    {
        bool isPlaying = true;

        [SerializeField] private Sprite pauseSprite;
        [SerializeField] private Sprite playSprite;
        [SerializeField] private Image image;

        public void TogglePause()
        {
            if (isPlaying)
            {
                isPlaying = false;
                Time.timeScale = 0;
                image.sprite = playSprite;
            }
            else
            {
                isPlaying = true;
                Time.timeScale = 1;
                image.sprite = pauseSprite;
            }
        }
    }
}

