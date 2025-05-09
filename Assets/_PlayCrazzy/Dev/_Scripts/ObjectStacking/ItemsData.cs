using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ItemType
{
    None,
    Tool,
    Material,
    Part,
}

[System.Serializable]
public class ItemDetails
{
    public ItemType itemType;
    public int itemCost;
    public Sprite itemIcon;
    public GameObject itemPrefab;
}
[CreateAssetMenu(fileName = "ItemData", menuName = "Game/ItemData")]
public class ItemsData : ScriptableObject
{
    public List<ItemDetails> entries = new List<ItemDetails>();

    public void SetAllVelus()
    {
        foreach (var VARIABLE in entries)
        {
            var item = VARIABLE.itemPrefab.GetComponent<Items>();
            item.itemType = VARIABLE.itemType;
            item.price = VARIABLE.itemCost;
            item.icon = VARIABLE.itemIcon;
        }
    } 
}
