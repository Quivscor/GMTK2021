using System.Collections.Generic;
using System;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public int pointCost = 1; //default to 1, so any enemy costs at least something

    #region EnemyData
    [SerializeField] private float hp;
    private float currentHp;
    [SerializeField] private int damage;
    #endregion

    #region TraversalData
    private Vector3 previousNode;
    private Vector3 nextNode;
    private int currentNodeIndex = 0;
    private float currentTime = 0;
    public float moveSpeed = 1;
    #endregion

    #region Events
    public Action OnReachDestination;
    public Action<Enemy> OnDeath;
    public Action OnSpawn;
    #endregion

    private void Start()
    {
        previousNode = transform.position;
        nextNode = EnemyController.Instance.EnemyPath.PathNodes[currentNodeIndex].transform.position;

        //gives extra delay on spawn
        currentTime = 1 - Vector3.Distance(previousNode, nextNode);

        currentHp = hp;
    }

    private void Update()
    {
        currentTime += Time.deltaTime;
        transform.position = Vector3.Lerp(previousNode, nextNode, currentTime / moveSpeed);
        
        if(currentTime >= 1)
        {
            previousNode = nextNode;
            currentNodeIndex++;
            if (currentNodeIndex < EnemyController.Instance.EnemyPath.PathNodes.Count)
                nextNode = EnemyController.Instance.EnemyPath.PathNodes[currentNodeIndex].transform.position;
            else
                OnReachDestination?.Invoke();

            currentTime = 0;
        }
    }

    public void TakeDamage(float damage)
    {
        currentHp -= damage;

        if (currentHp <= 0)
            Die();
    }

    public void Die()
    {
        OnDeath?.Invoke(this);
    }
}