using UnityEngine;
using MonoMerge.Audio;
using MonoMerge.Grid;
using MonoMerge.Score;
using MonoMerge.Tiles;
using MonoMerge.VFX;

namespace MonoMerge.Merge
{
    /// <summary>
    /// GDD 1: "Birlesme (Merge): ... bunlar birlesirler bir sonraki seviyedeki tasa donusur
    /// (Orn: Uc adet '1' tasi birlesip bir adet '2' tasi olur)."
    ///
    /// Called explicitly by DragDropController right after a successful placement — NOT via
    /// GridManager.OnGridChanged — because this class itself calls RemoveTile while resolving
    /// a merge, which would otherwise re-trigger the very event it listens to.
    /// </summary>
    public class MergeManager : MonoBehaviour
    {
        public static MergeManager Instance { get; private set; }

        [SerializeField] private TileTierDatabase tierDatabase;
        [Tooltip("GDD 2: black-dot particle burst played at the merge position.")]
        [SerializeField] private MergeParticleEffect mergeParticlePrefab;

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

        /// <summary>
        /// Checks for and resolves a merge centered on the tile that was just placed at
        /// <paramref name="coord"/>. Cascades automatically: if the upgraded tile forms a new
        /// group with its neighbors, it merges again in the same call.
        /// </summary>
        public void CheckMergeAt(GridCoordinate coord)
        {
            GridManager grid = GridManager.Instance;
            var group = MatchFinder.FindConnectedGroup(grid, coord);

            if (group.Count < MatchFinder.MinMergeGroupSize) return;

            GridCell originCell = grid.GetCell(coord);
            int currentTier = originCell.OccupyingTile.Tier;

            if (!tierDatabase.HasNextTier(currentTier))
            {
                // GDD does not define behaviour past the last tier — stop merging rather than
                // crash or wrap around. Revisit if/when a "max tier" reward is designed.
                return;
            }

            // Destroy every tile in the group except the one at `coord`, which is upgraded in place.
            foreach (var groupCoord in group)
            {
                if (groupCoord.Equals(coord)) continue;

                GridCell cell = grid.GetCell(groupCoord);
                Tile consumedTile = cell.OccupyingTile;
                grid.RemoveTile(groupCoord);
                if (consumedTile != null) Destroy(consumedTile.gameObject);
            }

            int nextTier = currentTier + 1;
            originCell.OccupyingTile.SetTier(nextTier);

            ScoreManager.Instance?.AddMergeScore(group.Count, nextTier);

            // GDD 2 "Juiciness": particle burst + camera shake + SFX, all centered on the
            // merge position, none of them required for the merge logic itself to be correct.
            Vector3 worldPosition = grid.GridToWorld(coord);
            MergeParticleEffect.SpawnAt(mergeParticlePrefab, worldPosition);
            CameraShake.Instance?.Shake();
            AudioManager.Instance?.PlayMerge();

            // Cascade check: the newly upgraded tile may now connect with different neighbors.
            CheckMergeAt(coord);
        }
    }
}
