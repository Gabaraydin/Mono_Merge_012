namespace MonoMerge.Ads
{
    /// <summary>
    /// Central home for every AdMob ad unit ID used by the game. Android now uses the
    /// project's real AdMob account (App ID ca-app-pub-3337422341749058~7427319010,
    /// set via Assets > Google Mobile Ads > Settings). iOS still uses Google's OFFICIAL
    /// public test IDs — safe to ship in dev builds, never earn real revenue — until an
    /// iOS app entry is created in the same AdMob account.
    /// Source: https://developers.google.com/admob/unity/test-ads
    /// </summary>
    public static class AdUnitIds
    {
#if UNITY_ANDROID
        public const string Banner = "ca-app-pub-3337422341749058/8668816717";
        public const string Interstitial = "ca-app-pub-3337422341749058/3739355449";
        public const string Rewarded = "ca-app-pub-3337422341749058/3963284942";
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
