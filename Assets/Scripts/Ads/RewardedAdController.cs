using System;
using System.Collections.Generic;
using UnityEngine;
using MonoMerge.Core;
using MonoMerge.Grid;
using MonoMerge.Tiles;
using MonoMerge.UI;
using MonoMerge.Undo;
#if GOOGLE_MOBILE_ADS
using GoogleMobileAds.Api;
#endif

namespace MonoMerge.Ads
{
    /// <summary>
    /// GDD 3, Rewarded placement (highest eCPM, "en degerli birim") — two distinct rewards:
    ///   1. "Reklam Izle ve Izgaradaki 3 Tasi Sil" — offered from GameOverPanel; clears 3
    ///      random tiles and resumes the run instead of ending it.
    ///   2. "Hatali hamleyi Geri Al (Undo)" — restores the last Undo/UndoManager snapshot;
    ///      call RequestUndo() from an in-game Undo button.
    /// Wired here rather than inside GameOverPanel/UndoManager themselves, so those classes
    /// stay free of any AdMob dependency (see their class docs).
    /// </summary>
    public class RewardedAdController : MonoBehaviour
    {
        [SerializeField] private GameOverPanel gameOverPanel;
        [SerializeField] private int tilesToClearOnContinue = 3;

#if GOOGLE_MOBILE_ADS
        private RewardedAd rewardedAd;
#endif

        private void Start()
        {
            LoadAd();

            if (gameOverPanel != null)
            {
                gameOverPanel.OnWatchAdToContinueRequested += HandleWatchAdToContinue;
            }
        }

        private void OnDestroy()
        {
            if (gameOverPanel != null)
            {
                gameOverPanel.OnWatchAdToContinueRequested -= HandleWatchAdToContinue;
            }
#if GOOGLE_MOBILE_ADS
            rewardedAd?.Destroy();
#endif
        }

        /// <summary>Hook this up to an in-game "Undo" button while GameState.Playing.</summary>
        public void RequestUndo()
        {
            if (UndoManager.Instance == null || !UndoManager.Instance.HasSnapshot) return;
            ShowAd(() => UndoManager.Instance.RestoreSnapshot(GridManager.Instance));
        }

        private void HandleWatchAdToContinue()
        {
            ShowAd(ClearTilesAndResume);
        }

        private void ClearTilesAndResume()
        {
            ClearRandomTiles(tilesToClearOnContinue);
            GameManager.Instance?.SetState(GameState.Playing);
        }

        /// <summary>GDD only specifies "3 tasi sil", not which three, so a random subset is
        /// the least-biased choice — Fisher-Yates shuffle over every occupied cell.</summary>
        private void ClearRandomTiles(int count)
        {
            GridManager grid = GridManager.Instance;
            var occupied = new List<GridCoordinate>();

            for (int x = 0; x < grid.Width; x++)
            {
                for (int y = 0; y < grid.Height; y++)
                {
                    var coord = new GridCoordinate(x, y);
                    if (!grid.IsCellEmpty(coord)) occupied.Add(coord);
                }
            }

            for (int i = occupied.Count - 1; i > 0; i--)
            {
                int j = UnityEngine.Random.Range(0, i + 1);
                (occupied[i], occupied[j]) = (occupied[j], occupied[i]);
            }

            int cleared = 0;
            foreach (var coord in occupied)
            {
                if (cleared >= count) break;

                GridCell cell = grid.GetCell(coord);
                Tile tile = cell.OccupyingTile;
                grid.RemoveTile(coord);
                if (tile != null) Destroy(tile.gameObject);
                cleared++;
            }
        }

        private void LoadAd()
        {
#if GOOGLE_MOBILE_ADS
            RewardedAd.Load(AdUnitIds.Rewarded, new AdRequest(), (ad, error) =>
            {
                if (error != null || ad == null) return;
                rewardedAd = ad;
            });
#endif
        }

        private void ShowAd(Action onRewarded)
        {
#if GOOGLE_MOBILE_ADS
            if (rewardedAd != null && rewardedAd.CanShowAd())
            {
                rewardedAd.Show(_ =>
                {
                    onRewarded?.Invoke();
                    LoadAd(); // preload the next one
                });
            }
#else
            Debug.LogWarning("MonoMerge.Ads: rewarded ad unavailable (SDK not installed) — reward not granted.");
#endif
        }
    }
}
