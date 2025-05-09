using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public enum ItemsLevelType
{
    Batch,
    NonBatch
}

[Serializable]
public class Level
{
    public GameObject perentGameObj;
    public List<GameObject> batchObjects = new();
    public List<GameObject> nonBatchObjects = new();
    public List<GameObject> exceptionObjects = new();
}

public class Items : MonoBehaviour
{
    [Header("Item Settings")]
    public Sprite icon;
    public ItemType itemType;
    public float price;
    public int visibilityLevel;

    [Header("Level Data")]
    public Level[] levels;

    [Header("Runtime Flags")]
    public ItemsLevelType itemsLevelType;
    [HideInInspector] public bool showExceptions;

    private void Start()
    {
        // Hide all parent containers at start
        SetBatchParentActive();
        ShowObjects();

    }

    /// <summary>
    /// Shows or hides objects based on current item level type and visibility level.
    /// </summary>
    
    [Button("ShowObjects")]
    public void ShowObjects()
    {
        if (levels == null || visibilityLevel < 0 || visibilityLevel >= levels.Length)
        {
            Debug.LogWarning($"Invalid visibilityLevel: {visibilityLevel}");
            return;
        }

        
        Level currentLevel = levels[visibilityLevel];
        // Deactivate the parent container first
        SetBatchParentActive();
        currentLevel.perentGameObj.SetActive(true);
        
        // Deactivate all objects
        SetObjectsActive(currentLevel.batchObjects, false);
        SetObjectsActive(currentLevel.nonBatchObjects, false);
        SetObjectsActive(currentLevel.exceptionObjects, false);

        // Reactivate objects based on item type
        switch (itemsLevelType)
        {
            case ItemsLevelType.Batch:
                SetObjectsActive(currentLevel.batchObjects, true);
                break;
            case ItemsLevelType.NonBatch:
                SetObjectsActive(currentLevel.nonBatchObjects, true);
                break;
        }

        // Optionally reactivate exception objects
        if (showExceptions)
        {
            SetObjectsActive(currentLevel.exceptionObjects, true);
        }

        // Activate the parent container again
    }

    /// <summary>
    /// Utility to set active state for a list of GameObjects.
    /// </summary>
    private void SetObjectsActive(List<GameObject> objects, bool isActive)
    {
        if (objects == null) return;

        foreach (var obj in objects)
        {
            if (obj != null)
                obj.SetActive(isActive);
        }
    }

    /// <summary>
    /// Utility to activate or deactivate the parent of the first batch object.
    /// </summary>
    private void SetBatchParentActive()
    {
        foreach (var level in levels)
        {
            if (level.perentGameObj != null)
            {
                level.perentGameObj.SetActive(false);
            }
        }
    }
}
