using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class BaseUnlockerEntry
{
  public int currntIndex;
  public BaseUnlocker baseUnlocker;
  public string baseName;
  public Sprite icon;
  public float baseUnlockCost;

  public bool showSubRows = false; 
  public List<BaseUnlockerEntry> subRows = new List<BaseUnlockerEntry>();
}
public class BaseUnlocker : MonoBehaviour
{
  [Header("Ui Elements")]
  [SerializeField] RectTransform baseRectTransform;
  [SerializeField] Image baseImage;
  [SerializeField] TMP_Text baseText;
  [SerializeField] TMP_Text baseUnlockCostText;
 
  [Header("Value"), SerializeField,SaveableField]
  private float baseUnlockCost;
  
  [Header("Base Unlock Objects")]
  [SerializeField] List<GameObject> baseUnlockPrefab = new List<GameObject>();
  
  
  private GameManager gameManager;
  
  
}
