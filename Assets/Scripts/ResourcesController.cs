using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


public class ResourcesController : MonoBehaviour
{
    [SerializeField] private int lives = 20;
    [SerializeField] private int money = 100;

    public Action<int> OnMoneyChange;
    public Action<int> OnLivesChange;

    [SerializeField] private GameObject gameOverCanvas;

    public void RemoveLife(EnemyEventData e)
    {
        lives -= e.Enemy.damage;
        if (lives <= 0)
            GameOver();
        else
            OnLivesChange?.Invoke(lives);
    }

    public bool TrySpendMoney(int value)
    {
        if (value > money)
            return false;

        money -= value;
        OnMoneyChange?.Invoke(money);
        return true;
    }

    public void GainMoney(EnemyEventData e)
    {
        money += e.Enemy.money;
        OnMoneyChange?.Invoke(money);
    }

    public void GameOver()
    {
        gameOverCanvas.SetActive(true);
        Time.timeScale = 0;
    }
}
