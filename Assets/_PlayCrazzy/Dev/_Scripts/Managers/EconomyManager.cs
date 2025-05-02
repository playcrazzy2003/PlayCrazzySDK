using TMPro;
using UnityEngine;
using System;
using Sirenix.OdinInspector;

public class EconomyManager : MonoBehaviour
{
    public event Action OnMoneyUpdate;

    [SerializeField] TMP_Text moneyText;
    [SaveableField] double totalMoney;

    GameManager gameManager;
    UiManager uiManager;

    SaveDataComponent SaveData;

    private void Awake()
    {
        gameManager = GameManager.Instance;
        uiManager = gameManager.uiManager;
    }

    private void Start()
    {
        SaveData = GetComponent<SaveDataComponent>();
        SaveDataStorage.Load(SaveData);
        UpdateMoneyCountUI();
    }

    public void UpdateMoneyCountUI()
    {
        moneyText.text = "" + uiManager.ScoreShow(totalMoney);
        OnMoneyUpdate?.Invoke();
        SaveDataStorage.Save(SaveData);
    }
    
    [Button("Add Economy")]
    public void AddMoney(double amountToIncrease)
    {
        totalMoney += amountToIncrease;
        UpdateMoneyCountUI();
    }
    public void SpendMoney(double amountToReduce)
    {
        totalMoney -= (int)amountToReduce;
        if (totalMoney < 0)
        {
            totalMoney = 0;
        }
        UpdateMoneyCountUI();

    }
    public bool bCanWeSpendMoney(double Amt)
    {
        double i = (float)totalMoney - Amt;
        if (i >= 0)
        {
            return true;
        }
        return false;
    }
   

}