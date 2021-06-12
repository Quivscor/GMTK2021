using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class TurretEnemyDetector : MonoBehaviour
{
    public Action<Enemy> OnEnemyEnterRange;
    public Action<Enemy> OnEnemyExitRange;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Enemy"))
        {
            Enemy e = collision.GetComponent<Enemy>();
            if(e != null)
            {
                OnEnemyEnterRange?.Invoke(e);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            Enemy e = collision.GetComponent<Enemy>();
            if (e != null)
            {
                OnEnemyExitRange?.Invoke(e);
            }
        }
    }
}
