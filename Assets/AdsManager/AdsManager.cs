using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdsManager : MonoBehaviour
{
    public static AdsManager instance;

    System.Action actionIS;
    System.Action actionRV;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
#if UNITY_ANDROID
        string appKey = "20d2711ed";
#elif UNITY_IPHONE
        string appKey = "";
#else
        string appKey = "unexpected_platform";
#endif

        Debug.Log("unity-script: IronSource.Agent.validateIntegration");
        IronSource.Agent.validateIntegration();

        Debug.Log("unity-script: unity version" + IronSource.unityVersion());

        // SDK init
        Debug.Log("unity-script: IronSource.Agent.init");
        IronSource.Agent.init(appKey);

        Invoke(nameof(ShowBannerAd), 10);
        Invoke(nameof(LoadInterstital), 10);
    }

    //[System.Obsolete]
    private void OnEnable()
    {
        //Add AdInfo Rewarded Video Events
        IronSourceRewardedVideoEvents.onAdOpenedEvent += ReardedVideoOnAdOpenedEvent;
        IronSourceRewardedVideoEvents.onAdClosedEvent += ReardedVideoOnAdClosedEvent;
        IronSourceRewardedVideoEvents.onAdAvailableEvent += ReardedVideoOnAdAvailable;
        IronSourceRewardedVideoEvents.onAdUnavailableEvent += ReardedVideoOnAdUnavailable;
        IronSourceRewardedVideoEvents.onAdShowFailedEvent += ReardedVideoOnAdShowFailedEvent;
        IronSourceRewardedVideoEvents.onAdRewardedEvent += ReardedVideoOnAdRewardedEvent;
        IronSourceRewardedVideoEvents.onAdClickedEvent += ReardedVideoOnAdClickedEvent;


        //Add AdInfo Interstitial Events
        IronSourceInterstitialEvents.onAdReadyEvent += InterstitialOnAdReadyEvent;
        IronSourceInterstitialEvents.onAdLoadFailedEvent += InterstitialOnAdLoadFailed;
        IronSourceInterstitialEvents.onAdOpenedEvent += InterstitialOnAdOpenedEvent;
        IronSourceInterstitialEvents.onAdClickedEvent += InterstitialOnAdClickedEvent;
        IronSourceInterstitialEvents.onAdShowSucceededEvent += InterstitialOnAdShowSucceededEvent;
        IronSourceInterstitialEvents.onAdShowFailedEvent += InterstitialOnAdShowFailedEvent;
        IronSourceInterstitialEvents.onAdClosedEvent += InterstitialOnAdClosedEvent;


        //Add AdInfo Banner Events
        IronSourceBannerEvents.onAdLoadedEvent += BannerOnAdLoadedEvent;
        IronSourceBannerEvents.onAdLoadFailedEvent += BannerOnAdLoadFailedEvent;
        IronSourceBannerEvents.onAdClickedEvent += BannerOnAdClickedEvent;
        IronSourceBannerEvents.onAdScreenPresentedEvent += BannerOnAdScreenPresentedEvent;
        IronSourceBannerEvents.onAdScreenDismissedEvent += BannerOnAdScreenDismissedEvent;
        IronSourceBannerEvents.onAdLeftApplicationEvent += BannerOnAdLeftApplicationEvent;
    }

    //[System.Obsolete]
    private void OnDisable()
    {
        //Add AdInfo Rewarded Video Events
        IronSourceRewardedVideoEvents.onAdOpenedEvent -= ReardedVideoOnAdOpenedEvent;
        IronSourceRewardedVideoEvents.onAdClosedEvent -= ReardedVideoOnAdClosedEvent;
        IronSourceRewardedVideoEvents.onAdAvailableEvent -= ReardedVideoOnAdAvailable;
        IronSourceRewardedVideoEvents.onAdUnavailableEvent -= ReardedVideoOnAdUnavailable;
        IronSourceRewardedVideoEvents.onAdShowFailedEvent -= ReardedVideoOnAdShowFailedEvent;
        IronSourceRewardedVideoEvents.onAdRewardedEvent -= ReardedVideoOnAdRewardedEvent;
        IronSourceRewardedVideoEvents.onAdClickedEvent -= ReardedVideoOnAdClickedEvent;


        //Add AdInfo Interstitial Events
        IronSourceInterstitialEvents.onAdReadyEvent -= InterstitialOnAdReadyEvent;
        IronSourceInterstitialEvents.onAdLoadFailedEvent -= InterstitialOnAdLoadFailed;
        IronSourceInterstitialEvents.onAdOpenedEvent -= InterstitialOnAdOpenedEvent;
        IronSourceInterstitialEvents.onAdClickedEvent -= InterstitialOnAdClickedEvent;
        IronSourceInterstitialEvents.onAdShowSucceededEvent -= InterstitialOnAdShowSucceededEvent;
        IronSourceInterstitialEvents.onAdShowFailedEvent -= InterstitialOnAdShowFailedEvent;
        IronSourceInterstitialEvents.onAdClosedEvent -= InterstitialOnAdClosedEvent;


        //Add AdInfo Banner Events
        IronSourceBannerEvents.onAdLoadedEvent -= BannerOnAdLoadedEvent;
        IronSourceBannerEvents.onAdLoadFailedEvent -= BannerOnAdLoadFailedEvent;
        IronSourceBannerEvents.onAdClickedEvent -= BannerOnAdClickedEvent;
        IronSourceBannerEvents.onAdScreenPresentedEvent -= BannerOnAdScreenPresentedEvent;
        IronSourceBannerEvents.onAdScreenDismissedEvent -= BannerOnAdScreenDismissedEvent;
        IronSourceBannerEvents.onAdLeftApplicationEvent -= BannerOnAdLeftApplicationEvent;
    }


    // Update is called once per frame
    void Update()
    {

    }

    //public void ShowRV()
    //{
    //    ShowRewardAd(null);
    //}

    //public void ShowIS()
    //{
    //    ShowInterstitialAd(null);
    //}

    public void ShowRewardAd(Action action)
    {
        if (IronSource.Agent.isRewardedVideoAvailable())
        {
            IronSource.Agent.showRewardedVideo();
            actionRV = action;
        }
        else
        {
            Debug.Log("unity-script: IronSource.Agent.isRewardedVideoAvailable - False");
        }
    }

    public void ShowInterstitialAd(int levelNumber, Action action)
    {
        Debug.LogError("IS Ad is diplayed in Level : " + levelNumber);

        if (levelNumber <= 1)
        {
            Debug.LogError("IS Ad is not diplayed");
            return;
        }

        if (IronSource.Agent.isRewardedVideoAvailable())
        {
            IronSource.Agent.showInterstitial();
            Debug.LogError("IS Ad is diplayed in Level : " + levelNumber);
            actionIS = action;
        }
        else
        {
            Debug.Log("unity-script: IronSource.Agent.isRewardedVideoAvailable - False");
        }
    }

    public void ShowBannerAd()
    {
        IronSource.Agent.loadBanner(IronSourceBannerSize.BANNER, IronSourceBannerPosition.BOTTOM);
    }
    void LoadInterstital()
    {
        IronSource.Agent.loadInterstitial();
    }

    #region RV Events
    void ReardedVideoOnAdOpenedEvent(IronSourceAdInfo adInfo)
    {
        Debug.Log("unity-script: I got ReardedVideoOnAdOpenedEvent With AdInfo " + adInfo.ToString());
    }
    void ReardedVideoOnAdClosedEvent(IronSourceAdInfo adInfo)
    {
        Debug.Log("unity-script: I got ReardedVideoOnAdClosedEvent With AdInfo " + adInfo.ToString());
        actionRV.Invoke();
        RV_WATCHED();
    }
    void ReardedVideoOnAdAvailable(IronSourceAdInfo adInfo)
    {
        Debug.Log("unity-script: I got ReardedVideoOnAdAvailable With AdInfo " + adInfo.ToString());
    }
    void ReardedVideoOnAdUnavailable()
    {
        Debug.Log("unity-script: I got ReardedVideoOnAdUnavailable");
    }
    void ReardedVideoOnAdShowFailedEvent(IronSourceError ironSourceError, IronSourceAdInfo adInfo)
    {
        Debug.Log("unity-script: I got RewardedVideoAdOpenedEvent With Error" + ironSourceError.ToString() + "And AdInfo " + adInfo.ToString());
    }
    void ReardedVideoOnAdRewardedEvent(IronSourcePlacement ironSourcePlacement, IronSourceAdInfo adInfo)
    {
        Debug.Log("unity-script: I got ReardedVideoOnAdRewardedEvent With Placement" + ironSourcePlacement.ToString() + "And AdInfo " + adInfo.ToString());
    }
    void ReardedVideoOnAdClickedEvent(IronSourcePlacement ironSourcePlacement, IronSourceAdInfo adInfo)
    {
        Debug.Log("unity-script: I got ReardedVideoOnAdClickedEvent With Placement" + ironSourcePlacement.ToString() + "And AdInfo " + adInfo.ToString());
    }
    #endregion

    #region IS Events
    void InterstitialOnAdReadyEvent(IronSourceAdInfo adInfo)
    {
        Debug.Log("unity-script: I got InterstitialOnAdReadyEvent With AdInfo " + adInfo.ToString());
    }

    void InterstitialOnAdLoadFailed(IronSourceError ironSourceError)
    {
        Debug.Log("unity-script: I got InterstitialOnAdLoadFailed With Error " + ironSourceError.ToString());
        IronSource.Agent.loadInterstitial();
    }

    void InterstitialOnAdOpenedEvent(IronSourceAdInfo adInfo)
    {
        Debug.Log("unity-script: I got InterstitialOnAdOpenedEvent With AdInfo " + adInfo.ToString());
    }

    void InterstitialOnAdClickedEvent(IronSourceAdInfo adInfo)
    {
        Debug.Log("unity-script: I got InterstitialOnAdClickedEvent With AdInfo " + adInfo.ToString());
    }

    void InterstitialOnAdShowSucceededEvent(IronSourceAdInfo adInfo)
    {
        Debug.Log("unity-script: I got InterstitialOnAdShowSucceededEvent With AdInfo " + adInfo.ToString());
    }

    void InterstitialOnAdShowFailedEvent(IronSourceError ironSourceError, IronSourceAdInfo adInfo)
    {
        Debug.Log("unity-script: I got InterstitialOnAdShowFailedEvent With Error " + ironSourceError.ToString() + " And AdInfo " + adInfo.ToString());
        IronSource.Agent.loadInterstitial();
    }

    void InterstitialOnAdClosedEvent(IronSourceAdInfo adInfo)
    {
        Debug.Log("unity-script: I got InterstitialOnAdClosedEvent With AdInfo " + adInfo.ToString());
        actionIS.Invoke();
        IS_WATCHED();
        IronSource.Agent.loadInterstitial();

    }
    #endregion

    #region Banner Events
    void BannerOnAdLoadedEvent(IronSourceAdInfo adInfo)
    {
        Debug.Log("unity-script: I got BannerOnAdLoadedEvent With AdInfo " + adInfo.ToString());
        IronSource.Agent.displayBanner();
    }

    void BannerOnAdLoadFailedEvent(IronSourceError ironSourceError)
    {
        Debug.Log("unity-script: I got BannerOnAdLoadFailedEvent With Error " + ironSourceError.ToString());
    }

    void BannerOnAdClickedEvent(IronSourceAdInfo adInfo)
    {
        Debug.Log("unity-script: I got BannerOnAdClickedEvent With AdInfo " + adInfo.ToString());
    }

    void BannerOnAdScreenPresentedEvent(IronSourceAdInfo adInfo)
    {
        Debug.Log("unity-script: I got BannerOnAdScreenPresentedEvent With AdInfo " + adInfo.ToString());
    }

    void BannerOnAdScreenDismissedEvent(IronSourceAdInfo adInfo)
    {
        Debug.Log("unity-script: I got BannerOnAdScreenDismissedEvent With AdInfo " + adInfo.ToString());
    }

    void BannerOnAdLeftApplicationEvent(IronSourceAdInfo adInfo)
    {
        Debug.Log("unity-script: I got BannerOnAdLeftApplicationEvent With AdInfo " + adInfo.ToString());
    }

    #endregion

    //#if GA_EVENTS
    void RV_WATCHED()
    {
        LogEvent("rv_watched");
    }

    public void RV_EVENT(string _rvName)
    {
        if (_rvName != null)
            LogEvent("rv_" + _rvName);
    }

    void IS_WATCHED()
    {
        LogEvent("is_watched");
    }

    void LogEvent(string eventName)
    {
        Debug.LogError("EventName: " + eventName);
        GameAnalyticsSDK.GameAnalytics.NewDesignEvent(eventName);
    }

    void LogEvent(string eventName, string eventParameter1, string eventValue1)
    {
        Dictionary<string, object> keyValuePairs = new Dictionary<string, object>();
        keyValuePairs.Add(eventParameter1, eventValue1);

        GameAnalyticsSDK.GameAnalytics.NewDesignEvent(eventName, keyValuePairs);
    }

    void LogEvent(string eventName, string eventParameter1, string eventValue1, string eventParameter2, string eventValue2)
    {
        Dictionary<string, object> keyValuePairs = new Dictionary<string, object>();
        keyValuePairs.Add(eventParameter1, eventValue1);
        keyValuePairs.Add(eventParameter2, eventValue2);

        GameAnalyticsSDK.GameAnalytics.NewDesignEvent(eventName, keyValuePairs);
    }
    //#endif
}
