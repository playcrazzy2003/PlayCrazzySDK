using System;
using System.Collections;
using DG.Tweening;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BaseUnlocker : MonoBehaviour
{
    [Header("Ui Elements")] [SerializeField]
    Transform baseTransform;

    [SerializeField] Image filler;
    [SerializeField] Image baseImage;
    [SerializeField] TMP_Text baseText;
    [SerializeField] TMP_Text baseUnlockCostText;

    [SerializeField] private float flyHeight = 2f;
    [SerializeField] private float flyDuration = 0.5f;
    [SerializeField] private Vector3 Tragetv;
    [SerializeField] private float spwanDalay;
    [SerializeField] private float maxTimeForMoneyReduce;

    [Header("Value"), SerializeField, SaveableField]
    private float baseUnlockCost;

    [SerializeField, SaveableField] int currentIndex;
    [SaveableField] public bool bIsUnlock;
    [SaveableField] public bool bIsUnLockerActive;
    private float defaultUnlockCost;
    private bool bIsSubRow;

    [Header("Base Unlock Objects")] [SerializeField]
    List<GameObject> baseUnlockPrefab = new List<GameObject>();


    private GameManager gameManager;
    private EconomyManager economyManager;
    private PlayerController playerController;
    private UiManager uiManager;
    private BaseUnlockerEntry baseEntryData;
    private SaveDataComponent savedData;
    private BaseUnlocker perentbaseUnlock;

    private void OnEnable()
    {
        gameManager = GameManager.Instance;
        economyManager = gameManager.economyManager;
        playerController = gameManager.playerController;
        uiManager = gameManager.uiManager;
        savedData = GetComponent<SaveDataComponent>();
    }

    #region DATA SETS

    public void UpdateBaseEntry(BaseUnlockerEntry entry)
    {
        baseEntryData = entry;
        if (baseEntryData != null)
        {
            // Updating data
            baseImage.sprite = baseEntryData.icon;
            baseText.text = baseEntryData.baseName.ToUpper();
            baseUnlockCost = baseEntryData.baseUnlockCost;
            defaultUnlockCost = baseEntryData.baseUnlockCost;
            bIsSubRow = baseEntryData.bIsSubRow;

            baseUnlockCostText.text = uiManager.ScoreShow(baseEntryData.baseUnlockCost);
            filler.fillAmount = Mathf.Clamp01(1f - (baseUnlockCost / defaultUnlockCost));

            for (int i = 0; i < baseEntryData.subRows.Count; i++)
            {
                var entrys = baseEntryData.subRows[i];
                if (entrys != null)
                {
                    if (entrys.baseUnlocker != null)
                    {
                        entrys.baseUnlocker.perentbaseUnlock = this;
                        entrys.bIsSubRow = true;
                        entrys.baseUnlocker.UpdateBaseEntry(entrys);
                    }
                    else
                    {
                        Debug.LogError("No baseUnlock found" + entrys.baseName);
                    }
                }
            }


            // load saved game data
            //LoadGame();
            UpdateVisual();
        }
    }

    public void UpdateVisual()
    {
        if (bIsUnlock)
        {
            baseTransform.gameObject.SetActive(false);
            gameManager.SetObjectsStates(baseUnlockPrefab.ToArray(), true);
        }
        else
        {
            gameManager.SetObjectsStates(baseUnlockPrefab.ToArray(), false);

            if (bIsUnLockerActive)
            {
                baseTransform.gameObject.SetActive(false);

                Vector3 lastScale = baseTransform.transform.localScale;
                baseTransform.transform.localScale = Vector3.zero;
                CameraController.OnMoveToTarget.Invoke(baseTransform.transform, 1f, .5f, true, () =>
                {
                    baseTransform.gameObject.SetActive(true);
                    baseTransform.DOScale(lastScale, .3f).SetEase(Ease.OutBounce);
                });
            }
            else
            {
                baseTransform.gameObject.SetActive(false);
            }
        }
        SaveGame();
    }

    void OnUnlock()
    {
        bIsUnlock = true;
        bIsUnLockerActive = false;
        UpdateVisual();

        if (!bIsSubRow)
        {
            gameManager.baseUnlockManager.currentUnlockLevel++;
            gameManager.baseUnlockManager.SetBaseVisuls();
            if (currentIndex < baseEntryData.subRows.Count)
            {
                var entry = baseEntryData.subRows[currentIndex];
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

            currentIndex++;
        }
        else
        {
            if (perentbaseUnlock)
            {

                if (perentbaseUnlock.currentIndex < perentbaseUnlock.baseEntryData.subRows.Count)
                {
                    Debug.LogError("currentIndex is not less" + perentbaseUnlock.currentIndex);
                    var entry = perentbaseUnlock.baseEntryData.subRows[perentbaseUnlock.currentIndex];
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
                else
                {
                    Debug.LogError("currentIndex is not less" + perentbaseUnlock.currentIndex);
                }

                //  perentbaseUnlock.currentIndex++;
            }
            else
            {
                Debug.LogWarning("perentbaseUnlock is not found");

            }
        }
    }

    void SaveGame()
    {
        SaveDataStorage.Save(savedData);
    }

    void LoadGame()
    {
        SaveDataStorage.Load(savedData);
    }

    #endregion

    #region MoneyShowCase

    private Coroutine drainCoroutine;
    private bool bIsPlayerStay = false;

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player") && !playerController.IsMoving() && !bIsPlayerStay)
        {
            bIsPlayerStay = true;

            if (economyManager.totalMoney > 0)
            {
                StratTakinMoney();
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            StopTakeMoney();
            bIsPlayerStay = false;
        }
    }

    void StratTakinMoney()
    {
        if (drainCoroutine == null)
        {
            drainCoroutine = StartCoroutine(DrainMoney());
            StartCoroutine(MoneyDroping());
        }
    }

    void StopTakeMoney()
    {
        if (drainCoroutine != null)
        {
            StopCoroutine(drainCoroutine);
            drainCoroutine = null;
            StopCoroutine(MoneyDroping());
        }
    }

    IEnumerator DrainMoney()
    {
        int remainingAmount = (int)baseUnlockCost;
        float elapsedTime = 0f;
        float moneyPerSecond = baseUnlockCost / maxTimeForMoneyReduce;
        while (remainingAmount > 0)
        {
            if (!bIsPlayerStay || economyManager.totalMoney <= 0)
            {
                StopTakeMoney();
                yield break;
            }

            float deltaTime = Time.deltaTime;
            elapsedTime += deltaTime;
            double amountToTake = moneyPerSecond * deltaTime;
            amountToTake = Mathf.Min((float)amountToTake, (float)economyManager.totalMoney, (float)baseUnlockCost);

            if (economyManager.bCanWeSpendMoney((float)amountToTake))
            {
                economyManager.SpendMoney(amountToTake);
                baseUnlockCost -= (int)amountToTake;

                filler.fillAmount = Mathf.Clamp01(1f - (baseUnlockCost / defaultUnlockCost));
                baseUnlockCostText.text = uiManager.ScoreShow(baseUnlockCost);

                if (baseUnlockCost <= 0)
                {
                    OnUnlock();
                    StopTakeMoney();
                    yield break;
                }
            }
            else
            {
                StopTakeMoney();
                yield break;
            }

            yield return null;
        }

        if (baseUnlockCost <= 0)
        {
            OnUnlock();
            StopTakeMoney();
        }
    }


    IEnumerator MoneyDroping()
    {
        while (true)
        {
            if (baseUnlockCost <= 0 || economyManager.totalMoney <= 0 || !bIsPlayerStay || playerController.IsMoving())
            {
                yield break;
            }

            SpawnAndAnimateMoney(playerController.transform.position + Tragetv, transform.position);
            yield return new WaitForSeconds(spwanDalay);
        }
    }

    void SpawnAndAnimateMoney(Vector3 from, Vector3 to)
    {
        GameObject moneyGO = Instantiate(gameManager.moneyBreak, from, Quaternion.identity);
        Material mat = moneyGO.GetComponentInChildren<MeshRenderer>().material;
        Color baseColor = mat.color;
        mat.color = new Color(baseColor.r, baseColor.g, baseColor.b, 0f);

// Sequence
        Sequence seq = DOTween.Sequence();

        moneyGO.transform.localScale = Vector3.zero;
        seq.Append(moneyGO.transform.DOScale(1f, 0.3f).SetEase(Ease.OutBack));

// Fade in
        seq.Join(mat.DOFade(1f, 0.3f));

// Jump
        seq.Append(moneyGO.transform.DOJump(to, flyHeight, 1, flyDuration).SetEase(Ease.InQuad));

// Fade out & destroy
        seq.Append(mat.DOFade(0f, 0.2f));
        seq.Join(moneyGO.transform.DOScale(0f, 0.2f));
        seq.OnComplete(() => Destroy(moneyGO));
    }

    #endregion
}