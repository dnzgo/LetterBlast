using UnityEngine;
using GoogleMobileAds.Api;
using System;

public class AdManager : MonoBehaviour
{
    public static AdManager Instance;

    private BannerView bannerView;
    private InterstitialAd interstitialAd;
    private RewardedAd rewardedAd;

    // can be override from inspector
#if UNITY_ANDROID
    [Header("Android Test IDs")]
    [SerializeField] private string bannerId       = "ca-app-pub-3940256099942544/6300978111";
    [SerializeField] private string interstitialId = "ca-app-pub-3940256099942544/1033173712";
    [SerializeField] private string rewardedId     = "ca-app-pub-3940256099942544/5224354917";
#elif UNITY_IOS
    [Header("iOS Test IDs")]
    [SerializeField] private string bannerId       = "ca-app-pub-3940256099942544/2934735716";
    [SerializeField] private string interstitialId = "ca-app-pub-3940256099942544/4411468910";
    [SerializeField] private string rewardedId     = "ca-app-pub-3940256099942544/1712485313";
#else
    [Header("Editor IDs (won't load real ads)")]
    [SerializeField] private string bannerId       = "test-banner";
    [SerializeField] private string interstitialId = "test-interstitial";
    [SerializeField] private string rewardedId     = "test-rewarded";
#endif

    private void Awake()
    {
        if (Instance == null) { Instance = this; DontDestroyOnLoad(gameObject); }
        else { Destroy(gameObject); return; }

        // (optional)make sure events come from main thread
        MobileAds.RaiseAdEventsOnUnityMainThread = true;

        MobileAds.Initialize(initStatus => {
            // Ads will be ready when app opened
            LoadInterstitial();
            LoadRewarded();
        });
    }

    #region Banner
    public void ShowBannerTop()
    {
        ShowBanner(AdPosition.Top);
    }

    public void ShowBannerBottom()
    {
        ShowBanner(AdPosition.Bottom);
    }

    private void ShowBanner(AdPosition pos)
    {
        DestroyBanner();

        bannerView = new BannerView(bannerId, AdSize.Banner, pos);
        var request = new AdRequest();
        bannerView.LoadAd(request);
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
        InterstitialAd.Load(interstitialId, new AdRequest(), (InterstitialAd ad, LoadAdError error) =>
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

    public bool CanShowInterstitial()
    {
        return interstitialAd != null && interstitialAd.CanShowAd();
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
        RewardedAd.Load(rewardedId, new AdRequest(), (RewardedAd ad, LoadAdError error) =>
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

    public bool CanShowRewarded()
    {
        return rewardedAd != null && rewardedAd.CanShowAd();
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
