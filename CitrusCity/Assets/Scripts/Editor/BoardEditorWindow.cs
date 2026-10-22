// ============================================================
//  BoardEditorWindow.cs — Citrus City
//  Unity Editor window: generate board previews, run logic
//  tests without entering Play mode.
// ============================================================
#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using CitrusCity.Board;
using CitrusCity.Fruit;
using CitrusCity.Selection;
using CitrusCity.Boosters;

namespace CitrusCity.Editor
{
    public class BoardEditorWindow : EditorWindow
    {
        private int _cols     = 8;
        private int _rows     = 9;
        private int _seed     = 42;
        private BoardState _board;
        private string _log   = "";

        [MenuItem("CitrusCity/Board Debug Window")]
        public static void ShowWindow()
        {
            GetWindow<BoardEditorWindow>("CC Board Debug");
        }

        private void OnGUI()
        {
            GUILayout.Label("Board Configuration", EditorStyles.boldLabel);
            _cols = EditorGUILayout.IntField("Columns", _cols);
            _rows = EditorGUILayout.IntField("Rows",    _rows);
            _seed = EditorGUILayout.IntField("Seed",    _seed);

            GUILayout.Space(8);

            if (GUILayout.Button("Generate Board"))
                GenerateBoard();

            if (GUILayout.Button("Run Logic Tests"))
                RunTests();

            if (GUILayout.Button("Clear"))
            { _board = null; _log = ""; }

            GUILayout.Space(8);

            // Draw board grid
            if (_board != null)
                DrawBoardGrid();

            GUILayout.Space(8);
            GUILayout.Label("Log:", EditorStyles.boldLabel);
            GUILayout.TextArea(_log, GUILayout.Height(200));
        }

        private void GenerateBoard()
        {
            _board = new BoardState(_cols, _rows);
            var fruits = new[] {
                FruitType.Orange, FruitType.Lemon,
                FruitType.Lime, FruitType.Cherry, FruitType.Grape
            };
            _board.Seed(fruits, _seed);
            _log += $"Board generated {_cols}×{_rows} seed={_seed}\n";
        }

        private void DrawBoardGrid()
        {
            float cellPx = 28f;
            float startX = 10f;
            float startY = 130f;

            for (int r = _board.Rows - 1; r >= 0; r--)
            {
                for (int c = 0; c < _board.Cols; c++)
                {
                    var type = _board.TypeAt(c, r);
                    var color = FruitDataRegistry.GetColor(type);
                    var rect = new Rect(startX + c * cellPx,
                                       startY + (_board.Rows - 1 - r) * cellPx,
                                       cellPx - 2f, cellPx - 2f);
                    EditorGUI.DrawRect(rect, color);
                    GUI.Label(new Rect(rect.x + 2, rect.y + 2, cellPx, cellPx),
                              type.ToString().Substring(0, 2),
                              new GUIStyle { fontSize = 8, normal = { textColor = Color.black } });
                }
            }
        }

        private void RunTests()
        {
            _log = "=== Logic Tests ===\n";

            // Test 1: GroupFinder
            {
                var b = new BoardState(5, 5);
                var fruits = new[] { FruitType.Orange, FruitType.Lemon };
                b.Seed(fruits, 1);
                // Force a known connected group
                b.PlaceFruit(0, 0, FruitType.Orange);
                b.PlaceFruit(1, 0, FruitType.Orange);
                b.PlaceFruit(2, 0, FruitType.Orange);
                var group = GroupFinder.FindGroup(b, 0, 0);
                bool pass = group.Contains((1, 0)) && group.Contains((2, 0));
                _log += $"  GroupFinder: {(pass ? "PASS" : "FAIL")} (found {group.Count} cells)\n";
            }

            // Test 2: BoosterFactory
            {
                bool p5 = BoosterFactory.GetBoosterForGroupSize(5) == BoosterType.Rocket;
                bool p7 = BoosterFactory.GetBoosterForGroupSize(7) == BoosterType.Bomb;
                bool p9 = BoosterFactory.GetBoosterForGroupSize(9) == BoosterType.Vortex;
                bool p4 = BoosterFactory.GetBoosterForGroupSize(4) == BoosterType.None;
                _log += $"  BoosterFactory: {(p5 && p7 && p9 && p4 ? "PASS" : "FAIL")}\n";
            }

            // Test 3: GravitySystem
            {
                var b = new BoardState(3, 4);
                b.PlaceFruit(0, 0, FruitType.Orange);
                b.PlaceFruit(0, 1, FruitType.Orange);
                // Leave (0,2) empty
                b.PlaceFruit(0, 3, FruitType.Lemon);
                var moves = GravitySystem.ApplyGravity(b);
                bool pass = b.TypeAt(0, 2) == FruitType.Lemon &&
                            b.TypeAt(0, 3) == FruitType.None;
                _log += $"  GravitySystem: {(pass ? "PASS" : "FAIL")} (moves={moves.Count})\n";
            }

            // Test 4: BoosterEffect.Rocket
            {
                var b = new BoardState(5, 5);
                for (int c = 0; c < 5; c++) b.PlaceFruit(c, 2, FruitType.Orange);
                b.PlaceSpecial(2, 2, BoosterType.Rocket, BoosterDirection.Horizontal);
                var cleared = BoosterEffect.ApplyRocket(b, BoosterDirection.Horizontal, 2, 2);
                bool pass = cleared.Count == 5 && b.IsEmpty(0, 2) && b.IsEmpty(4, 2);
                _log += $"  Rocket(H): {(pass ? "PASS" : "FAIL")} (cleared={cleared.Count})\n";
            }

            // Test 5: BoosterEffect.Bomb
            {
                var b = new BoardState(5, 5);
                for (int c = 0; c < 5; c++)
                    for (int r = 0; r < 5; r++)
                        b.PlaceFruit(c, r, FruitType.Grape);
                var cleared = BoosterEffect.ApplyBomb(b, 2, 2, 1);
                bool pass = cleared.Count == 9;
                _log += $"  Bomb: {(pass ? "PASS" : "FAIL")} (cleared={cleared.Count})\n";
            }

            // Test 6: Vortex
            {
                var b = new BoardState(4, 4);
                b.PlaceFruit(0, 0, FruitType.Cherry);
                b.PlaceFruit(1, 1, FruitType.Cherry);
                b.PlaceFruit(2, 2, FruitType.Lemon);
                var cleared = BoosterEffect.ApplyVortex(b, FruitType.Cherry);
                bool pass = cleared.Count == 2 && !b.IsEmpty(2, 2);
                _log += $"  Vortex: {(pass ? "PASS" : "FAIL")} (cleared={cleared.Count})\n";
            }

            // Test 7: SelectionState direction
            {
                var sel = new Selection.SelectionState();
                var group = new System.Collections.Generic.List<(int, int)>
                    { (0,0),(1,0),(2,0),(3,0),(4,0) };   // horizontal row
                sel.SetGroup(group, FruitType.Orange);
                sel.SetTouchCell(0, 0);   // left edge
                bool pass = sel.Direction == BoosterDirection.Horizontal;
                _log += $"  SelectionDir: {(pass ? "PASS" : $"FAIL (got {sel.Direction})")}\n";
            }

            _log += "=== Tests Done ===\n";
        }
    }
}
#endif
