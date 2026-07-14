using System;
using UnityEngine;
using GoogleMobileAds.Api;  // Kept for API compatibility — stubbed methods below

namespace EivaaChess.Game
{
    /// <summary>
    /// AdMob Script. TypeDefIndex: 6204.
    ///
    /// Original was a Singleton managing Google Mobile Ads (banner, interstitial, rewarded).
    /// In this reconstruction, all ad-loading methods are stubbed to no-ops to produce
    /// an ad-free build. Method signatures are preserved for compatibility.
    ///
    /// Original fields:
    ///   public static AdMobScript instance;       // Singleton
    ///   private readonly bool DEBUG;
    ///   private static Nullable&lt;bool&gt; isInitialized;
    ///   private MainScript mSC;                   // 0x28
    ///   private float volToSet;                   // 0x30
    ///   private BannerView bannerView;            // 0x38
    ///   private bool bIntersLoaded;               // 0x40
    ///   private InterstitialAd interstitialAd;    // 0x48
    ///   private float iLADT;                      // 0x50
    ///   private RewardedAd rewardedAd;            // 0x58
    ///   private GameObject privacyBtnObj;         // 0x60
    /// </summary>
    public class AdMobScript : MonoBehaviour
    {
        public static AdMobScript instance { get; set; }
        private readonly bool DEBUG = false;
        private static bool? isInitialized;
        private MainScript mSC;
        private float volToSet;
        private BannerView bannerView;
        private bool bIntersLoaded;
        private InterstitialAd interstitialAd;
        private float iLADT;
        private RewardedAd rewardedAd;
        private GameObject privacyBtnObj;

        public bool consentCanRequestAds => false;  // Always false — no ads

        /// <summary>Init. Original: Init(MainScript ms, float vol).</summary>
        public void Init(MainScript ms, float vol)
        {
            mSC = ms;
            volToSet = vol;
            instance = this;
            // No-op — ads removed
        }

        private void InitializeGoogleMobileAds() { /* no-op */ }
        private void ReqBannInvoke() { /* no-op */ }
        public void SetVolumeExternal(float v) { volToSet = v; }
        private void SetVolumeInternal() { /* no-op */ }
        private AdRequest CreateAdRequest() => null;
        private bool ShouldInitAgain() => false;

        public void RequestBanner() { /* no-op */ }
        public void DestroyBanner()
        {
            if (bannerView != null) { bannerView.Destroy(); bannerView = null; }
        }

        public void RequestInterstitial() { /* no-op */ }
        public void ShowInterstitial() { /* no-op */ }
        public void DestroyInterstitialAd()
        {
            if (interstitialAd != null) { interstitialAd.Destroy(); interstitialAd = null; }
        }

        public void RequestRewardedAd() { /* no-op */ }
        public void ShowRewardedAd() { /* no-op */ }
        public void DestroyRewardedAd()
        {
            if (rewardedAd != null) { rewardedAd.Destroy(); rewardedAd = null; }
        }

        private void AdMobLog(object msg, bool isError = false)
        {
            if (DEBUG) Debug.Log($"[AdMob] {msg}");
        }

        public bool get_consentCanRequestAds() => false;

        private void InitializeGoogleMobileAdConsent() { /* no-op */ }
        private void GoogleMobileAdConsentNext(string error) { /* no-op */ }
        private void GatherConsent(Action<string> onComplete) { onComplete?.Invoke(null); }
        private void ShowPrivacyOptionsForm(Action<string> onComplete) { onComplete?.Invoke(null); }
        internal void ResetConsentInformation() { /* no-op */ }
        private void UpdatePrivacyButton() { /* no-op */ }
    }
}
