using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IUIDisplay
{
    void Show(DisplayData data);

    void Hide();
}
