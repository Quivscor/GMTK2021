using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


public class ResourcesController : MonoBehaviour
{
    [SerializeField] private int m_Lives = 20;
    [SerializeField] private int m_Money = 100;

    public int Lives => m_Lives;
    public int Money => m_Money;

    public Action<int> OnMoneyChange;
    public Action<int> OnLivesChange;

    [SerializeField] private GameObject gameOverCanvas;

    public void SubscribeGenerator(IGenerator generator)
    {
        generator.OnGenerate += GainMoney;
    }

    private void Start()
    {
        OnLivesChange?.Invoke(Lives);
        OnMoneyChange?.Invoke(Money);
    }

    public void RemoveLife(EnemyEventData e)
    {
        m_Lives -= e.Enemy.damage;
        if (m_Lives <= 0)
            GameOver();
        else
            OnLivesChange?.Invoke(m_Lives);
    }

    public bool TrySpendMoney(int value)
    {
        if (value > m_Money)
            return false;

        m_Money -= value;
        OnMoneyChange?.Invoke(m_Money);
        return true;
    }

    public void GainMoney(EnemyEventData e)
    {
        m_Money += e.Enemy.currentMoney;
        OnMoneyChange?.Invoke(m_Money);
    }

    public void GainMoney(GeneratorEventData e)
    {
        m_Money += Mathf.RoundToInt(e.GeneratedValue);
        OnMoneyChange?.Invoke(m_Money);
    }

    public void GameOver()
    {
        gameOverCanvas.SetActive(true);
        Time.timeScale = 0;
    }
}
