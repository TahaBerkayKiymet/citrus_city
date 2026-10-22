// ============================================================
//  GravitySystem.cs — Citrus City
//  Pure C# gravity: collapses columns after cells are cleared.
//  Returns a list of (col, fromRow, toRow) movements so the
//  view layer can animate each fruit dropping.
// ============================================================
using System.Collections.Generic;
using CitrusCity.Fruit;

namespace CitrusCity.Board
{
    public readonly struct FruitFallMove
    {
        public readonly int Col;
        public readonly int FromRow;
        public readonly int ToRow;
        public FruitFallMove(int col, int from, int to) { Col = col; FromRow = from; ToRow = to; }
    }

    public static class GravitySystem
    {
        /// <summary>
        /// Collapses the board downward (row 0 = bottom).
        /// Mutates <paramref name="state"/> in place.
        /// Returns a list of moves so views can animate.
        /// </summary>
        public static List<FruitFallMove> ApplyGravity(BoardState state)
        {
            var moves = new List<FruitFallMove>();

            for (int c = 0; c < state.Cols; c++)
            {
                int writeRow = 0;
                for (int r = 0; r < state.Rows; r++)
                {
                    if (!state.IsEmpty(c, r))
                    {
                        if (writeRow != r)
                        {
                            moves.Add(new FruitFallMove(c, r, writeRow));
                            state.Set(c, writeRow, state.Get(c, r));
                            state.Clear(c, r);
                        }
                        writeRow++;
                    }
                }
            }

            return moves;
        }

        /// <summary>
        /// Fill empty cells at the top of each column with new random fruits.
        /// Returns the new cells so they can be animated (fall in from above).
        /// </summary>
        public static List<(int col, int row, FruitType type)> Refill(
            BoardState state, FruitType[] availableTypes, System.Random rng)
        {
            var newCells = new List<(int, int, FruitType)>();
            for (int c = 0; c < state.Cols; c++)
            {
                for (int r = 0; r < state.Rows; r++)
                {
                    if (state.IsEmpty(c, r))
                    {
                        var t = availableTypes[rng.Next(availableTypes.Length)];
                        state.PlaceFruit(c, r, t);
                        newCells.Add((c, r, t));
                    }
                }
            }
            return newCells;
        }
    }
}
