using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AdiveryUnity;
using System;

public class Advertisement : MonoBehaviour
{
    [SerializeField]
    private float _timeBasedAdDurationMinute;
    [SerializeField]
    GameObject _advertisementPanel;
    
    public int TimeBasedGem, rewardAddGem;

    public static Advertisement Instance { get; private set; }
    private void Awake()
    {
        if(Instance!=null && Instance!=this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;    
        }
    }

    AdiveryListener listener;

    private void Start()
    {
        // StartCoroutine(timeBasedAdCounter());
        
        hideAd = PlayerPrefs.GetInt("hideAd");
        if (hideAd == 1) return;
        
        
        bannerAdSetup();
    }
    BannerAd bannerAd;
    public void bannerAdSetup()
    {   
            bannerAd = new BannerAd(bannerPlacement, BannerAd.TYPE_BANNER, BannerAd.POSITION_BOTTOM);
            bannerAd.OnAdLoaded += OnBannerAdLoaded;
            bannerAd.LoadAd();        
    }

    public void interstitialSetUp()
    {
        if (hideAd == 1) return;
      //  Debug.Log("inter 1");
        TouchManager.instance.StopTouch();
        ad = new Action(showIntersetial);
        StartCoroutine(LeaveAppForAction(ad));
       // Debug.Log("inter 2");
    }

    void showIntersetial()
    {
        if (Adivery.IsLoaded(InterstitialPlacement))
        {
            Adivery.Show(InterstitialPlacement);
        }
        else
        {
            Debug.Log("interNotLoaded");
        }
    }


    private void OnBannerAdLoaded(object sender, EventArgs e)
    {
        if(hideAd == 1) return;
        bannerAd.Show();
    }

    public string APPID, RewardPlacement,InterstitialPlacement;
    public void adiverySetUp()
    {
        Adivery.Configure(APPID);

        Adivery.PrepareRewardedAd(RewardPlacement);
        Adivery.PrepareInterstitialAd(InterstitialPlacement);

        listener = new AdiveryListener();

        listener.OnError += OnError;
        listener.OnRewardedAdLoaded += OnRewardedLoaded;
        listener.OnRewardedAdClosed += OnRewardedClosed;
        listener.OnRewardedAdShown += OnRewardedShown;
        listener.OnInterstitialAdShown += InterstitialAdShown;
        

        Adivery.AddListener(listener);
        
    }
    public void InterstitialAdShown(object caller,string message)
    {
        
        TouchManager.instance.ResumeTouch();
        PowerManager.instance.AddtoGems(TimeBasedGem);
        BlockListManager.instance.GemAnim(TimeBasedGem);
        GameBrain.Instance.CheckForPowerUpsInGameOver();
        /*StartCoroutine(timeBasedAdCounter());*/

    }
    public void OnError(object caller, AdiveryError error)
    {
        Debug.Log("placement: " + error.PlacementId + " error: " + error.Reason);
    }

    public void OnRewardedLoaded(object caller, string placementId)
    {
        // Rewarded ad loaded
        Debug.Log("Reward ad loaded");
    }
    bool ignore;
    public void OnRewardedClosed(object caller, AdiveryReward reward)
    {
        TouchManager.instance.ResumeTouch();
        if (reward.IsRewarded)
        {
            if (RewardGiven) return;
            PowerManager.instance.AddtoGems(rewardAddGem);
            BlockListManager.instance.GemAnim(rewardAddGem);
            GameBrain.Instance.CheckForPowerUpsInGameOver();
            RewardGiven = true;

        }

    }

    public void OnRewardedShown(object caller,string reward)
    {
       
            

    }



    bool startReward,RewardGiven;
    public string bannerPlacement;

    public void RewardAd()
    {
        TouchManager.instance.StopTouch();
        ad = new Action(showReward);
        //GameBrain.Instance.LeaveAppForAction(ad);
        StartCoroutine(LeaveAppForAction(ad));
    }

    public IEnumerator LeaveAppForAction(Action leavingaction)
    {

        //Debug.Log("leaving game to ad or payment");
        yield return null;
        while (SelectionUIManager.Instance.swaping)
        {
            yield return null;
        }
        leavingaction.Invoke();

    }


    void showReward()
    {
        RewardGiven = false;
        TouchManager.instance.StopTouch();

        if (Adivery.IsLoaded(RewardPlacement))
        {
            Adivery.Show(RewardPlacement);
        }
        else
        {
            TouchManager.instance.ResumeTouch();
        }

    }


    Action ad;



    public void TimeBasedAdd()
    {
        if (GameBrain.Instance.CheckIfPlayin())
        {
            TouchManager.instance.StopTouch();
            if(Adivery.IsLoaded(InterstitialPlacement))
            {
                Adivery.Show(InterstitialPlacement);    
            }
            
        }            
        
    }

    public int hideAd;
    public void StopAd()
    {
        PlayerPrefs.SetInt("hideAd", 1);
        hideAd = 1;
        bannerAd.Hide();
        //interstitial remains
    }

    
  
}
