// ============================================================
//  BoosterEffect.cs — Citrus City
//  Pure C#: applies one booster's effect to the board state.
//  Returns a list of cells that were cleared so the view can
//  play effects on them.
// ============================================================
using System.Collections.Generic;
using CitrusCity.Fruit;

namespace CitrusCity.Boosters
{
    public static class BoosterEffect
    {
        /// <summary>
        /// Apply a booster at (col, row) with the given direction.
        /// Returns cells cleared so effects can be scheduled.
        /// Mutates <paramref name="state"/> directly.
        /// </summary>
        public static List<(int col, int row)> Apply(
            Board.BoardState state,
            BoosterType type,
            BoosterDirection dir,
            int col, int row,
            FruitType vortexTarget = FruitType.None)
        {
            return type switch
            {
                BoosterType.Rocket => ApplyRocket(state, dir, col, row),
                BoosterType.Bomb   => ApplyBomb(state, col, row, 1),   // 3×3 = radius 1
                BoosterType.Vortex => ApplyVortex(state, vortexTarget),
                _                  => new List<(int, int)>(),
            };
        }

        // ── Rocket ───────────────────────────────────────────

        public static List<(int col, int row)> ApplyRocket(
            Board.BoardState state, BoosterDirection dir, int col, int row)
        {
            var cleared = new List<(int, int)>();

            if (dir == BoosterDirection.Horizontal || dir == BoosterDirection.None)
            {
                // Clear entire row
                for (int c = 0; c < state.Cols; c++)
                    if (!state.IsEmpty(c, row)) { cleared.Add((c, row)); state.Clear(c, row); }
            }

            if (dir == BoosterDirection.Vertical || dir == BoosterDirection.None)
            {
                // Clear entire column
                for (int r = 0; r < state.Rows; r++)
                    if (!state.IsEmpty(col, r)) { cleared.Add((col, r)); state.Clear(col, r); }
            }

            return cleared;
        }

        // ── Bomb ─────────────────────────────────────────────

        public static List<(int col, int row)> ApplyBomb(
            Board.BoardState state, int col, int row, int radius = 1)
        {
            var cleared = new List<(int, int)>();
            for (int dc = -radius; dc <= radius; dc++)
            {
                for (int dr = -radius; dr <= radius; dr++)
                {
                    int nc = col + dc, nr = row + dr;
                    if (state.IsInBounds(nc, nr) && !state.IsEmpty(nc, nr))
                    {
                        cleared.Add((nc, nr));
                        state.Clear(nc, nr);
                    }
                }
            }
            return cleared;
        }

        // ── Vortex ───────────────────────────────────────────

        public static List<(int col, int row)> ApplyVortex(
            Board.BoardState state, FruitType targetColor)
        {
            var cleared = new List<(int, int)>();
            for (int c = 0; c < state.Cols; c++)
            {
                for (int r = 0; r < state.Rows; r++)
                {
                    var t = state.TypeAt(c, r);
                    if (t == targetColor)
                    {
                        cleared.Add((c, r));
                        state.Clear(c, r);
                    }
                }
            }
            return cleared;
        }

        // ── Clear All (Vortex+Vortex combo) ─────────────────

        public static List<(int col, int row)> ClearAll(Board.BoardState state)
        {
            var cleared = new List<(int, int)>();
            for (int c = 0; c < state.Cols; c++)
                for (int r = 0; r < state.Rows; r++)
                    if (!state.IsEmpty(c, r)) { cleared.Add((c, r)); state.Clear(c, r); }
            return cleared;
        }
    }
}
