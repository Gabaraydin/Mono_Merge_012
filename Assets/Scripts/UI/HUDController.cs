using TMPro;
using UnityEngine;
using MonoMerge.Score;

namespace MonoMerge.UI
{
    /// <summary>
    /// GDD 2 (Ornek Arayuz Taslagi): title + a single live score number above the grid,
    /// nothing else — "dikkati sadece oyun izgarasina ceken temiz tasarim dili." No buttons,
    /// bars, or icons here by design; settings/mute live in their own menu (SettingsPanel).
    ///
    /// Subscribes in Start (not OnEnable) so ScoreManager.Instance is guaranteed to already
    /// exist — Unity does not guarantee OnEnable ordering across different GameObjects at
    /// scene load, only that every object's Awake runs before any object's Start.
    /// </summary>
    public class HUDController : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI scoreLabel;

        private void Start()
        {
            if (ScoreManager.Instance != null)
            {
                ScoreManager.Instance.OnScoreChanged += HandleScoreChanged;
                HandleScoreChanged(ScoreManager.Instance.CurrentScore);
            }
        }

        private void OnDestroy()
        {
            if (ScoreManager.Instance != null)
            {
                ScoreManager.Instance.OnScoreChanged -= HandleScoreChanged;
            }
        }

        private void HandleScoreChanged(int newScore)
        {
            if (scoreLabel != null) scoreLabel.text = newScore.ToString("N0");
        }
    }
}
