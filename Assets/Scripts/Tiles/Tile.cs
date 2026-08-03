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

        public int Tier { get; private set; } = 1;
        public GridCoordinate Coordinate { get; private set; }
        public bool IsPlaced { get; private set; }

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
