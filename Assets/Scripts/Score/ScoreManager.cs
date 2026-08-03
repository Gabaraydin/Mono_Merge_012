using System;
using UnityEngine;
using MonoMerge.Core;

namespace MonoMerge.Score
{
    /// <summary>
    /// GDD 1 &amp; 5: score increases on every merge ("skor hesaplamalari" — Week 2 roadmap
    /// item). Formula is intentionally simple and designer-tunable via a single Inspector
    /// field, per the project's "karmasiklikdan uzak dur" mandate: no formula tables, no
    /// hidden constants in code.
    /// </summary>
    public class ScoreManager : MonoBehaviour
    {
        public static ScoreManager Instance { get; private set; }

        [Tooltip("Score gained on merge = mergedTileCount * resultingTier * this value.")]
        [SerializeField] private int scorePerTierPoint = 10;

        public int CurrentScore { get; private set; }

        public event Action<int> OnScoreChanged;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
        }

        private void OnDestroy()
        {
            if (Instance == this) Instance = null;
        }

        /// <summary>Called by MergeManager immediately after a merge resolves.</summary>
        public void AddMergeScore(int mergedTileCount, int resultingTier)
        {
            int gained = mergedTileCount * resultingTier * scorePerTierPoint;
            CurrentScore += gained;
            OnScoreChanged?.Invoke(CurrentScore);
            SaveManager.Instance?.TryUpdateHighScore(CurrentScore);
        }

        /// <summary>Call when a new run starts (GameManager.StartGame).</summary>
        public void ResetScore()
        {
            CurrentScore = 0;
            OnScoreChanged?.Invoke(CurrentScore);
        }

        /// <summary>Directly sets the score without touching the high score — used by
        /// Undo/UndoManager when reverting to a snapshot. A revert must not create a new
        /// high score, which is why this bypasses AddMergeScore entirely.</summary>
        public void SetScore(int score)
        {
            CurrentScore = score;
            OnScoreChanged?.Invoke(CurrentScore);
        }
    }
}
