namespace MonoMerge.Ads
{
    /// <summary>
    /// Central home for every AdMob ad unit ID used by the game. Currently populated with
    /// Google's OFFICIAL public test IDs (safe to ship in dev builds — they never earn real
    /// revenue and never risk a policy strike). Replace every constant here with your own
    /// IDs from the AdMob console before the Week 4 "V1.0 Magaza Gonderimi" release.
    /// Source: https://developers.google.com/admob/unity/test-ads
    /// </summary>
    public static class AdUnitIds
    {
#if UNITY_ANDROID
        public const string Banner = "ca-app-pub-3940256099942544/6300978111";
        public const string Interstitial = "ca-app-pub-3940256099942544/1033173712";
        public const string Rewarded = "ca-app-pub-3940256099942544/5224354917";
#elif UNITY_IOS
        public const string Banner = "ca-app-pub-3940256099942544/2934735716";
        public const string Interstitial = "ca-app-pub-3940256099942544/4411468910";
        public const string Rewarded = "ca-app-pub-3940256099942544/1712485313";
#else
        public const string Banner = "unused";
        public const string Interstitial = "unused";
        public const string Rewarded = "unused";
#endif
    }
}
