using System;

namespace MonoMerge.Grid
{
    /// <summary>
    /// Integer address of a single grid cell. Kept separate from Vector2/Vector3 so grid
    /// logic never accidentally mixes world-space math with cell-index math.
    /// </summary>
    [Serializable]
    public struct GridCoordinate : IEquatable<GridCoordinate>
    {
        public int x;
        public int y;

        public GridCoordinate(int x, int y)
        {
            this.x = x;
            this.y = y;
        }

        public static readonly GridCoordinate Up = new GridCoordinate(0, 1);
        public static readonly GridCoordinate Down = new GridCoordinate(0, -1);
        public static readonly GridCoordinate Left = new GridCoordinate(-1, 0);
        public static readonly GridCoordinate Right = new GridCoordinate(1, 0);

        public static GridCoordinate operator +(GridCoordinate a, GridCoordinate b) =>
            new GridCoordinate(a.x + b.x, a.y + b.y);

        public bool Equals(GridCoordinate other) => x == other.x && y == other.y;
        public override bool Equals(object obj) => obj is GridCoordinate other && Equals(other);
        public override int GetHashCode() => (x * 397) ^ y;
        public override string ToString() => $"({x}, {y})";
    }
}
