using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretVisuals : MonoBehaviour
{
    [SerializeField] private ParticleSystem m_OnFireParticles;
    public ParticleSystem OnFireParticles { get => m_OnFireParticles; }

    private void Awake()
    {
        ITurret turret = GetComponent<ITurret>();

        turret.OnTurretFire += PlayFireParticles;
    }

    public void PlayFireParticles()
    {
        OnFireParticles.Play();
    }
}
