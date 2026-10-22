// ============================================================
//  BoosterCombo.cs — Citrus City
//  Pure C#: detects and resolves booster-on-booster combos.
//  Implemented combos (5 of 10 from the GDD):
//    1. Rocket + Rocket  → row AND column (plus shape)
//    2. Rocket + Bomb    → 5×5 blast (radius 2)
//    3. Bomb  + Bomb     → 5×5 blast  
//    4. Bomb  + Vortex   → all of Vortex's target color → Bombs → explode
//    5. Vortex + Vortex  → clear entire board
// ============================================================
using System.Collections.Generic;
using CitrusCity.Fruit;

namespace CitrusCity.Boosters
{
    public readonly struct ComboResult
    {
        public readonly List<(int col, int row)> Cleared;
        public readonly bool WasCombo;
        public ComboResult(List<(int, int)> cleared, bool wasCombo)
        { Cleared = cleared; WasCombo = wasCombo; }
    }

    public static class BoosterCombo
    {
        /// <summary>
        /// Check if tapping (col, row) would trigger a combo with an adjacent booster.
        /// <paramref name="adjacentCol"/> / <paramref name="adjacentRow"/> are the coords
        /// of the other booster. Returns ComboResult with WasCombo=false if no combo.
        /// </summary>
        public static ComboResult TryCombo(
            Board.BoardState state,
            int col, int row,
            BoosterType typeA, BoosterDirection dirA, FruitType vortexColorA,
            int adjCol, int adjRow,
            BoosterType typeB, BoosterDirection dirB, FruitType vortexColorB)
        {
            // Normalise order
            var (a, b) = Normalise(typeA, typeB);

            // Remove both boosters first
            state.Clear(col, row);
            state.Clear(adjCol, adjRow);

            // Combo logic
            List<(int, int)> cleared;
            bool wasCombo = true;

            if (a == BoosterType.Vortex && b == BoosterType.Vortex)
            {
                // Combo 5: clear everything
                cleared = BoosterEffect.ClearAll(state);
            }
            else if (a == BoosterType.Bomb && b == BoosterType.Vortex)
            {
                // Combo 4: all of vortex's target color → Bomb → explode
                var targetColor = vortexColorA != FruitType.None ? vortexColorA : vortexColorB;
                cleared = new List<(int, int)>();
                // Find all cells of target color, explode each as a mini-bomb
                var targets = new List<(int, int)>();
                for (int c = 0; c < state.Cols; c++)
                    for (int r = 0; r < state.Rows; r++)
                        if (state.TypeAt(c, r) == targetColor) targets.Add((c, r));
                foreach (var (tc, tr) in targets)
                    cleared.AddRange(BoosterEffect.ApplyBomb(state, tc, tr, 1));
            }
            else if (a == BoosterType.Rocket && b == BoosterType.Bomb)
            {
                // Combo 2: 5×5 blast centred on col, row
                cleared = BoosterEffect.ApplyBomb(state, col, row, 2);
            }
            else if (a == BoosterType.Bomb && b == BoosterType.Bomb)
            {
                // Combo 3: 5×5 blast
                cleared = BoosterEffect.ApplyBomb(state, col, row, 2);
            }
            else if (a == BoosterType.Rocket && b == BoosterType.Rocket)
            {
                // Combo 1: entire row AND column (plus shape)
                cleared = BoosterEffect.ApplyRocket(state, BoosterDirection.None, col, row);
            }
            else
            {
                wasCombo = false;
                cleared  = new List<(int, int)>();
            }

            return new ComboResult(cleared, wasCombo);
        }

        /// <summary>Find if any neighbour of (col, row) is a booster.</summary>
        public static bool FindAdjacentBooster(
            Board.BoardState state, int col, int row,
            out int adjCol, out int adjRow, out BoosterType adjType)
        {
            (int dc, int dr)[] dirs = { (0,1),(0,-1),(1,0),(-1,0) };
            foreach (var (dc, dr) in dirs)
            {
                int nc = col + dc, nr = row + dr;
                if (!state.IsInBounds(nc, nr)) continue;
                var cell = state.Get(nc, nr);
                if (cell.Booster != BoosterType.None)
                {
                    adjCol = nc; adjRow = nr; adjType = cell.Booster;
                    return true;
                }
            }
            adjCol = -1; adjRow = -1; adjType = BoosterType.None;
            return false;
        }

        private static (BoosterType a, BoosterType b) Normalise(BoosterType a, BoosterType b)
        {
            // Ensure a <= b for symmetric matching
            return a <= b ? (a, b) : (b, a);
        }
    }
}
