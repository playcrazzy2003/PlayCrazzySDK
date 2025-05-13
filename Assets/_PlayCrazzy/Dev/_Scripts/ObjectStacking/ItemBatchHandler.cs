using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using DG.Tweening;
using Unity.VisualScripting;

public class ItemBatchHandler : MonoBehaviour
{
    [Header("Stacking visual setting")] [SerializeField]
    float numberOfEmptyObj;

    [SerializeField] float stackingSpacesPerObj;
    [SerializeField] private Rigidbody baseStackingRigidbody;
    [SerializeField] private GameObject emptyStackingObj;

    [Header("Stacker Settings")] public int maxCapacity;
    public List<Transform> itemSlots = new List<Transform>();

    [Header("UI")] public GameObject fullIndicatorCanvas;
    public TMP_Text liveCountText;

    protected List<Items> currentItems = new();
    public bool CanReceive() => currentItems.Count < maxCapacity;
    public bool CanGive() => currentItems.Count > 0;

    private void Start()
    {
        for (int i = 0; i < numberOfEmptyObj; i++)
        {
            GameObject emptyObj = Instantiate(emptyStackingObj, transform.position, Quaternion.identity, transform);
            emptyObj.SetActive(false);
            itemSlots.Add(emptyObj.transform);
            if (i == 0)
            {
                Vector3 pos = baseStackingRigidbody.transform.position;
                pos.y += stackingSpacesPerObj;
                emptyObj.transform.position = pos;
                emptyObj.GetComponent<ConfigurableJoint>().connectedBody = baseStackingRigidbody;
            }
            else
            {
                Vector3 pos = itemSlots[i - 1].transform.position;
                pos.y += stackingSpacesPerObj;
                emptyObj.transform.position = pos;
                emptyObj.GetComponent<ConfigurableJoint>().connectedBody = itemSlots[i - 1].GetComponent<Rigidbody>();
            }
            emptyObj.SetActive(true);

        }
    }

    private Coroutine transferCoroutine;
    private ItemBatchStacker currentInteractingStacker;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("ItemBatchStacker"))
        {
            ItemBatchStacker stacker = other.GetComponent<ItemBatchStacker>();
            if (stacker == null) return;


            currentInteractingStacker = stacker;

            if (stacker.stackerType == BatchStackerType.Receiver)
            {
                HandleItemsGive(currentInteractingStacker);
            }
            else if (stacker.stackerType == BatchStackerType.Giver)
            {
                HandleItemsReceive(currentInteractingStacker);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("ItemBatchStacker")) return;

        ItemBatchStacker stacker = other.GetComponent<ItemBatchStacker>();
        if (stacker == null || stacker != currentInteractingStacker) return;

        if (transferCoroutine != null)
        {
            StopCoroutine(transferCoroutine);
            transferCoroutine = null;
        }

        currentInteractingStacker = null;
    }

    public Items GiveItemOfType(ItemType type)
    {
        for (int i = currentItems.Count - 1; i >= 0; i--)
        {
            if (currentItems[i].itemType == type)
            {
                Items foundItem = currentItems[i];
                currentItems.RemoveAt(i);

                // Optionally reorganize stack (reload list)
                ReloadItemsVisuals();

                UpdateUI();
                return foundItem;
            }
        }

        return null;
    }
    
    

    private void ReloadItemsVisuals()
    {
        for (int i = 0; i < currentItems.Count; i++)
        {
            PositionItem(currentItems[i].gameObject, i);
        }
    }
    public void HandleItemsReceive(ItemBatchStacker itemBatchStacker)
    {
        if (!CanReceive() || !itemBatchStacker.CanGive()) return;
        if (transferCoroutine == null)
            transferCoroutine = StartCoroutine(ReceiveItemsRoutine(itemBatchStacker));
    }

    public void HandleItemsGive(ItemBatchStacker itemBatchStacker)
    {
        if (!CanGive() || !itemBatchStacker.CanReceive()) return;
        if (transferCoroutine == null)
            transferCoroutine = StartCoroutine(GiveItemsRoutine(itemBatchStacker));
    }

    private IEnumerator ReceiveItemsRoutine(ItemBatchStacker itemBatchStacker)
    {
        while (CanReceive() && itemBatchStacker.CanGive())
        {
            Items item = itemBatchStacker.GiveItem();
            if (item == null) break;

            GameObject obj = item.gameObject;
            obj.SetActive(true);

            Transform targetSlot = itemSlots[currentItems.Count];
            ReceiveItem(item); // this will parent and align it
            yield return AnimateItemJump(obj.transform, targetSlot);

        }
    }
    

    private IEnumerator GiveItemsRoutine(ItemBatchStacker itemBatchStacker)
    {
        while (CanGive() && itemBatchStacker.CanReceive())
        {
            Items item = GiveItemOfType(itemBatchStacker.itemType);
            Debug.LogError("oyoy");
            if (item == null) break;

            GameObject obj = item.gameObject;
            obj.SetActive(true);

            Transform targetSlot = itemBatchStacker.itemSlots[itemBatchStacker.currentItems.Count];
            yield return AnimateItemJump(obj.transform, targetSlot);

            itemBatchStacker.ReceiveItem(item); // this will parent and align it
        }
    }


    public void ReceiveItem(Items newItem)
    {
        if (!CanReceive()) return;
        currentItems.Add(newItem);
        PositionItem(newItem.gameObject, currentItems.Count - 1);
        UpdateUI();
    }

    public Items GiveItem()
    {
        if (!CanGive()) return null;

        var item = currentItems[^1];
        currentItems.RemoveAt(currentItems.Count - 1);
        UpdateUI();
        return item;
    }

    protected void PositionItem(GameObject obj, int index)
    {
        if (index >= itemSlots.Count)
        {
            Debug.LogWarning("Not enough item slot transforms defined.");
            return;
        }

        obj.transform.SetParent(itemSlots[index]);
        obj.transform.localPosition = Vector3.zero;
        obj.transform.localRotation = Quaternion.identity;
    }

    protected void UpdateUI()
    {
        bool isFull = currentItems.Count >= maxCapacity;

        if (liveCountText)
            liveCountText.text = isFull ? "MAX" : $"{currentItems.Count}/{maxCapacity}";

        if (fullIndicatorCanvas)
            fullIndicatorCanvas.SetActive(isFull);
    }

    private IEnumerator AnimateItemJump(Transform item, Transform targetSlot)
    {
        float duration = 0.3f;
        float jumpPower = 1.5f;

        // Jump and rotate simultaneously
        var moveTween = item.DOJump(targetSlot.position, jumpPower, 1, duration)
            .SetEase(Ease.OutQuad);

        var rotateTween = item.DORotate(new Vector3(0, 360, 0), duration, RotateMode.FastBeyond360)
            .SetEase(Ease.OutQuad);

        // Wait until both tweens complete
        yield return moveTween.WaitForCompletion();

        // Set parent and reset local transform
        item.SetParent(targetSlot);
        item.localPosition = Vector3.zero;
        item.localRotation = Quaternion.identity;
    }
    
    
}