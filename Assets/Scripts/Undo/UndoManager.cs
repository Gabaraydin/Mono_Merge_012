using System.Collections.Generic;
using UnityEngine;
using MonoMerge.Grid;
using MonoMerge.Score;
using MonoMerge.Tiles;

namespace MonoMerge.Undo
{
    /// <summary>
    /// GDD 3, Rewarded placement #2: "Hatali hamleyi 'Geri Al' (Undo)." Captures a snapshot of
    /// the grid, tray and score immediately before each placement so Week 4's
    /// Ads/RewardedAdController can offer a one-shot "watch ad to undo" after a bad move.
    /// Only ever holds the single most recent snapshot — the GDD does not ask for a
    /// multi-step undo history, so one slot is deliberately all this supports.
    /// </summary>
    public class UndoManager : MonoBehaviour
    {
        public static UndoManager Instance { get; private set; }

        [SerializeField] private TileSpawner spawner;

        private struct TileSnapshot
        {
            public GridCoordinate coordinate;
            public int tier;
        }

        private List<TileSnapshot> gridSnapshot;
        private List<int> traySnapshot;
        private int scoreSnapshot;

        public bool HasSnapshot { get; private set; }

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

        /// <summary>Call right before a placement is committed (DragDropController.EndDrag,
        /// before RegisterTile). Overwrites any previous snapshot.</summary>
        public void CaptureSnapshot(GridManager grid)
        {
            gridSnapshot = new List<TileSnapshot>();
            for (int x = 0; x < grid.Width; x++)
            {
                for (int y = 0; y < grid.Height; y++)
                {
                    var coord = new GridCoordinate(x, y);
                    GridCell cell = grid.GetCell(coord);
                    if (cell != null && !cell.IsEmpty)
                    {
                        gridSnapshot.Add(new TileSnapshot { coordinate = coord, tier = cell.OccupyingTile.Tier });
                    }
                }
            }

            traySnapshot = new List<int>();
            foreach (Tile tile in spawner.ActiveTrayTiles)
            {
                traySnapshot.Add(tile.Tier);
            }

            scoreSnapshot = ScoreManager.Instance != null ? ScoreManager.Instance.CurrentScore : 0;
            HasSnapshot = true;
        }

        /// <summary>Restores grid, tray and score to the last captured snapshot. Called by
        /// Week 4's RewardedAdController after a successful "watch ad to undo". Consumes the
        /// snapshot — a second undo attempt without a new placement in between does nothing.</summary>
        public void RestoreSnapshot(GridManager grid)
        {
            if (!HasSnapshot) return;

            ClearAllGridTiles(grid);
            ClearAllTrayTiles();

            foreach (var snap in gridSnapshot)
            {
                Tile tile = spawner.SpawnStandaloneTile(snap.tier, grid.GridToWorld(snap.coordinate));
                grid.RegisterTile(tile, snap.coordinate);
            }

            spawner.RebuildTray(traySnapshot);

            // Direct SetScore, not AddMergeScore — reverting a mistake should not count
            // towards a new high score.
            ScoreManager.Instance?.SetScore(scoreSnapshot);

            HasSnapshot = false;
        }

        private void ClearAllGridTiles(GridManager grid)
        {
            for (int x = 0; x < grid.Width; x++)
            {
                for (int y = 0; y < grid.Height; y++)
                {
                    var coord = new GridCoordinate(x, y);
                    GridCell cell = grid.GetCell(coord);
                    if (cell != null && !cell.IsEmpty)
                    {
                        Tile tile = cell.OccupyingTile;
                        grid.RemoveTile(coord);
                        if (tile != null) Destroy(tile.gameObject);
                    }
                }
            }
        }

        private void ClearAllTrayTiles()
        {
            foreach (Tile tile in spawner.ActiveTrayTiles)
            {
                if (tile != null) Destroy(tile.gameObject);
            }
        }
    }
}
