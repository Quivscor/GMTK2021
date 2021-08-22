using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToggleDisplayChanger : MonoBehaviour
{
    public void ToggleDisplay()
    {
        if (gameObject.activeInHierarchy)
            Hide();
        else
            Show();
    }

    private void Show()
    {
        gameObject.SetActive(true);
    }

    private void Hide()
    {
        gameObject.SetActive(false);
    }
}
