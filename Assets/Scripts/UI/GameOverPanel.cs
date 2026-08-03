using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using MonoMerge.Core;
using MonoMerge.Score;

namespace MonoMerge.UI
{
    /// <summary>
    /// GDD 1 &amp; 3: shown when GameManager reaches GameState.GameOver. Displays the run's
    /// final score plus the persisted high score (Core.SaveManager), and exposes two actions:
    /// restart, and "watch ad to continue" (GDD 3, Rewarded placement #1: "Reklam Izle ve
    /// Izgaradaki 3 Tasi Sil"). The continue button only raises an event here — Week 4's
    /// Ads/RewardedAdController is what actually plays the ad and clears tiles, so this panel
    /// stays free of any AdMob dependency.
    /// </summary>
    public class GameOverPanel : MonoBehaviour
    {
        [SerializeField] private GameObject root;
        [SerializeField] private TextMeshProUGUI finalScoreLabel;
        [SerializeField] private TextMeshProUGUI highScoreLabel;
        [SerializeField] private Button restartButton;
        [SerializeField] private Button watchAdToContinueButton;

        /// <summary>Raised when the player taps "watch ad to continue". Unwired until Week 4's
        /// RewardedAdController subscribes — the button is a harmless no-op until then.</summary>
        public event Action OnWatchAdToContinueRequested;

        private void Awake()
        {
            if (root != null) root.SetActive(false);
            if (restartButton != null) restartButton.onClick.AddListener(HandleRestartClicked);
            if (watchAdToContinueButton != null) watchAdToContinueButton.onClick.AddListener(HandleWatchAdClicked);
        }

        private void Start()
        {
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
        }

        private void HandleStateChanged(GameState state)
        {
            bool isGameOver = state == GameState.GameOver;
            if (root != null) root.SetActive(isGameOver);
            if (isGameOver) RefreshLabels();
        }

        private void RefreshLabels()
        {
            int finalScore = ScoreManager.Instance != null ? ScoreManager.Instance.CurrentScore : 0;
            int highScore = SaveManager.Instance != null ? SaveManager.Instance.HighScore : 0;

            if (finalScoreLabel != null) finalScoreLabel.text = finalScore.ToString("N0");
            if (highScoreLabel != null) highScoreLabel.text = highScore.ToString("N0");
        }

        private void HandleRestartClicked()
        {
            GameManager.Instance?.StartGame();
        }

        private void HandleWatchAdClicked()
        {
            OnWatchAdToContinueRequested?.Invoke();
        }
    }
}
