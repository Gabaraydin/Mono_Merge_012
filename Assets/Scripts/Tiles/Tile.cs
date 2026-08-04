using TMPro;
using UnityEngine;
using MonoMerge.Grid;

namespace MonoMerge.Tiles
{
    /// <summary>
    /// A single draggable/placed tile instance. Holds its numeric tier and, once placed,
    /// its grid coordinate. Visuals (color + label) are driven entirely by TileTierDatabase
    /// so art direction changes never require touching this script.
    ///
    /// Prefab requirements: SpriteRenderer (tile background) + BoxCollider2D (drag hit-test,
    /// see Input/DragDropController) + a child TextMeshPro for the tier label.
    /// </summary>
    [RequireComponent(typeof(SpriteRenderer))]
    [RequireComponent(typeof(BoxCollider2D))]
    public class Tile : MonoBehaviour
    {
        [SerializeField] private TextMeshPro label;

        private TileTierDatabase tierDatabase;
        private SpriteRenderer spriteRenderer;

        [Tooltip("Alpha applied to a tray tile while it isn't the player's turn to place it yet.")]
        [SerializeField] private float lockedAlpha = 0.35f;

        public int Tier { get; private set; } = 1;
        public GridCoordinate Coordinate { get; private set; }
        public bool IsPlaced { get; private set; }
        public bool IsInteractable { get; private set; } = true;

        private void Awake()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
        }

        /// <summary>Called by TileSpawner right after Instantiate.</summary>
        public void Initialize(int tier, TileTierDatabase database)
        {
            tierDatabase = database;
            SetTier(tier);
        }

        public void SetTier(int tier)
        {
            Tier = tier;
            var definition = tierDatabase != null ? tierDatabase.GetTier(tier) : null;
            if (definition == null) return;

            if (spriteRenderer != null) spriteRenderer.color = definition.color;
            if (label != null) label.text = definition.label;
        }

        /// <summary>Tiles/TileSpawner: only the current tile in the tray's placement order is
        /// interactable — the rest are dimmed and ignored by DragDropController until their turn.</summary>
        public void SetInteractable(bool interactable)
        {
            IsInteractable = interactable;
            if (spriteRenderer == null) return;

            Color c = spriteRenderer.color;
            c.a = interactable ? 1f : lockedAlpha;
            spriteRenderer.color = c;
        }

        /// <summary>Called by GridManager.RegisterTile once placement is accepted into the data model.</summary>
        public void SetGridCoordinate(GridCoordinate coord)
        {
            Coordinate = coord;
            IsPlaced = true;
        }

        public void ClearPlacement()
        {
            IsPlaced = false;
        }
    }
}
