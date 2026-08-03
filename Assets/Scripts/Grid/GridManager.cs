using System;
using UnityEngine;
using MonoMerge.Tiles;

namespace MonoMerge.Grid
{
    /// <summary>
    /// Single source of truth for the 5x5 board state (GDD 1: "Izgara (Grid): Oyun alani 5x5
    /// boyutunda bir kareden olusur."). Owns cell occupancy and world&lt;-&gt;grid conversions.
    ///
    /// Deliberately does NOT move tile transforms on placement (see RegisterTile) — visual
    /// snapping is the presentation layer's job (Input/DragDropController's lerp coroutine),
    /// kept separate so grid logic stays framework-agnostic and easy to unit test later.
    /// </summary>
    public class GridManager : MonoBehaviour
    {
        public static GridManager Instance { get; private set; }

        [SerializeField] private int width = 5;
        [SerializeField] private int height = 5;
        [SerializeField] private float cellSize = 1.2f;
        [SerializeField] private Vector2 originWorldPosition = Vector2.zero;

        private GridCell[,] cells;

        public int Width => width;
        public int Height => height;
        public float CellSize => cellSize;

        /// <summary>Raised whenever a tile is registered into or removed from a cell.
        /// Merge/GameOverChecker (Week 2) subscribe to this rather than being called directly.</summary>
        public event Action OnGridChanged;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            BuildGrid();
        }

        private void OnDestroy()
        {
            if (Instance == this) Instance = null;
        }

        private void BuildGrid()
        {
            cells = new GridCell[width, height];
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    cells[x, y] = new GridCell(new GridCoordinate(x, y));
                }
            }
        }

        public bool IsInsideGrid(GridCoordinate coord) =>
            coord.x >= 0 && coord.x < width && coord.y >= 0 && coord.y < height;

        public bool IsCellEmpty(GridCoordinate coord) =>
            IsInsideGrid(coord) && cells[coord.x, coord.y].IsEmpty;

        public GridCell GetCell(GridCoordinate coord)
        {
            if (!IsInsideGrid(coord)) return null;
            return cells[coord.x, coord.y];
        }

        /// <summary>
        /// Writes a tile into the data model only (no transform movement). Returns false
        /// without side effects if the target cell is occupied or out of bounds.
        /// </summary>
        public bool RegisterTile(Tile tile, GridCoordinate coord)
        {
            if (!IsCellEmpty(coord)) return false;

            cells[coord.x, coord.y].SetTile(tile);
            tile.SetGridCoordinate(coord);
            OnGridChanged?.Invoke();
            return true;
        }

        public void RemoveTile(GridCoordinate coord)
        {
            if (!IsInsideGrid(coord)) return;
            cells[coord.x, coord.y].Clear();
            OnGridChanged?.Invoke();
        }

        public Vector2 GridToWorld(GridCoordinate coord) =>
            originWorldPosition + new Vector2(coord.x * cellSize, coord.y * cellSize);

        /// <summary>Nearest grid cell to a world position. Used by drag &amp; drop to decide
        /// where a released tile is trying to land — does not check emptiness by itself.</summary>
        public GridCoordinate WorldToGrid(Vector2 worldPosition)
        {
            Vector2 local = worldPosition - originWorldPosition;
            int x = Mathf.RoundToInt(local.x / cellSize);
            int y = Mathf.RoundToInt(local.y / cellSize);
            return new GridCoordinate(x, y);
        }

        /// <summary>GDD 1: "Oyun Sonu (Fail State): Izgarada yeni bir tas koyacak yer kalmadiginda
        /// oyun biter." Week 1 exposes raw fullness; Week 2's GameOverChecker additionally confirms
        /// no tray tile fits anywhere before ending the game, since a merge can free up cells.</summary>
        public bool HasAnyEmptyCell()
        {
            for (int x = 0; x < width; x++)
                for (int y = 0; y < height; y++)
                    if (cells[x, y].IsEmpty) return true;
            return false;
        }

        public GridCell[,] GetAllCells() => cells;
    }
}
