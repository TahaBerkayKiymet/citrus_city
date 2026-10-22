// ============================================================
//  BoardState.cs — Citrus City
//  Pure C# board model. No Unity types except Vector2Int.
//  Owns the 2-D grid of cell data; all reads/writes go here.
//  Visual presentation is owned by BoardController + FruitView.
// ============================================================
using System;
using System.Collections.Generic;
using UnityEngine;
using CitrusCity.Fruit;
using CitrusCity.Boosters;

namespace CitrusCity.Board
{
    /// <summary>Data stored for each grid cell.</summary>
    public struct CellData
    {
        public FruitType Type;          // None if empty
        public BoosterType Booster;     // None if not a special item
        public BoosterDirection Dir;    // meaningful only for Rocket
        public FruitType VortexColor;  // which color a Vortex will clear (set at spawn)

        public bool IsEmpty    => Type == FruitType.None;
        public bool IsSpecial  => Type.IsSpecial();
    }

    public class BoardState
    {
        public int Cols { get; }
        public int Rows { get; }

        private readonly CellData[,] _grid;

        // ── Construction ─────────────────────────────────────

        public BoardState(int cols, int rows)
        {
            Cols  = cols;
            Rows  = rows;
            _grid = new CellData[cols, rows];
        }

        // ── Accessors ─────────────────────────────────────────

        public CellData Get(int col, int row) => _grid[col, row];
        public void Set(int col, int row, CellData data) => _grid[col, row] = data;

        public bool IsInBounds(int col, int row) =>
            col >= 0 && col < Cols && row >= 0 && row < Rows;

        public bool IsEmpty(int col, int row) =>
            !IsInBounds(col, row) || _grid[col, row].IsEmpty;

        public FruitType TypeAt(int col, int row) =>
            IsInBounds(col, row) ? _grid[col, row].Type : FruitType.None;

        // ── Mutations ─────────────────────────────────────────

        public void Clear(int col, int row) =>
            _grid[col, row] = default;

        public void PlaceFruit(int col, int row, FruitType type) =>
            _grid[col, row] = new CellData { Type = type };

        public void PlaceSpecial(int col, int row, BoosterType booster,
                                 BoosterDirection dir = BoosterDirection.None,
                                 FruitType vortexColor = FruitType.None)
        {
            var type = booster switch
            {
                BoosterType.Rocket => FruitType.Rocket,
                BoosterType.Bomb   => FruitType.Bomb,
                BoosterType.Vortex => FruitType.Vortex,
                _                  => FruitType.None,
            };
            _grid[col, row] = new CellData
            {
                Type       = type,
                Booster    = booster,
                Dir        = dir,
                VortexColor = vortexColor,
            };
        }

        // ── Seeding ───────────────────────────────────────────

        /// <summary>
        /// Fill the board with random regular fruits.
        /// Avoids placing 3-in-a-row to prevent instant matches.
        /// </summary>
        public void Seed(FruitType[] availableTypes, int seed = -1)
        {
            var rng = seed >= 0 ? new System.Random(seed) : new System.Random();

            for (int c = 0; c < Cols; c++)
            {
                for (int r = 0; r < Rows; r++)
                {
                    FruitType chosen;
                    int attempts = 0;
                    do
                    {
                        chosen = availableTypes[rng.Next(availableTypes.Length)];
                        attempts++;
                    }
                    while (attempts < 10 && WouldMakeThreeInLine(c, r, chosen));

                    _grid[c, r] = new CellData { Type = chosen };
                }
            }
        }

        private bool WouldMakeThreeInLine(int col, int row, FruitType t)
        {
            // check horizontal
            if (col >= 2 && TypeAt(col-1,row) == t && TypeAt(col-2,row) == t) return true;
            // check vertical
            if (row >= 2 && TypeAt(col,row-1) == t && TypeAt(col,row-2) == t) return true;
            return false;
        }

        // ── Clone (for undo / preview) ────────────────────────

        public BoardState Clone()
        {
            var copy = new BoardState(Cols, Rows);
            Array.Copy(_grid, copy._grid, _grid.Length);
            return copy;
        }
    }
}
