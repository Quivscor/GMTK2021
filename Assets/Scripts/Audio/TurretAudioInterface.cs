using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class TurretAudioInterface : AudioInterface
{
    [SerializeField] private string m_OnTurretFireClipId;
    public string OnTurretFireClipId { get => m_OnTurretFireClipId; }

    private void Awake()
    {
        ITurret turret = GetComponent<ITurret>();

        turret.OnTurretFire += PlayFireSound;
    }

    public void PlayFireSound()
    {
        PlaySound(OnTurretFireClipId);
    }
}
