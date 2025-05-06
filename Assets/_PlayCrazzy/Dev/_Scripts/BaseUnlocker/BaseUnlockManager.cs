using System;
using UnityEngine;
using System.Collections.Generic;

public class BaseUnlockManager : MonoBehaviour
{
    [Space(3)] [Header("Values")] [SaveableField]
    public int currentUnlockLevel = 0;


    [Space(3)] [Header("References")] [SerializeField]
    public List<BaseUnlockerEntry> entries = new List<BaseUnlockerEntry>();


    private SaveDataComponent saveDataComponent;

    private void Start()
    {
        saveDataComponent = GetComponent<SaveDataComponent>();
        SaveDataStorage.Load(saveDataComponent);
        UpdateData();
    }

    void UpdateData()
    {
        for (int i = 0; i < entries.Count; i++)
        {
            var entrys = entries[i];
            if (entrys != null)
            {
                if (entrys.baseUnlocker != null)
                {
                    entrys.baseUnlocker.UpdateBaseEntry(entrys);
                }
                else
                {
                    Debug.LogError("No baseUnlock found" + entrys.baseName);
                }
            }
        }

        SetBaseVisuls();
    }

    public void SetBaseVisuls()
    {
        if (currentUnlockLevel < entries.Count)
        {
            var entry = entries[currentUnlockLevel];
            if (entry != null)
            {
                if (entry.baseUnlocker != null)
                {
                    entry.baseUnlocker.bIsUnLockerActive = true;
                    entry.baseUnlocker.UpdateVisual();
                }
                else
                {
                    Debug.LogError("No baseUnlock found" + entry.baseName);
                }
            }
        }
        SaveDataStorage.Save(saveDataComponent);
    }
}