using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class BaseUnlockerEntry
{
    public int currntIndex;
    public BaseUnlocker baseUnlocker;
    public string baseName;
    public Sprite icon;
    public float baseUnlockCost;
    public bool bIsSubRow = false;
    public bool showSubRows = false; 
    public List<BaseUnlockerEntry> subRows = new List<BaseUnlockerEntry>();
}
[CreateAssetMenu(fileName = "BaseUnlockerTable", menuName = "Game/Base Unlocker Table")]
public class BaseUnlockerTable : ScriptableObject
{
    public int currentIndex = 0;
    public List<BaseUnlockerEntry> entries = new List<BaseUnlockerEntry>();
}
