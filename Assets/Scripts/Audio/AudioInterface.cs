using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioInterface : MonoBehaviour
{
    public void PlaySound(string clipId)
    {
        AudioController.PlaySound(clipId);
    }
}
