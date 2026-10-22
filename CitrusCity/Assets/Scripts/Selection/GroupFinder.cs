// ============================================================
//  GroupFinder.cs — Citrus City
//  Pure C#: BFS flood-fill to find connected same-color groups.
// ============================================================
using System.Collections.Generic;
using CitrusCity.Fruit;

namespace CitrusCity.Selection
{
    public static class GroupFinder
    {
        private static readonly (int dc, int dr)[] Neighbours =
        {
            ( 0,  1),   // up
            ( 0, -1),   // down
            (-1,  0),   // left
            ( 1,  0),   // right
        };

        /// <summary>
        /// Returns all cells reachable from (startCol, startRow) that share the
        /// same FruitType (must be a regular fruit, not a special item).
        /// Returns an empty list if the starting cell is empty or special.
        /// </summary>
        public static List<(int col, int row)> FindGroup(
            Board.BoardState state, int startCol, int startRow)
        {
            var result = new List<(int, int)>();
            if (!state.IsInBounds(startCol, startRow)) return result;

            var startType = state.TypeAt(startCol, startRow);
            if (startType == FruitType.None || startType.IsSpecial()) return result;

            var visited = new HashSet<(int, int)>();
            var queue   = new Queue<(int, int)>();

            queue.Enqueue((startCol, startRow));
            visited.Add((startCol, startRow));

            while (queue.Count > 0)
            {
                var (c, r) = queue.Dequeue();
                result.Add((c, r));

                foreach (var (dc, dr) in Neighbours)
                {
                    var nc = c + dc;
                    var nr = r + dr;
                    if (!state.IsInBounds(nc, nr)) continue;
                    if (visited.Contains((nc, nr))) continue;
                    if (state.TypeAt(nc, nr) != startType) continue;

                    visited.Add((nc, nr));
                    queue.Enqueue((nc, nr));
                }
            }

            return result;
        }

        /// <summary>
        /// Computes the axis-aligned bounding rectangle of a cell list.
        /// Returns (minCol, minRow, maxCol, maxRow).
        /// </summary>
        public static (int minC, int minR, int maxC, int maxR) GetBounds(
            List<(int col, int row)> cells)
        {
            int minC = int.MaxValue, minR = int.MaxValue;
            int maxC = int.MinValue, maxR = int.MinValue;
            foreach (var (c, r) in cells)
            {
                if (c < minC) minC = c;
                if (c > maxC) maxC = c;
                if (r < minR) minR = r;
                if (r > maxR) maxR = r;
            }
            return (minC, minR, maxC, maxR);
        }
    }
}
