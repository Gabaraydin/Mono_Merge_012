using System.Collections.Generic;
using MonoMerge.Grid;

namespace MonoMerge.Merge
{
    /// <summary>
    /// GDD 1: "Yan yana veya ust uste ayni numaradan/sekilden 3 adet geldiginde, bunlar
    /// birlesirler." Interpreted as: an orthogonally-connected cluster of same-tier tiles
    /// (BFS through Up/Down/Left/Right) merges as one group as soon as its size reaches 3+.
    /// Chosen over a strict "exactly 3 in a line" rule because it is simpler to reason about,
    /// still matches every example in the GDD, and scales naturally to bigger clusters without
    /// extra rules. Pure grid-data logic — no MonoBehaviour, so it stays easy to unit test.
    /// </summary>
    public static class MatchFinder
    {
        private static readonly GridCoordinate[] Directions =
        {
            GridCoordinate.Up, GridCoordinate.Down, GridCoordinate.Left, GridCoordinate.Right
        };

        /// <summary>Minimum connected same-tier tiles required to trigger a merge (GDD: "3 adet").</summary>
        public const int MinMergeGroupSize = 3;

        /// <summary>
        /// BFS flood fill starting at origin, collecting every orthogonally-connected tile
        /// that shares origin's tier. Returns an empty list if origin itself is empty.
        /// </summary>
        public static List<GridCoordinate> FindConnectedGroup(GridManager grid, GridCoordinate origin)
        {
            var result = new List<GridCoordinate>();
            GridCell originCell = grid.GetCell(origin);
            if (originCell == null || originCell.IsEmpty) return result;

            int tier = originCell.OccupyingTile.Tier;
            var visited = new HashSet<GridCoordinate> { origin };
            var queue = new Queue<GridCoordinate>();
            queue.Enqueue(origin);

            while (queue.Count > 0)
            {
                GridCoordinate current = queue.Dequeue();
                result.Add(current);

                foreach (var dir in Directions)
                {
                    GridCoordinate neighborCoord = current + dir;
                    if (visited.Contains(neighborCoord)) continue;
                    if (!grid.IsInsideGrid(neighborCoord)) continue;

                    GridCell neighborCell = grid.GetCell(neighborCoord);
                    if (neighborCell == null || neighborCell.IsEmpty) continue;
                    if (neighborCell.OccupyingTile.Tier != tier) continue;

                    visited.Add(neighborCoord);
                    queue.Enqueue(neighborCoord);
                }
            }

            return result;
        }
    }
}
