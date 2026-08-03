using System;
using UnityEngine;
using MonoMerge.Grid;
using MonoMerge.Score;
using MonoMerge.Tiles;

namespace MonoMerge.Core
{
    /// <summary>
    /// Top-level game state machine: MainMenu/Playing/Paused/GameOver. Wires a fresh run's
    /// score reset and tile spawn (StartGame) and drives GameOver off GameOverChecker.
    /// UI (Week 3) and Ads (Week 4) hook into OnStateChanged rather than polling this class.
    /// </summary>
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }

        [SerializeField] private TileSpawner spawner;

        public GameState CurrentState { get; private set; } = GameState.MainMenu;

        public event Action<GameState> OnStateChanged;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
        }

        // Subscribing in Start (not Awake/OnEnable) guarantees GridManager.Instance already
        // exists — Unity runs every Awake() in the scene before any Start().
        private void Start()
        {
            if (GridManager.Instance != null)
            {
                GridManager.Instance.OnGridChanged += HandleGridChanged;
            }
        }

        private void OnDestroy()
        {
            if (GridManager.Instance != null)
            {
                GridManager.Instance.OnGridChanged -= HandleGridChanged;
            }
            if (Instance == this) Instance = null;
        }

        /// <summary>GDD 6 (Son Not): oyuncu acilista 3 saniye icinde oynamaya baslayabilmeli —
        /// no tutorial gate here, StartGame goes straight to Playing.</summary>
        public void StartGame()
        {
            SetState(GameState.Playing);
            ScoreManager.Instance?.ResetScore();
            spawner.SpawnNextBatch();
        }

        public void SetState(GameState newState)
        {
            if (CurrentState == newState) return;
            CurrentState = newState;
            OnStateChanged?.Invoke(newState);
        }

        /// <summary>GDD 1: "Oyun Sonu (Fail State): Izgarada yeni bir tas koyacak yer kalmadiginda
        /// oyun biter." Delegates to Core.GameOverChecker (Week 2) so the end condition itself
        /// is independently testable rather than inlined here.</summary>
        private void HandleGridChanged()
        {
            if (CurrentState != GameState.Playing) return;

            if (GameOverChecker.IsGameOver(GridManager.Instance))
            {
                SetState(GameState.GameOver);
            }
        }
    }
}
