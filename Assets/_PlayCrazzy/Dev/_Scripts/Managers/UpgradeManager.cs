using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public enum UpgradeType
{
    Manager,
    Staff
}
public class UpgradeManager : MonoBehaviour
{
    public static UpgradeManager Instance;

    private string SaveKey => $"{gameObject.name}_Data_{SceneManager.GetActiveScene().name}";

    [System.Serializable]
    public class CommonProperties
    {
        public Transform mainT;
        public TMP_Text headingTxt;
        public Image mainImgBg;
        public Image upfillImg;
        public Button cashBtn;
        public TMP_Text cashTxt;
        public Button rvBtn;
        public Transform maxBtn;
    }

    [System.Serializable]
    public class UpgradeOption
    {
        public string heading;
        public Sprite mainSprite;
        public int currUpVal;
        public float upGradeMultiplier;
        public int maxUpVal;
        public int cashAmount;
        public float cashAmountUpGradeMultiplier;
        public float value;
    }

    [System.Serializable]
    public class UpgraderData
    {
        public List<UpgradeOption> managerOptions = new List<UpgradeOption>();
        public List<UpgradeOption> staffOptions = new List<UpgradeOption>();
    }

    public UpgradeType currType;

    [SerializeField] Transform profilePanel;
    [SerializeField] Transform mainBgPanel;
    public CommonProperties[] commonProperties;
    public UpgradeOption[] managerOptions;
    public UpgradeOption[] staffOptions;

    [SerializeField] Transform selectionImgT;
    [SerializeField] Button managerBtn;
    [SerializeField] TMP_Text managerTxt;
    [SerializeField] Button staffBtn;
    [SerializeField] TMP_Text staffTxt;
    [SerializeField] Button profileBtn;
    [SerializeField] Button closeBtn;

    [SerializeField] float valPerUpgrade;
    EconomyManager ecoManager;
    PlayerController playerController;
    ItemBatchHandler playerObjectStacking;
    GameManager gameManager;
    GameObject helperPrefab;

    private void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        gameManager = GameManager.Instance;
        ecoManager = gameManager.economyManager;
        playerController = gameManager.playerController;
        playerObjectStacking = playerController.gameObject.GetComponentInChildren<ItemBatchHandler>();

        LodaData();
        LoadButtons();
        //helperPrefab = gameManager.helperGO;
    }


    void  LoadUiOptions()
    {
        switch (currType)
        {
            case UpgradeType.Manager:

                for (int i = 0; i < managerOptions.Length; i++)
                {
                    var managerOption = managerOptions[i];
                    CommonLoad(managerOption, i);
                }

                break;

            case UpgradeType.Staff:

                //CommonLoad(staffOptions);
                for (int i = 0; i < staffOptions.Length; i++)
                {
                    var staffOption = staffOptions[i];
                    CommonLoad(staffOption, i);
                }

                break;
        }
    }

    void CommonLoad(UpgradeOption upgradeOverlays, int index)
    {
        commonProperties[index].headingTxt.text = upgradeOverlays.heading;
        commonProperties[index].upfillImg.DOFillAmount(upgradeOverlays.currUpVal * valPerUpgrade, .2f)
            .SetEase(Ease.OutBounce);
        commonProperties[index].cashTxt.text = gameManager.uiManager.ScoreShow(upgradeOverlays.cashAmount);
        commonProperties[index].mainImgBg.sprite = upgradeOverlays.mainSprite;
        
        if (upgradeOverlays.currUpVal >= upgradeOverlays.maxUpVal)
        {
            commonProperties[index].cashBtn.gameObject.SetActive(false);
            commonProperties[index].rvBtn.gameObject.SetActive(false);
            commonProperties[index].maxBtn.gameObject.SetActive(true);
        }
        else
        {
            commonProperties[index].cashBtn.interactable = ecoManager.bCanWeSpendMoney(upgradeOverlays.cashAmount);
            commonProperties[index].cashBtn.gameObject.SetActive(true);
            commonProperties[index].rvBtn.gameObject.SetActive(true);
            commonProperties[index].maxBtn.gameObject.SetActive(false);
        }
    }

    void UpdateAllBts()
    {
        
        switch (currType)
        {
            case UpgradeType.Manager:

                for (int i = 0; i < managerOptions.Length; i++)
                {
                    var managerOption = managerOptions[i];
                    commonProperties[i].cashBtn.interactable = ecoManager.bCanWeSpendMoney(managerOption.cashAmount);
                    CommonLoad(managerOption, i);
                }

                break;

            case UpgradeType.Staff:

                //CommonLoad(staffOptions);
                for (int i = 0; i < staffOptions.Length; i++)
                {
                    var staffOption = staffOptions[i];
                    commonProperties[i].cashBtn.interactable = ecoManager.bCanWeSpendMoney(staffOption.cashAmount);
                    CommonLoad(staffOption, i);
                }

                break;
        }
    }

    void LoadButtons()
    {
        profileBtn.onClick.AddListener(OpenPanel);

        managerBtn.onClick.AddListener(() =>
        {
            currType = UpgradeType.Manager;
            selectionImgT.DOMove(managerBtn.transform.position, .2f);
            managerTxt.DOColor(Color.black, .2f);
            staffTxt.DOColor(Color.white, .2f);
            LoadWithUiEffect();
            LoadUiOptions();
        });
        staffBtn.onClick.AddListener(() =>
        {
            currType = UpgradeType.Staff;
            selectionImgT.DOMove(staffBtn.transform.position, .2f);
            managerTxt.DOColor(Color.white, .2f);
            staffTxt.DOColor(Color.black, .2f);
            LoadWithUiEffect();
            LoadUiOptions();
        });

        for (int i = 0; i < commonProperties.Length; i++)
        {
            int index = i; // capture the current index value
            commonProperties[index].cashBtn.onClick.AddListener(() => OnCashButtonClick(index));
            commonProperties[index].rvBtn.onClick.AddListener(() => OnRvButtonClick(index));
        }

        closeBtn.onClick.AddListener(OnCloseBtnClick);
    }

    void OnCashButtonClick(int index)
    {
        if (currType == UpgradeType.Manager)
        {
            ManagerCashClick(index);
        }
        else if (currType == UpgradeType.Staff)
        {
            StaffCashClick(index);
        }
    }

    void OnRvButtonClick(int index)
    {
        if (currType == UpgradeType.Manager)
        {
            OnManagerRvClick(index);
        }
        else if (currType == UpgradeType.Staff)
        {
            OnStaffRvClick(index);
        }
    }

    #region PLAYER UPGRADES

    void ManagerCashClick(int index)
    {
        switch (index)
        {
            case 0:
                OnPlayerSpeedUpgrade(managerOptions[index], index);
                break;

            case 1:
                OnPlayerCapacityUpgrade(managerOptions[index], index);
                break;

            case 2:
                OnPlayerProfitUpgrade(managerOptions[index], index);
                break;
        }
        SaveData();

    }

    void OnPlayerSpeedUpgrade(UpgradeOption upgradeOverlays, int index)
    {
        if (ecoManager.bCanWeSpendMoney(upgradeOverlays.cashAmount))
        {
            ecoManager.SpendMoney(upgradeOverlays.cashAmount);
            upgradeOverlays.currUpVal++;
            playerController.playerControllerData.maxSpeed += upgradeOverlays.upGradeMultiplier;
            upgradeOverlays.cashAmount =
                (int)(upgradeOverlays.cashAmount * upgradeOverlays.cashAmountUpGradeMultiplier);
            //CommonLoad(upgradeOverlays, index);
            LoadUiOptions();
            UpdateAllBts();
        }
    }

    void OnPlayerCapacityUpgrade(UpgradeOption upgradeOverlays, int index)
    {
        if (ecoManager.bCanWeSpendMoney(upgradeOverlays.cashAmount))
        {
            ecoManager.SpendMoney(upgradeOverlays.cashAmount);
            upgradeOverlays.currUpVal++;
            playerObjectStacking.maxCapacity += (int)upgradeOverlays.upGradeMultiplier;
            upgradeOverlays.cashAmount =
                (int)(upgradeOverlays.cashAmount * upgradeOverlays.cashAmountUpGradeMultiplier);
   
            LoadUiOptions();
            UpdateAllBts();

            //playerObjectStacking.LoadOffsets();
        }
    }

    void OnPlayerProfitUpgrade(UpgradeOption upgradeOverlays, int index)
    {
        if (ecoManager.bCanWeSpendMoney(upgradeOverlays.cashAmount))
        {
            ecoManager.SpendMoney(upgradeOverlays.cashAmount);
            upgradeOverlays.currUpVal++;
            //globalVars.profitMultiplier += upgradeOverlays.upGradeMultiplier;
            upgradeOverlays.cashAmount =
                (int)(upgradeOverlays.cashAmount * upgradeOverlays.cashAmountUpGradeMultiplier);
            //CommonLoad(upgradeOverlays, index);
            LoadUiOptions();
            UpdateAllBts();

        }
    }

    #endregion

    #region  StaffUPgrades

    

    
    void StaffCashClick(int index)
    {
       
        switch (index)
        {
            case 0:
                OnStaffHire(staffOptions[index], index);
                break;

            case 1:
                OnStaffSpeedUpgrade(staffOptions[index], index);
                break;

            case 2:
                OnStaffCapacityUpgrade(staffOptions[index], index);
                break;
        }
        SaveData();

    }
    
    void OnStaffHire(UpgradeOption upgradeOverlays, int index)
    {
        if (ecoManager.bCanWeSpendMoney(upgradeOverlays.cashAmount))
        {
            ecoManager.SpendMoney(upgradeOverlays.cashAmount);
            upgradeOverlays.currUpVal++;
            // GameObject helper = Instantiate(helperPrefab, gameManager.storeExitPoint.transform.position, Quaternion.identity);
            // upgradeOverlays.cashAmount =
            //     (int)(upgradeOverlays.cashAmount * upgradeOverlays.cashAmountUpGradeMultiplier);
            // helper.GetComponent<HelperBehaviour>().moveSpeed += staffOptions[1].value;
            // helper.GetComponent<HelperBehaviour>().capacity += staffOptions[2].value;
            
           // CommonLoad(upgradeOverlays, index);
            LoadUiOptions();
            UpdateAllBts();

        }
    }
    void OnStaffSpeedUpgrade(UpgradeOption upgradeOverlays, int index)
    {
        if (ecoManager.bCanWeSpendMoney(upgradeOverlays.cashAmount))
        {
            ecoManager.SpendMoney(upgradeOverlays.cashAmount);
            upgradeOverlays.currUpVal++;
            upgradeOverlays.value+= upgradeOverlays.upGradeMultiplier;
            // for (int i = 0; i < globalVars.gameManager.helpers.Count; i++)
            // {
            //     var helper = globalVars.gameManager.helpers[i];
            //     if (helper != null)
            //     {
            //         helper.moveSpeed += upgradeOverlays.upGradeMultiplier;
            //     }
            // }
            // upgradeOverlays.cashAmount =
               // (int)(upgradeOverlays.cashAmount * upgradeOverlays.cashAmountUpGradeMultiplier);
            //CommonLoad(upgradeOverlays, index);
            LoadUiOptions();
            UpdateAllBts();

        }
    }
    
    void OnStaffCapacityUpgrade(UpgradeOption upgradeOverlays, int index)
    {
        if (ecoManager.bCanWeSpendMoney(upgradeOverlays.cashAmount))
        {
            ecoManager.SpendMoney(upgradeOverlays.cashAmount);
            upgradeOverlays.currUpVal++;
            upgradeOverlays.value+= upgradeOverlays.upGradeMultiplier;

            // for (int i = 0; i < globalVars.gameManager.helpers.Count; i++)
            // {
            //     var helper = globalVars.gameManager.helpers[i];
            //     if (helper != null)
            //     {
            //         helper.capacity += upgradeOverlays.upGradeMultiplier;
            //     }
            // }
            upgradeOverlays.cashAmount =
                (int)(upgradeOverlays.cashAmount * upgradeOverlays.cashAmountUpGradeMultiplier);
            //CommonLoad(upgradeOverlays, index);
            LoadUiOptions();
            UpdateAllBts();

        }
    }

   
#endregion

    void OnManagerRvClick(int index)
    {
    }

    void OnStaffRvClick(int index)
    {
        
    }

    void OnCloseBtnClick()
    {
        playerController.playerControllerData.joystickPanel.gameObject.SetActive(true);
        BtnEffect(mainBgPanel.transform, .1f,
            () =>
            {
                mainBgPanel.transform.DOScale(Vector3.zero, .1f).OnComplete(() =>
                {
                    profilePanel.gameObject.SetActive(false);
                    UiManager.bIsUiOn = false;
                });
            });
    }

    void OpenPanel()
    {
        playerController.playerControllerData.joystickPanel.gameObject.SetActive(false);
        BtnEffect(profileBtn.transform, .2f, () =>
        {
            for (int i = 0; i < commonProperties.Length; i++)
            {
                commonProperties[i].mainT.localScale = Vector3.zero;
                commonProperties[i].cashBtn.transform.localScale = Vector3.zero;
                commonProperties[i].rvBtn.transform.localScale = Vector3.zero;
            }

            mainBgPanel.transform.localScale = Vector3.one;
            LoadUiOptions();
            profilePanel.gameObject.SetActive(true);
            UiManager.bIsUiOn = true;

            BtnEffect(mainBgPanel.transform, .05f, () => { LoadWithUiEffect(); });
        });
    }

    void BtnEffect(Transform btnT, float punchOffset, Action action)
    {
        btnT.DOPunchScale(Vector3.one * punchOffset, .3f, 1, .2f).SetEase(Ease.OutBounce, .1f).OnComplete(() =>
        {
            action.Invoke();
        });
    }

    void LoadWithUiEffect()
    {
        for (int i = 0; i < commonProperties.Length; i++)
        {
            Transform t = commonProperties[i].mainT;
            Transform cbT = commonProperties[i].cashBtn.transform;
            Transform rbT = commonProperties[i].rvBtn.transform;

            float delay = 0.05f * i;

            DOVirtual.DelayedCall(delay, () =>
            {
                t.DOScale(1f, 0.4f).OnComplete(() =>
                {
                    cbT.DOScale(Vector3.one, .5f).OnComplete(() => { rbT.DOScale(Vector3.one, .5f); });
                    t.DOPunchScale(Vector3.one * 0.1f, 0.2f, 2, .2f);
                });
            });
        }
    }

    void SaveData()
    {
        var upgrader = new UpgraderData();
        upgrader.managerOptions.Clear();
        upgrader.staffOptions.Clear();
        for (int i = 0; i < managerOptions.Length; i++)
        {
            var updata = managerOptions[i];
            var upgraderDataTemp = new UpgradeOption();
            if (updata != null)
            {
                upgraderDataTemp = updata;
                upgrader.managerOptions.Add(upgraderDataTemp);
            }
        }

        for (int i = 0; i < staffOptions.Length; i++)
        {
            var updata = staffOptions[i];
            var upgraderDataTemp = new UpgradeOption();
            if (updata != null)
            {
                upgraderDataTemp = updata;
                upgrader.staffOptions.Add(upgraderDataTemp);
            }
        }

        string Data = JsonUtility.ToJson(upgrader);
        PlayerPrefs.SetString(SaveKey, Data);
    }

    void AllPlayerDataLoadUpgrader()
    {
        //Player
        var upgraderDataTemp = managerOptions[0];
        playerController.playerControllerData.maxSpeed +=
            upgraderDataTemp.upGradeMultiplier * upgraderDataTemp.currUpVal;
        var upgraderDataTemp_1 = managerOptions[1];
        playerObjectStacking.maxCapacity +=
            (int)(upgraderDataTemp_1.upGradeMultiplier * upgraderDataTemp_1.currUpVal);
        var upgraderDataTemp_2 = managerOptions[2];
       // globalVars.profitMultiplier += upgraderDataTemp_2.upGradeMultiplier * upgraderDataTemp_2.currUpVal;
        
        //Staff
        var staffUpgraderDataTemp = staffOptions[0];
        //for (int i = 0; i < staffUpgraderDataTemp.currUpVal; i++)
        //{
        //    GameObject helper = Instantiate(helperPrefab, gameManager.storeExitPoint.transform.position, Quaternion.identity);
        //}
        StartCoroutine(SpawnHelper(staffUpgraderDataTemp.currUpVal));
        
    }

    IEnumerator SpawnHelper(int count)
    {
        var staffUpgraderDataTemp_1 = staffOptions[1];
        var staffUpgraderDataTemp_2 = staffOptions[2];
        for (int i = 0; i < count; i++)
        {
            yield return new WaitForSeconds(1.3f);
            //GameObject helper = Instantiate(gameManager.helperGO, gameManager.storeExitPoint.transform.position, Quaternion.identity);
           // HelperBehaviour helperS = helper.GetComponent<HelperBehaviour>();
            // helperS.moveSpeed += staffUpgraderDataTemp_1.upGradeMultiplier * staffUpgraderDataTemp_1.currUpVal; 
            // helperS.capacity += staffUpgraderDataTemp_2.upGradeMultiplier * staffUpgraderDataTemp_2.currUpVal;
        }
        
    }

    void LodaData()
    {
        var upgrader = new UpgraderData();

        if (PlayerPrefs.HasKey(SaveKey))
        {
            string Data = PlayerPrefs.GetString(SaveKey);
            upgrader = JsonUtility.FromJson<UpgraderData>(Data);
            for (int i = 0; i < managerOptions.Length; i++)
            {
                var upgraderDataTemp = upgrader.managerOptions[i];
                if (upgraderDataTemp != null)
                {
                    managerOptions[i].currUpVal = upgraderDataTemp.currUpVal;
                    managerOptions[i].upGradeMultiplier = upgraderDataTemp.upGradeMultiplier;
                    managerOptions[i].maxUpVal = upgraderDataTemp.maxUpVal;
                    managerOptions[i].cashAmount = upgraderDataTemp.cashAmount;
                    managerOptions[i].cashAmountUpGradeMultiplier = upgraderDataTemp.cashAmountUpGradeMultiplier;
                }
            }

            for (int i = 0; i < staffOptions.Length; i++)
            {
                var upgraderDataTemp = upgrader.staffOptions[i];
                if (upgraderDataTemp != null)
                {
                    staffOptions[i].currUpVal = upgraderDataTemp.currUpVal;
                    staffOptions[i].upGradeMultiplier = upgraderDataTemp.upGradeMultiplier;
                    staffOptions[i].maxUpVal = upgraderDataTemp.maxUpVal;
                    staffOptions[i].cashAmount = upgraderDataTemp.cashAmount;
                    staffOptions[i].cashAmountUpGradeMultiplier = upgraderDataTemp.cashAmountUpGradeMultiplier;
                }
            }
            
            AllPlayerDataLoadUpgrader();
        }
    }

    private void OnApplicationPause(bool pause)
    {
        if (pause)
        {
            SaveData();
        }
    }

    private void OnApplicationQuit()
    {
        SaveData();
    }
}