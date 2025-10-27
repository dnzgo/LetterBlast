using UnityEngine;
using GoogleMobileAds;
using GoogleMobileAds.Api;
using System;

public class AdManager : MonoBehaviour
{
    public static AdManager Instance;

    private BannerView bannerView;
    private InterstitialAd interstitialAd;
    private RewardedAd rewardedAd;

    private string bannerID       = "ca-app-pub-3940256099942544/2435281174";
    private string interstitialID = "ca-app-pub-3940256099942544/4411468910";
    private string rewardedID     = "ca-app-pub-3940256099942544/1712485313";


    private void Awake()
    {
        if (Instance == null) { Instance = this; DontDestroyOnLoad(gameObject); }
        else { Destroy(gameObject); return; }

        // (optional)make sure events come from main thread
        MobileAds.RaiseAdEventsOnUnityMainThread = true;

        // RequestConfiguration
        RequestConfiguration requestConfiguration = new RequestConfiguration
        {
            TagForChildDirectedTreatment = TagForChildDirectedTreatment.True,
            MaxAdContentRating = MaxAdContentRating.G // optional
        };

        MobileAds.SetRequestConfiguration(requestConfiguration);

        // Initialize Google Mobile Ads SDK.
        MobileAds.Initialize((InitializationStatus initStatus) =>
        {
            // This callback is called once the MobileAds SDK is initialized.
            LoadInterstitial();
            LoadRewarded();
        });
    }

    #region Banner

    public void ShowAdaptiveBanner()
    {
        if (bannerView != null)
        {
            bannerView.Destroy();
        }
        // Get the device safe width in density-independent pixels.
        int deviceWidth = MobileAds.Utils.GetDeviceSafeWidth();

        // Define the anchored adaptive ad size.
        AdSize adaptiveSize =
            AdSize.GetCurrentOrientationAnchoredAdaptiveBannerAdSizeWithWidth(deviceWidth);

        // Create an anchored adaptive banner view.
        bannerView = new BannerView(bannerID, adaptiveSize, AdPosition.Bottom);

        // Send a request to load an ad into the banner view.
        bannerView.LoadAd(new AdRequest());
    }

    public void HideBanner()
    {
        DestroyBanner();
    }

    private void DestroyBanner()
    {
        if (bannerView != null)
        {
            bannerView.Destroy();
            bannerView = null;
        }
    }
    #endregion

    #region Interstitial
    public void LoadInterstitial()
    {
        // Static Load API
        InterstitialAd.Load(interstitialID, new AdRequest(), (InterstitialAd ad, LoadAdError error) =>
        {
            if (error != null)
            {
                Debug.LogWarning("Interstitial load error: " + error);
                interstitialAd = null;
                return;
            }

            interstitialAd = ad;

            // Fullscreen event’leri
            interstitialAd.OnAdFullScreenContentClosed += () =>
            {
                interstitialAd = null;
                LoadInterstitial(); // reload after closed
            };

            interstitialAd.OnAdFullScreenContentFailed += (AdError err) =>
            {
                Debug.LogWarning("Interstitial failed to show: " + err);
                interstitialAd = null;
                LoadInterstitial();
            };
        });
    }

    public void ShowInterstitial()
    {
        if (interstitialAd != null && interstitialAd.CanShowAd())
        {
            interstitialAd.Show();
        }
        else
        {
            // try to reload if its not ready
            LoadInterstitial();
            Debug.Log("Interstitial not ready yet.");
        }
    }
    #endregion

    #region Rewarded
    public void LoadRewarded()
    {

        // Create our request used to load the ad.
        var adRequest = new AdRequest();

        RewardedAd.Load(rewardedID, adRequest, (RewardedAd ad, LoadAdError error) =>
        {
            if (error != null)
            {
                Debug.LogWarning("Rewarded load error: " + error);
                rewardedAd = null;
                return;
            }

            rewardedAd = ad;

            rewardedAd.OnAdFullScreenContentClosed += () =>
            {
                rewardedAd = null;
                LoadRewarded(); // reload after closed
            };

            rewardedAd.OnAdFullScreenContentFailed += (AdError err) =>
            {
                Debug.LogWarning("Rewarded failed to show: " + err);
                rewardedAd = null;
                LoadRewarded();
            };
        });
    }

    /// <summary>
    /// Sow rewarded; when the user receives the reward, the onReward callback is triggered.
    /// </summary>
    public void ShowRewarded(Action onReward)
    {
        if (rewardedAd != null && rewardedAd.CanShowAd())
        {
            // New API: rewards come from callback in Show
            rewardedAd.Show((Reward reward) =>
            {
                Debug.Log($"Reward earned: {reward.Amount} {reward.Type}");
                onReward?.Invoke();
            });
        }
        else
        {
            LoadRewarded();
            Debug.Log("Rewarded not ready yet.");
        }
    }
    #endregion

    private void OnDestroy()
    {
        DestroyBanner();
        interstitialAd?.Destroy();
        rewardedAd?.Destroy();
    }
}
