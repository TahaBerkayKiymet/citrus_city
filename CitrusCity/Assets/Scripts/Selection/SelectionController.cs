// ============================================================
//  SelectionController.cs — Citrus City
//  MonoBehaviour: touch/mouse input → SelectionState updates.
//
//  Input flow:
//    PointerDown → find cell → find group → highlight → show preview
//    PointerMove → update touch cell → update direction → update preview
//    PointerUp   → if inside group boundary → confirm move
//                  if outside → cancel
//
//  Supports mouse in Editor via Input.GetMouseButton + Camera.main.
//  Supports multi-touch on device via Input.GetTouch.
// ============================================================
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CitrusCity.Board;
using CitrusCity.Boosters;
using CitrusCity.Core;
using CitrusCity.Selection;

namespace CitrusCity.Selection
{
    public class SelectionController : MonoBehaviour
    {
        // ── Inspector refs ───────────────────────────────────
        [SerializeField] private BoardController board;
        [SerializeField] private BoosterPreviewView preview;

        // ── State ─────────────────────────────────────────────
        private readonly SelectionState _sel = new();
        private bool _pointerDown;
        private bool _moveInProgress;

        // ── Objective tracker ref (needed for move consume) ───
        private Objectives.ObjectiveTracker _tracker;

        public void SetTracker(Objectives.ObjectiveTracker t) => _tracker = t;

        // ── Lifecycle ─────────────────────────────────────────

        private void Update()
        {
            if (_moveInProgress || board == null || board.IsAnimating) return;

#if UNITY_EDITOR || UNITY_STANDALONE
            HandleMouse();
#else
            HandleTouch();
#endif
        }

        // ── Mouse (Editor / Desktop) ──────────────────────────

        private void HandleMouse()
        {
            if (Input.GetMouseButtonDown(0))
                OnPointerDown(GetWorldPos(Input.mousePosition));
            else if (Input.GetMouseButton(0) && _pointerDown)
                OnPointerMove(GetWorldPos(Input.mousePosition));
            else if (Input.GetMouseButtonUp(0) && _pointerDown)
                OnPointerUp(GetWorldPos(Input.mousePosition));
        }

        // ── Touch (Mobile) ────────────────────────────────────

        private void HandleTouch()
        {
            if (Input.touchCount == 0) return;
            var t = Input.GetTouch(0);
            var world = GetWorldPos(t.position);

            switch (t.phase)
            {
                case TouchPhase.Began:    OnPointerDown(world); break;
                case TouchPhase.Moved:
                case TouchPhase.Stationary: OnPointerMove(world); break;
                case TouchPhase.Ended:
                case TouchPhase.Canceled: OnPointerUp(world);   break;
            }
        }

        // ── Core handlers ─────────────────────────────────────

        private void OnPointerDown(Vector3 worldPos)
        {
            _pointerDown = false;

            if (!board.WorldToCell(worldPos, out int col, out int row)) return;

            var cellData = board.State.Get(col, row);

            // Case A: tapping on a booster to activate it
            if (cellData.Booster != BoosterType.None)
            {
                StartCoroutine(ActivateBoosterFlow(col, row));
                return;
            }

            // Case B: tapping a regular fruit → start selection
            var group = GroupFinder.FindGroup(board.State, col, row);
            if (group.Count < Constants.MinGroupSize)
            {
                _sel.Clear();
                return;
            }

            _pointerDown = true;
            _sel.SetGroup(group, cellData.Type);
            _sel.SetTouchCell(col, row);

            board.HighlightGroup(group, true);
            UpdatePreview(col, row);
        }

        private void OnPointerMove(Vector3 worldPos)
        {
            if (!_sel.HasSelection) return;

            board.WorldToCell(worldPos, out int col, out int row);
            _sel.SetTouchCell(col, row);
            UpdatePreview(col, row);

            // Check if pointer has left the group bounding box → cancel
            var (minC, minR, maxC, maxR) = GroupFinder.GetBounds(_sel.Group);
            bool inside = col >= minC && col <= maxC && row >= minR && row <= maxR;
            if (!inside)
            {
                CancelSelection();
            }
        }

        private void OnPointerUp(Vector3 worldPos)
        {
            _pointerDown = false;

            if (!_sel.IsValid)
            {
                CancelSelection();
                return;
            }

            // Check pointer is still inside any group cell
            board.WorldToCell(worldPos, out int col, out int row);
            bool insideGroup = _sel.Group.Contains((col, row));

            // Also accept release anywhere within the bounding box
            var (minC, minR, maxC, maxR) = GroupFinder.GetBounds(_sel.Group);
            bool insideBounds = col >= minC && col <= maxC && row >= minR && row <= maxR;

            if (insideGroup || insideBounds)
                StartCoroutine(ConfirmSelectionFlow());
            else
                CancelSelection();
        }

        // ── Move flows ────────────────────────────────────────

        private IEnumerator ConfirmSelectionFlow()
        {
            _moveInProgress = true;
            preview.Hide();
            board.HighlightGroup(_sel.Group, false);

            var group     = new List<(int, int)>(_sel.Group);
            var fruitType = _sel.FruitType;
            var booster   = _sel.PendingBooster;
            var dir       = _sel.Direction;
            int tCol      = _sel.TouchCol;
            int tRow      = _sel.TouchRow;

            _sel.Clear();

            _tracker?.ConsumeMove();

            yield return StartCoroutine(board.ConfirmMove(group, fruitType, booster, dir, tCol, tRow));

            _moveInProgress = false;
        }

        private IEnumerator ActivateBoosterFlow(int col, int row)
        {
            _moveInProgress = true;
            _tracker?.ConsumeMove();
            yield return StartCoroutine(board.ActivateBooster(col, row));
            _moveInProgress = false;
        }

        private void CancelSelection()
        {
            board.HighlightGroup(_sel.Group, false);
            _sel.Clear();
            preview.Hide();
            _pointerDown = false;
        }

        // ── Preview update ────────────────────────────────────

        private void UpdatePreview(int col, int row)
        {
            if (!_sel.HasSelection || _sel.PendingBooster == BoosterType.None)
            {
                preview.Hide();
                return;
            }

            Vector3 worldPos = board.CellToWorld(_sel.TouchCol, _sel.TouchRow);
            preview.Show(worldPos, _sel.PendingBooster, _sel.Direction, _sel.FruitType);
        }

        // ── Utility ───────────────────────────────────────────

        private static Vector3 GetWorldPos(Vector3 screenPos)
        {
            var cam = Camera.main;
            if (cam == null) return Vector3.zero;
            screenPos.z = Mathf.Abs(cam.transform.position.z);
            return cam.ScreenToWorldPoint(screenPos);
        }
    }
}
