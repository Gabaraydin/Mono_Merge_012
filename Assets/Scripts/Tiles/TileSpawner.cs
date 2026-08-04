using System.Collections.Generic;
using UnityEngine;

namespace MonoMerge.Tiles
{
    /// <summary>
    /// GDD 1: "Spawn (Uretim): Ekranin alt kisminda her tur oyuncuya rastgele 2 veya 3 sekil
    /// verilir." Owns the spawn tray — a fixed set of slot positions below the grid — and
    /// refills it once every tile from the current batch has been placed.
    ///
    /// Tiles must be placed in a fixed order — rightmost slot first, then the next one to its
    /// left, and so on — rather than freely in any order. Only the current tile in that order
    /// is draggable (Tile.IsInteractable); the rest are dimmed. This forces the player to plan
    /// ahead for the whole batch instead of just cherry-picking the easiest tile each turn.
    /// </summary>
    public class TileSpawner : MonoBehaviour
    {
        [SerializeField] private Tile tilePrefab;
        [SerializeField] private TileTierDatabase tierDatabase;
        [Tooltip("Slot transforms below the grid. Needs at least maxTilesPerTurn entries.")]
        [SerializeField] private Transform[] traySlots;
        [SerializeField] private int minTilesPerTurn = 2;
        [SerializeField] private int maxTilesPerTurn = 3;

        private readonly List<Tile> activeTrayTiles = new List<Tile>();
        private readonly List<Tile> placementOrder = new List<Tile>();

        public IReadOnlyList<Tile> ActiveTrayTiles => activeTrayTiles;
        public bool TrayEmpty => activeTrayTiles.Count == 0;

        /// <summary>Call once at game start (GameManager.StartGame) and again whenever the tray empties.</summary>
        public void SpawnNextBatch()
        {
            activeTrayTiles.Clear();

            int count = Mathf.Min(Random.Range(minTilesPerTurn, maxTilesPerTurn + 1), traySlots.Length);

            for (int i = 0; i < count; i++)
            {
                int tier = Random.Range(1, tierDatabase.SpawnableTierCount + 1);
                Tile tile = Instantiate(tilePrefab, traySlots[i].position, Quaternion.identity);
                tile.Initialize(tier, tierDatabase);
                activeTrayTiles.Add(tile);
            }

            BuildPlacementOrder();
        }

        /// <summary>Rightmost tray slot goes first, then leftward — see class doc.</summary>
        private void BuildPlacementOrder()
        {
            placementOrder.Clear();
            for (int i = activeTrayTiles.Count - 1; i >= 0; i--)
            {
                placementOrder.Add(activeTrayTiles[i]);
            }
            RefreshInteractable();
        }

        /// <summary>Only the front of placementOrder is draggable; every other tray tile is locked.</summary>
        private void RefreshInteractable()
        {
            for (int i = 0; i < placementOrder.Count; i++)
            {
                placementOrder[i].SetInteractable(i == 0);
            }
        }

        /// <summary>Called by DragDropController once a tray tile has been successfully placed on the grid.</summary>
        public void NotifyTileConsumed(Tile tile)
        {
            activeTrayTiles.Remove(tile);
            placementOrder.Remove(tile);

            if (activeTrayTiles.Count == 0)
            {
                SpawnNextBatch();
            }
            else
            {
                RefreshInteractable();
            }
        }

        /// <summary>Snaps a tile back to its tray slot after a failed placement attempt (dropped
        /// on an occupied cell or outside the grid).</summary>
        public void ReturnToTray(Tile tile)
        {
            int index = activeTrayTiles.IndexOf(tile);
            if (index >= 0 && index < traySlots.Length)
            {
                tile.transform.position = traySlots[index].position;
            }
        }

        /// <summary>Creates a tile outside the tray system entirely — used by
        /// Undo/UndoManager to rebuild grid tiles from a snapshot.</summary>
        public Tile SpawnStandaloneTile(int tier, Vector3 worldPosition)
        {
            Tile tile = Instantiate(tilePrefab, worldPosition, Quaternion.identity);
            tile.Initialize(tier, tierDatabase);
            return tile;
        }

        /// <summary>Replaces the current tray with tiles matching the given tiers, one per
        /// slot in order. Used by Undo/UndoManager to restore the tray from a snapshot —
        /// does not go through SpawnNextBatch's randomization.</summary>
        public void RebuildTray(IReadOnlyList<int> tiers)
        {
            activeTrayTiles.Clear();
            for (int i = 0; i < tiers.Count && i < traySlots.Length; i++)
            {
                Tile tile = Instantiate(tilePrefab, traySlots[i].position, Quaternion.identity);
                tile.Initialize(tiers[i], tierDatabase);
                activeTrayTiles.Add(tile);
            }

            BuildPlacementOrder();
        }
    }
}
