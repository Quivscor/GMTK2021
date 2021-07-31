using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyVisuals : MonoBehaviour
{
    [SerializeField] private ParticleSystem m_OnDamageParticles;
    public ParticleSystem OnDamageParticles { get => m_OnDamageParticles; }

    private void Awake()
    {
        Enemy e = GetComponent<Enemy>();

        e.OnTakeDamage += PlayDamageParticles;
    }

    public void PlayDamageParticles(EnemyEventData e)
    {
        OnDamageParticles.Play();
    }
}
