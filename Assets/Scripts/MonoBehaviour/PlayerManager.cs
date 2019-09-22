using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayerManager : MonoBehaviour
{
    public static PlayerManager Instance;
    private void Awake()
    {
        if (!Instance) Instance = this;
    }
    [System.Serializable]
    public class GoldEvent : UnityEvent<int> { }

    public UnityEngine.UI.Text goldText, lifeText;

    public GoldEvent OnGoldGain = new GoldEvent(), OnGoldLoss = new GoldEvent();
    public UnityEvent OnHealthLoss;

    public int gold = 250;
    public int life = 9;

    public void HealthLoss()
    {
        life--;
        OnHealthLoss.Invoke();
    }

    public void Pay(int cost)
    {
        gold -= cost;
        OnGoldLoss.Invoke(cost);
        
    }
    public void Gain(int price)
    {
        gold += price;
        OnGoldGain.Invoke(price);
    }

    public void GainRandom()
    {
        Gain(Random.Range(3, 30));
    }

    private void Update()
    {
        UpdateUI();
    }

    void UpdateUI()
    {
        goldText.text = "Gold: " + gold.ToString();
        lifeText.text = "PrincECS " + life.ToString();
    }

}
