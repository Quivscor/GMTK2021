using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public EnemyPath EnemyPath { get; private set; }

    private void Start()
    {
        EnemyPath = new EnemyPath();
        EnemyPath.ConstructPath();
    }


    void Update()
    {
        
    }
}
