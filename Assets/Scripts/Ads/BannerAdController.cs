using UnityEngine;
#if GOOGLE_MOBILE_ADS
using GoogleMobileAds.Api;
#endif

namespace MonoMerge.Ads
{
    /// <summary>
    /// GDD 3, Banner placement: "Ekranin en altinda, oyun alanini kapatmayacak sekilde
    /// surekli acik kalir." Loaded once at startup and left on screen — never hidden or
    /// toggled during normal play. See AdsManager's class doc for the SDK setup steps that
    /// activate the code inside the #if blocks below.
    /// </summary>
    public class BannerAdController : MonoBehaviour
    {
#if GOOGLE_MOBILE_ADS
        private BannerView bannerView;
#endif

        private void Start()
        {
            LoadBanner();
        }

        private void LoadBanner()
        {
#if GOOGLE_MOBILE_ADS
            bannerView = new BannerView(AdUnitIds.Banner, AdSize.Banner, AdPosition.Bottom);
            bannerView.LoadAd(new AdRequest());
#endif
        }

        private void OnDestroy()
        {
#if GOOGLE_MOBILE_ADS
            bannerView?.Destroy();
#endif
        }
    }
}
