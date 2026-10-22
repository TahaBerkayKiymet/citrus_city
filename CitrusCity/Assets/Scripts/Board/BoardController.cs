// ============================================================
//  BoardController.cs — Citrus City
//  MonoBehaviour: orchestrates board state + view.
//  Owns the FruitView grid, processes gravity + refill with
//  animations, and exposes the API used by SelectionController.
// ============================================================
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CitrusCity.Fruit;
using CitrusCity.Boosters;
using CitrusCity.Core;
using CitrusCity.Objectives;

namespace CitrusCity.Board
{
    public class BoardController : MonoBehaviour
    {
        // ── Inspector refs ───────────────────────────────────
        [SerializeField] private BoardConfig config;
        [SerializeField] private Fruit.FruitViewPool pool;

        // ── State ─────────────────────────────────────────────
        public BoardState State   { get; private set; }
        private FruitView[,] _views;
        private System.Random _rng;
        private FruitType[] _availableFruits;
        private bool _isAnimating;

        public bool IsAnimating => _isAnimating;

        // ── ObjectiveTracker reference ────────────────────────
        private ObjectiveTracker _tracker;

        // ── Initialise ────────────────────────────────────────

        public void Initialize(Levels.LevelData levelData, ObjectiveTracker tracker)
        {
            _tracker        = tracker;
            _availableFruits = levelData.availableFruits;
            _rng            = levelData.seed > 0
                              ? new System.Random(levelData.seed)
                              : new System.Random();

            State = new BoardState(levelData.cols, levelData.rows);
            State.Seed(_availableFruits, levelData.seed > 0 ? levelData.seed : -1);

            _views = new FruitView[levelData.cols, levelData.rows];
            SpawnBoardTiles();
            SpawnAllViews();
        }

        // ── World ↔ Grid coordinate helpers ──────────────────

        public Vector3 CellToWorld(int col, int row)
        {
            float step = config ? config.cellSize + config.cellSpacing
                                : Constants.CellSize + Constants.CellSpacing;
            float boardW = State.Cols * step;
            float boardH = State.Rows * step;
            float x = col * step - boardW * 0.5f + step * 0.5f;
            float y = row * step - boardH * 0.5f + step * 0.5f;
            return new Vector3(x, y, 0f) + transform.position;
        }

        public bool WorldToCell(Vector3 worldPos, out int col, out int row)
        {
            float step = config ? config.cellSize + config.cellSpacing
                                : Constants.CellSize + Constants.CellSpacing;
            float boardW = State.Cols * step;
            float boardH = State.Rows * step;
            Vector3 local = worldPos - transform.position
                          + new Vector3(boardW * 0.5f, boardH * 0.5f, 0f);
            col = Mathf.FloorToInt(local.x / step);
            row = Mathf.FloorToInt(local.y / step);
            return State.IsInBounds(col, row);
        }

        // ── View creation ─────────────────────────────────────

        private Transform _tilesParent;

        private void SpawnBoardTiles()
        {
            if (_tilesParent != null)
                Destroy(_tilesParent.gameObject);

            var tilesGo = new GameObject("BoardTiles");
            _tilesParent = tilesGo.transform;
            _tilesParent.SetParent(transform, false);

            var tileSprite = Resources.Load<Sprite>("Sprites/Board/Board_Tile");
            if (tileSprite == null) return;

            for (int c = 0; c < State.Cols; c++)
            {
                for (int r = 0; r < State.Rows; r++)
                {
                    var cellTile = new GameObject($"Tile_{c}_{r}");
                    cellTile.transform.SetParent(_tilesParent, false);
                    cellTile.transform.position = CellToWorld(c, r);
                    var sr = cellTile.AddComponent<SpriteRenderer>();
                    sr.sprite = tileSprite;
                    sr.sortingLayerName = "Board";
                    sr.sortingOrder = 0;
                }
            }
        }

        private void SpawnAllViews()
        {
            for (int c = 0; c < State.Cols; c++)
            {
                for (int r = 0; r < State.Rows; r++)
                {
                    var cellData = State.Get(c, r);
                    if (!cellData.IsEmpty)
                        CreateView(c, r, cellData);
                }
            }
        }

        private FruitView CreateView(int col, int row, CellData data)
        {
            Vector3 pos  = CellToWorld(col, row);
            var view     = pool.Rent(pos, transform);
            view.Init(col, row, data.Type, data.Booster, data.Dir);
            _views[col, row] = view;
            return view;
        }

        // ── Move resolution ───────────────────────────────────

        /// <summary>
        /// Confirms a player move: removes the group, optionally spawns a booster,
        /// records removals in ObjectiveTracker, applies gravity, refills, animates.
        /// Returns when all animations finish.
        /// </summary>
        public IEnumerator ConfirmMove(
            List<(int col, int row)> group,
            Fruit.FruitType fruitType,
            BoosterType booster,
            BoosterDirection direction,
            int touchCol, int touchRow)
        {
            _isAnimating = true;

            // 1) Record removals for objectives
            foreach (var (c, r) in group)
                _tracker?.RecordRemoval(fruitType);

            // 2) Publish group-confirmed event (for FX)
            EventBus<GroupConfirmedEvent>.Publish(new GroupConfirmedEvent(
                group, fruitType, booster, touchCol, touchRow, direction));

            // 3) Pop animations
            int done = 0;
            foreach (var (c, r) in group)
            {
                var view = _views[c, r];
                if (view != null)
                {
                    var vc = c; var vr = r;
                    view.AnimatePop(() =>
                    {
                        pool.Return(view);
                        _views[vc, vr] = null;
                        done++;
                    });
                    State.Clear(c, r);
                }
                else done++;
            }

            yield return new WaitUntil(() => done >= group.Count);

            // 4) Spawn booster at touch position
            if (booster != BoosterType.None && State.IsInBounds(touchCol, touchRow))
            {
                var mostCommonColor = GetMostCommonColor(group, fruitType);
                State.PlaceSpecial(touchCol, touchRow, booster, direction, mostCommonColor);
                CreateView(touchCol, touchRow, State.Get(touchCol, touchRow));
                yield return new WaitForSeconds(0.1f);
            }

            // 5) Apply gravity
            var fallMoves = GravitySystem.ApplyGravity(State);
            yield return StartCoroutine(AnimateFalls(fallMoves));

            // 6) Refill
            var newCells = GravitySystem.Refill(State, _availableFruits, _rng);
            yield return StartCoroutine(AnimateRefill(newCells));

            EventBus<BoardRefreshedEvent>.Publish(new BoardRefreshedEvent());
            _isAnimating = false;
        }

        // ── Booster activation ────────────────────────────────

        public IEnumerator ActivateBooster(int col, int row)
        {
            _isAnimating = true;

            var cell = State.Get(col, row);
            if (cell.Booster == BoosterType.None)
            {
                _isAnimating = false;
                yield break;
            }

            EventBus<BoosterActivatedEvent>.Publish(
                new BoosterActivatedEvent(col, row, cell.Booster, cell.Dir));

            // Flash the booster view
            if (_views[col, row] != null)
                _views[col, row].AnimateBoosterActivate();

            yield return new WaitForSeconds(0.15f);

            // Clear the booster cell first
            State.Clear(col, row);
            if (_views[col, row] != null)
            {
                pool.Return(_views[col, row]);
                _views[col, row] = null;
            }

            // Apply effect and record objective hits
            var cleared = BoosterEffect.Apply(State, cell.Booster, cell.Dir,
                                              col, row, cell.VortexColor);

            foreach (var (cc, cr) in cleared)
                _tracker?.RecordRemoval(State.TypeAt(cc, cr));

            // Pop cleared views
            int done = 0;
            foreach (var (cc, cr) in cleared)
            {
                var view = _views[cc, cr];
                if (view != null)
                {
                    view.AnimatePop(() => { pool.Return(view); done++; });
                    _views[cc, cr] = null;
                }
                else done++;
            }

            yield return new WaitUntil(() => done >= cleared.Count);

            // Gravity + refill
            var fallMoves = GravitySystem.ApplyGravity(State);
            yield return StartCoroutine(AnimateFalls(fallMoves));

            var newCells = GravitySystem.Refill(State, _availableFruits, _rng);
            yield return StartCoroutine(AnimateRefill(newCells));

            EventBus<BoardRefreshedEvent>.Publish(new BoardRefreshedEvent());
            _isAnimating = false;
        }

        // ── Animation coroutines ──────────────────────────────

        private IEnumerator AnimateFalls(List<FruitFallMove> moves)
        {
            float speed = config ? config.fallSpeed : Constants.FruitFallSpeed;
            float stagger = config ? config.fallStagger : Constants.FruitFallDelay;

            // Group by column for stagger
            var byCol = new Dictionary<int, List<FruitFallMove>>();
            foreach (var m in moves)
            {
                if (!byCol.ContainsKey(m.Col)) byCol[m.Col] = new();
                byCol[m.Col].Add(m);
            }

            int totalDone = 0;
            int total     = moves.Count;

            foreach (var (col, colMoves) in byCol)
            {
                yield return new WaitForSeconds(stagger);
                foreach (var move in colMoves)
                {
                    var view = _views[move.Col, move.FromRow];
                    if (view == null) { totalDone++; continue; }

                    _views[move.Col, move.ToRow] = view;
                    _views[move.Col, move.FromRow] = null;

                    Vector3 target = CellToWorld(move.Col, move.ToRow);
                    view.AnimateFallTo(target, speed, () => totalDone++);
                }
            }

            yield return new WaitUntil(() => totalDone >= total);
        }

        private IEnumerator AnimateRefill(List<(int col, int row, FruitType type)> newCells)
        {
            float speed = config ? config.fallSpeed : Constants.FruitFallSpeed;

            foreach (var (col, row, type) in newCells)
            {
                // Spawn above the board
                Vector3 target = CellToWorld(col, row);
                Vector3 start  = CellToWorld(col, State.Rows);
                var view = pool.Rent(start, transform);
                view.Init(col, row, type);
                _views[col, row] = view;
                view.AnimateSpawnFrom(start, target, speed);
                yield return new WaitForSeconds(Constants.FruitFallDelay * 0.5f);
            }

            // Give time for all to land
            yield return new WaitForSeconds(0.3f);
        }

        // ── Selection highlight passthrough ───────────────────

        public void HighlightGroup(List<(int col, int row)> group, bool selected)
        {
            foreach (var (c, r) in group)
                if (_views[c, r] != null)
                    _views[c, r].SetSelected(selected);
        }

        // ── Utility ───────────────────────────────────────────

        private Fruit.FruitType GetMostCommonColor(
            List<(int col, int row)> group, Fruit.FruitType fallback)
        {
            // For now, groups are always single color, so just return it
            return fallback;
        }

        public FruitView GetView(int col, int row) =>
            State.IsInBounds(col, row) ? _views[col, row] : null;
    }
}
