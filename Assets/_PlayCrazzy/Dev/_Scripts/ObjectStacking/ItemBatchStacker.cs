using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public enum BatchStackerType
{
    Giver,
    Receiver,
}

public class ItemBatchStacker : MonoBehaviour
{
    [Header("Temp DiplayObj")] public GameObject nig;

    [Header("Stacker Settings")] public ItemType itemType;
    public BatchStackerType stackerType;
    [SerializeField] private int maxCapacity;

    public List<Transform> itemSlots = new List<Transform>();

    [Header("UI")] public GameObject fullIndicatorCanvas;
    public TMP_Text liveCountText;

    internal List<Items> currentItems = new();
    private ItemGridBuilder _itemGridBuilder;

    /// <summary>
    /// Checks if the stacker can receive more items.
    /// </summary>
    public bool CanReceive() => currentItems.Count < maxCapacity;

    /// <summary>
    /// Checks if the stacker has any items to give.
    /// </summary>
    public bool CanGive() => currentItems.Count > 0;

    /// <summary>
    /// Receives an item and places it into the next available slot.
    /// </summary>
    /// <param name="newItem">The item to receive.</param>
    private void Start()
    {
        _itemGridBuilder = gameObject.GetComponentInChildren<ItemGridBuilder>();
        if (_itemGridBuilder != null)
        {
            _itemGridBuilder.blockPrefab = new GameObject("emptyObj");
            _itemGridBuilder.BuildSelectedPattern();
            itemSlots.AddRange(_itemGridBuilder.gridData.transforms);
            ShowDisplayObjects();
        }
    }

    public void ReceiveItem(Items newItem)
    {
        if (!CanReceive() || newItem == null)
            return;

        currentItems.Add(newItem);
        PositionItem(newItem.gameObject, currentItems.Count - 1);
        UpdateUI();
    }

    /// <summary>
    /// Removes and returns the last item in the stack.
    /// </summary>
    /// <returns>The item removed, or null if none.</returns>
    public Items GiveItem()
    {
        if (!CanGive())
            return null;

        int lastIndex = currentItems.Count - 1;
        Items item = currentItems[lastIndex];
        currentItems.RemoveAt(lastIndex);
        UpdateUI();
        return item;
    }

    /// <summary>
    /// Positions the item prefab at the specified index's slot.
    /// </summary>
    /// <param name="obj">The item GameObject to position.</param>
    /// <param name="index">Index in the slot list.</param>
    protected void PositionItem(GameObject obj, int index)
    {
        if (index >= itemSlots.Count)
        {
            Debug.LogWarning("Not enough item slot transforms defined.");
            return;
        }

        Transform slot = itemSlots[index];
        obj.transform.SetParent(slot, false);
        obj.transform.localPosition = Vector3.zero;
        obj.transform.localRotation = Quaternion.identity;
    }

    /// <summary>
    /// Updates the UI to reflect current item count and full state.
    /// </summary>
    protected void UpdateUI()
    {
        bool isFull = currentItems.Count >= maxCapacity;

        if (liveCountText != null)
            liveCountText.text = isFull ? "MAX" : $"{currentItems.Count}/{maxCapacity}";

        if (fullIndicatorCanvas != null)
            fullIndicatorCanvas.SetActive(isFull);
    }

    /// <summary>
    /// Displays any additional visual elements (placeholder for extension).
    /// </summary>
    public void ShowDisplayObjects()
    {
        if (nig != null)
        {
            for (int i = 0; i < maxCapacity; i++)
            {
                GameObject obj = Instantiate(nig);
                Items item = obj.GetComponent<Items>();
                ReceiveItem(item);
            }
        }
    }
}