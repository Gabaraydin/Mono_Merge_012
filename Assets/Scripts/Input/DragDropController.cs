using System.Collections;
using UnityEngine;
using MonoMerge.Audio;
using MonoMerge.Core;
using MonoMerge.Grid;
using MonoMerge.Merge;
using MonoMerge.Tiles;
using MonoMerge.Undo;

namespace MonoMerge.Input
{
    /// <summary>
    /// GDD 1: "Yerlestirme: Oyuncu, parmagiyla tasi surukleyip izgaradaki bos bir huecreye
    /// birakir (Drag &amp; Drop)." GDD 4: "Fizik Motoru: Gelismis fizige gerek yoktur. Taslarin
    /// izgaraya oturmasi (Snap to grid) matematiksel lerp fonksiyonlari ile kodlanacaktir."
    ///
    /// Uses legacy Input (mouse on desktop doubles as single-touch on mobile under Unity's
    /// input simulation) and Physics2D.OverlapPoint purely for hit-testing — no rigidbody,
    /// no physics simulation, matching the GDD's "no physics engine needed" instruction.
    /// </summary>
    public class DragDropController : MonoBehaviour
    {
        [SerializeField] private Camera mainCamera;
        [SerializeField] private LayerMask tileLayerMask;
        [SerializeField] private TileSpawner spawner;
        [SerializeField] private float snapDuration = 0.12f;

        private Tile draggedTile;
        private Vector3 dragOffset;
        private bool isDragging;

        private void Reset()
        {
            mainCamera = Camera.main;
        }

        private void Update()
        {
            if (GameManager.Instance != null &&
                GameManager.Instance.CurrentState != GameState.Playing)
            {
                return;
            }

            if (UnityEngine.Input.GetMouseButtonDown(0))
            {
                TryBeginDrag(UnityEngine.Input.mousePosition);
            }
            else if (UnityEngine.Input.GetMouseButton(0) && isDragging)
            {
                ContinueDrag(UnityEngine.Input.mousePosition);
            }
            else if (UnityEngine.Input.GetMouseButtonUp(0) && isDragging)
            {
                EndDrag();
            }
        }

        private void TryBeginDrag(Vector3 screenPosition)
        {
            Vector2 worldPoint = mainCamera.ScreenToWorldPoint(screenPosition);
            Collider2D hit = Physics2D.OverlapPoint(worldPoint, tileLayerMask);
            if (hit == null) return;

            Tile tile = hit.GetComponent<Tile>();
            if (tile == null || tile.IsPlaced || !tile.IsInteractable) return;

            draggedTile = tile;
            dragOffset = tile.transform.position - (Vector3)worldPoint;
            isDragging = true;
        }

        private void ContinueDrag(Vector3 screenPosition)
        {
            Vector2 worldPoint = mainCamera.ScreenToWorldPoint(screenPosition);
            draggedTile.transform.position = (Vector3)worldPoint + dragOffset;
        }

        private void EndDrag()
        {
            isDragging = false;
            GridCoordinate target = GridManager.Instance.WorldToGrid(draggedTile.transform.position);

            if (GridManager.Instance.IsCellEmpty(target))
            {
                // Snapshot BEFORE committing the placement — GDD 3 Rewarded placement #2
                // ("Hatali hamleyi Geri Al") needs to restore exactly what the board looked
                // like right before this move (see Undo/UndoManager).
                UndoManager.Instance?.CaptureSnapshot(GridManager.Instance);

                Vector3 snapPosition = GridManager.Instance.GridToWorld(target);
                GridManager.Instance.RegisterTile(draggedTile, target);
                spawner.NotifyTileConsumed(draggedTile);
                StartCoroutine(SnapToPosition(draggedTile.transform, snapPosition));
                AudioManager.Instance?.PlayPlace();

                // GDD 1: merge check happens right after placement, centered on the tile
                // that just landed (see Merge/MergeManager for the adjacency + cascade logic).
                MergeManager.Instance?.CheckMergeAt(target);
            }
            else
            {
                spawner.ReturnToTray(draggedTile);
            }

            draggedTile = null;
        }

        /// <summary>Pure Vector3.Lerp over snapDuration seconds — the "matematiksel lerp
        /// fonksiyonlari" the GDD asks for instead of physics-based snapping.</summary>
        private IEnumerator SnapToPosition(Transform tileTransform, Vector3 targetPosition)
        {
            Vector3 startPosition = tileTransform.position;
            float elapsed = 0f;

            while (elapsed < snapDuration)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.Clamp01(elapsed / snapDuration);
                tileTransform.position = Vector3.Lerp(startPosition, targetPosition, t);
                yield return null;
            }

            tileTransform.position = targetPosition;
        }
    }
}
