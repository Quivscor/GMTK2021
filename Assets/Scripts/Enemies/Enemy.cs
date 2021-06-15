using System.Collections.Generic;
using System;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public int pointCost = 1; //default to 1, so any enemy costs at least something
    public int money = 2;

    #region EnemyData
    [SerializeField] private float hp;
    private float currentHp;
    public int damage;
    [SerializeField] private float perWaveIncrease = 0.05f;
    #endregion

    #region TraversalData
    private Vector3 previousNode;
    private Vector3 nextNode;
    private int currentNodeIndex = 0;
    public float CurrentTime { get; private set; } = 0;
    public float moveSpeed = 1;
    #endregion

    #region Events
    public Action<Enemy> OnReachDestination;
    public Action<Enemy> OnDeath;
    public Action OnSpawn;
    #endregion

    private ParticleSystem damageParticles;
    private AudioSource source;

    private void Start()
    {
        damageParticles = GetComponentInChildren<ParticleSystem>();
        source = GetComponent<AudioSource>();
        //CurrentTime = 1 - Vector3.Distance(previousNode, nextNode);
    }

    public void Construct(int waveNumber)
    {
        previousNode = transform.position;
        nextNode = EnemyController.EnemyPath.PathNodes[currentNodeIndex].transform.position;
        currentHp = hp * (1 + (waveNumber * perWaveIncrease));
        //gives extra delay on spawn
        CurrentTime = 1 - Vector3.Distance(previousNode, nextNode);
    }

    private void Update()
    {
        CurrentTime += Time.deltaTime;
        transform.position = Vector3.Lerp(previousNode, nextNode, CurrentTime / moveSpeed);
        
        if(CurrentTime >= 1)
        {
            previousNode = nextNode;
            currentNodeIndex++;
            if (currentNodeIndex < EnemyController.EnemyPath.PathNodes.Count)
                nextNode = EnemyController.EnemyPath.PathNodes[currentNodeIndex].transform.position;
            else
                OnReachDestination?.Invoke(this);

            CurrentTime = 0;
        }
    }

    public void TakeDamage(float damage)
    {
        currentHp -= damage;
        damageParticles.Play();

        if (currentHp <= 0)
            Die();
    }

    public void Die()
    {
        OnDeath?.Invoke(this);
        source.PlayOneShot(source.clip);
    }
}