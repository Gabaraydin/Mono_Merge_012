using UnityEngine;
using MonoMerge.Core;
#if GOOGLE_MOBILE_ADS
using GoogleMobileAds.Api;
#endif

namespace MonoMerge.Ads
{
    /// <summary>
    /// GDD 3, Interstitial placement: "Her 'Game Over' ekranindan sonra veya oyuncu ana
    /// menuye donerken gosterilir. Oyunun akisini bolmemek icin oyun esnasinda ASLA
    /// gosterilmemelidir." Listens to GameManager.OnStateChanged and only ever shows on the
    /// GameOver/MainMenu transitions — it has no code path that can show one while Playing.
    /// </summary>
    public class InterstitialAdController : MonoBehaviour
    {
#if GOOGLE_MOBILE_ADS
        private InterstitialAd interstitialAd;
#endif

        private void Start()
        {
            LoadAd();

            if (GameManager.Instance != null)
            {
                GameManager.Instance.OnStateChanged += HandleStateChanged;
            }
        }

        private void OnDestroy()
        {
            if (GameManager.Instance != null)
            {
                GameManager.Instance.OnStateChanged -= HandleStateChanged;
            }
#if GOOGLE_MOBILE_ADS
            interstitialAd?.Destroy();
#endif
        }

        private void HandleStateChanged(GameState state)
        {
            // GDD's explicit rule lives here as code, not just as a comment: the only two
            // states that can ever trigger Show() are GameOver and MainMenu.
            if (state == GameState.GameOver || state == GameState.MainMenu)
            {
                ShowAd();
            }
        }

        private void LoadAd()
        {
#if GOOGLE_MOBILE_ADS
            InterstitialAd.Load(AdUnitIds.Interstitial, new AdRequest(), (ad, error) =>
            {
                if (error != null || ad == null) return;
                interstitialAd = ad;
                interstitialAd.OnAdFullScreenContentClosed += LoadAd; // preload the next one
            });
#endif
        }

        private void ShowAd()
        {
#if GOOGLE_MOBILE_ADS
            if (interstitialAd != null && interstitialAd.CanShowAd())
            {
                interstitialAd.Show();
            }
#endif
        }
    }
}
