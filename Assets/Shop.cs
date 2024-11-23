using Bazaar.Data;
using Bazaar.Poolakey;
using Bazaar.Poolakey.Data;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Bazaar.Poolakey.Callbacks;

public class Shop : MonoBehaviour
{
    // Start is called before the first frame update
    private Payment payment;
    public Text test;
    Result<bool> result;
    int ToReward;
      void Start()
    {
        
        SecurityCheck securityCheck = SecurityCheck.Enable("MIHNMA0GCSqGSIb3DQEBAQUAA4G7ADCBtwKBrwDIyAFU05iAjt990oR5I41NXiCKVuUKGtLI3EQ0TFLJR8/jU3gme3kND7bJ6oea5f66jAtvOvHoRIqVT07/W5Vqa0ylPv9rw8gNk76x2ZR9wcRKPv949vw/pv7mouZ9vPcgygsO8Hx7V3a+07OSEHf+T/q4jrxoJt9mHdt/0el66AX2C/FBNNyGg04ozsV2YD4s5aUkLzVhcoEU2oOAmzw9IPrMaHQbPwLpMycFe2sCAwEAAQ==");
        PaymentConfiguration paymentConfiguration = new PaymentConfiguration(securityCheck);
        payment = new Payment(paymentConfiguration);
        _ = payment.Connect(OnPaymentConnect);
        
        //presult.data.

    }
    void OnPaymentConnect(Result<bool> result) {
        GetUserPurchases();
    }

    public void GetProductInfo(string productid)
    {
        _ = payment.GetSkuDetails(productid, SKUDetails.Type.all, OnReceiveSkuDetails);
    }
    void OnApplicationQuit()
    {
        payment.Disconnect();
    }
    Result<Bazaar.Poolakey.Data.PurchaseInfo> PurchaseResult;
    public void GetUserPurchases()
    {
        Debug.Log("userPurchases");
        _ = payment.GetPurchases(SKUDetails.Type.all, OnReceivePurchases);
    }

    string Product;
    public void Buy(string ProductId)
    {
        Debug.Log(ProductId);
        Product = ProductId;
        if (Debug.isDebugBuild)
        {
            if(ProductId== "Ad_free")
            {
                Product = ProductId;
            }
            else
            {
                Product = "Gem_test";
            }
            
        }
        _ = payment.Purchase(Product, SKUDetails.Type.all, OnPuschaseStart, OnPuschaseComplete, "PAYLOAD");
        
    }
    string AdToken;
    public void ConsumeAd()
    {
        Product = "";
        Advertisement.Instance.bannerAdSetup();
        PlayerPrefs.SetInt("hideAd", 0);
        Advertisement.Instance.hideAd = 0;
        _ = payment.Consume(AdToken, OnConsumeComlete);
    }


    private void OnPuschaseComplete(Result<PurchaseInfo> obj)
    {
        test.text = Product;
        if (Product== "Ad_free")
        {
            Advertisement.Instance.StopAd();
            AdToken = obj.data.purchaseToken;
        }
        else
        {
            _ = payment.Consume(obj.data.purchaseToken, OnConsumeComlete);
        }
        
    }

    private void OnConsumeComlete(Result<bool> obj)
    {
        purchaseFinish();
    }

    private void OnPuschaseStart(Result<PurchaseInfo> obj)
    {
       
    }

    void OnReceivePurchases(Result<List<PurchaseInfo>> result) {
        Debug.Log($"{result.message}, {result.stackTrace}");
        if (result.status == Status.Success)
        {
            foreach (var purchase in result.data)
            {
                
                if (purchase.productId == "Ad_free")
                {
                    AdToken = purchase.purchaseToken;
                    PlayerPrefs.SetInt("hideAd", 1);
                }
            }
        }

    }
    void OnReceiveSkuDetails(Result<List<SKUDetails>> result) {
        Debug.Log($"{result.message}, {result.stackTrace}");
        if (result.status == Status.Success)
        {
            foreach (var sku in result.data)
            {
                Debug.Log(sku.ToString());
            }
        }
    }
    void purchaseFinish()
    {
         if(Product== "Gem_test")
        {
            ToReward = 1000;
            CloseShop();
        }
        switch(Product)
        {
            case "Gem_pack_1000":
                ToReward = (1000);
                break;
            case "Gem_pack_3000":
                ToReward = (3000);
                break;
            case "Gem_pack_5000":
                ToReward = (5000);
                break;
            case "Gem_pack_11000":
                ToReward = (11000);
                break;
            case "Gem_pack_25000":
                ToReward = (25000);
                break;
            case "Gem_pack_50000":
                ToReward = (50000);
                break;
            case "Gem_pack_100000":
                ToReward = (100000);
                break;
            
        }
        
        
       
        CloseShop();
    }



    [SerializeField] GameObject ShopWindow;
    public void CloseShop()
    {
        ShopWindow.SetActive(false);
        if(!GameStateManager.instance.Home)
            {
                TouchManager.instance.ResumeTouch();
             
            }
        
        if (ToReward != 0)
        {
            
            PowerManager.instance.AddtoGems(ToReward);
            BlockListManager.instance.GemAnim(ToReward);
            GameBrain.Instance.CheckForPowerUpsInGameOver();    

            ToReward = 0;   
        }
        Product = "";

    }

    public void OpenShop()
    {
        ToReward = 0;
        ShopWindow.SetActive(true);
        ShopWindow.transform.parent.SetAsLastSibling();
        if (!GameStateManager.instance.Home)
        {
            TouchManager.instance.StopTouch();
        }
        
    }

    public static Shop instance { get; private set; }
    private void Awake()
    {
        if(instance != null && instance!=this)
        {
            Destroy(this);
        }
        else
        {
            instance = this;
        }
    }
    // Update is called once per frame

}
