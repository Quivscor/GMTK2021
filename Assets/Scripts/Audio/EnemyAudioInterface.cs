using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAudioInterface : AudioInterface
{
    [SerializeField] private string OnTakeDamageClipId;
    [SerializeField] private string OnDeathClipId;

    private void Awake()
    {
        Enemy enemy = GetComponent<Enemy>();

        enemy.OnDeath += PlayDeathClip;
        enemy.OnTakeDamage += PlayDamageClip;
    }

    public void PlayDamageClip(EnemyEventData e)
    {
        PlaySound(OnTakeDamageClipId);
    }

    public void PlayDeathClip(EnemyEventData e)
    {
        PlaySound(OnDeathClipId);
    }
}
