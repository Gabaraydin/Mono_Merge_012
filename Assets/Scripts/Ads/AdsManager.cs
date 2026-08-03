using UnityEngine;
#if GOOGLE_MOBILE_ADS
using GoogleMobileAds.Api;
#endif

namespace MonoMerge.Ads
{
    /// <summary>
    /// GDD 3: "Para Kazanma (Monetization) Stratejisi ... Google AdMob kullanilacaktir."
    ///
    /// IMPORTANT — every script in Assets/Scripts/Ads/ is wrapped in
    /// <c>#if GOOGLE_MOBILE_ADS</c> so the project keeps compiling before the Google Mobile
    /// Ads Unity SDK is installed (a missing native SDK would otherwise break compilation for
    /// the ENTIRE project, including Weeks 1-3, which have nothing to do with ads). To turn
    /// on real ads:
    ///   1. Install the plugin (Google's official distribution, not the Package Manager
    ///      registry): https://github.com/googleads/googleads-mobile-unity/releases — import
    ///      the .unitypackage, which also brings in the External Dependency Manager (EDM4U).
    ///   2. Run Assets &gt; External Dependency Manager &gt; Android/iOS Resolver once, so the
    ///      native Google Mobile Ads libraries get pulled in for each platform.
    ///   3. Add <c>GOOGLE_MOBILE_ADS</c> to Edit &gt; Project Settings &gt; Player &gt; Other
    ///      Settings &gt; Scripting Define Symbols, for both Android and iOS tabs.
    ///   4. Set your real AdMob App ID via Assets &gt; Google Mobile Ads &gt; Settings, and
    ///      replace every ID in Ads/AdUnitIds.cs with your own from the AdMob console.
    /// Until all four steps are done, every class in this folder simply logs and no-ops.
    /// </summary>
    public class AdsManager : MonoBehaviour
    {
        public static AdsManager Instance { get; private set; }

        public bool IsInitialized { get; private set; }

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        private void OnDestroy()
        {
            if (Instance == this) Instance = null;
        }

        private void Start()
        {
            Initialize();
        }

        private void Initialize()
        {
#if GOOGLE_MOBILE_ADS
            MobileAds.Initialize(_ => { IsInitialized = true; });
#else
            Debug.LogWarning("MonoMerge.Ads: Google Mobile Ads SDK not installed — ads are " +
                              "disabled. See AdsManager's class doc for the 4 setup steps.");
#endif
        }
    }
}
