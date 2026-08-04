using UnityEngine;

namespace MonoMerge.Grid
{
    /// <summary>
    /// Spawns one cell_background sprite per grid cell so the 5x5 board reads as an actual
    /// grid with visible cell boundaries. The scene originally placed a single cell_background
    /// sprite stretched over the whole board, giving the player no visual cue of where one
    /// cell ends and the next begins. Purely presentational — GridManager's data model is
    /// unaffected either way.
    /// </summary>
    public class GridVisualizer : MonoBehaviour
    {
        [SerializeField] private GridManager grid;
        [SerializeField] private Sprite cellSprite;
        [Tooltip("World-space size of each cell sprite; kept slightly under GridManager's cellSize so a thin gap outlines every cell.")]
        [SerializeField] private float cellVisualSize = 1.08f;

        private void Start()
        {
            if (grid == null || cellSprite == null) return;

            float scale = cellVisualSize / cellSprite.bounds.size.x;

            for (int x = 0; x < grid.Width; x++)
            {
                for (int y = 0; y < grid.Height; y++)
                {
                    Vector2 worldPosition = grid.GridToWorld(new GridCoordinate(x, y));

                    var cellObject = new GameObject($"Cell_{x}_{y}");
                    cellObject.transform.SetParent(transform);
                    cellObject.transform.position = worldPosition;
                    cellObject.transform.localScale = Vector3.one * scale;

                    var renderer = cellObject.AddComponent<SpriteRenderer>();
                    renderer.sprite = cellSprite;
                    renderer.sortingOrder = -1;
                }
            }
        }
    }
}
