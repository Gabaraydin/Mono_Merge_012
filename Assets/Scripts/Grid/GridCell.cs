using MonoMerge.Tiles;

namespace MonoMerge.Grid
{
    /// <summary>
    /// State of a single cell: either empty or occupied by exactly one tile.
    /// Pure data holder — no Unity API dependency, so it stays trivially testable.
    /// </summary>
    public class GridCell
    {
        public GridCoordinate Coordinate { get; }
        public Tile OccupyingTile { get; private set; }
        public bool IsEmpty => OccupyingTile == null;

        public GridCell(GridCoordinate coordinate)
        {
            Coordinate = coordinate;
        }

        public void SetTile(Tile tile) => OccupyingTile = tile;
        public void Clear() => OccupyingTile = null;
    }
}
