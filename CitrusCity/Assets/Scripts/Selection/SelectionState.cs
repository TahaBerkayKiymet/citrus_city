// ============================================================
//  SelectionState.cs — Citrus City
//  Pure C#: tracks the currently selected group, touch position,
//  computed booster direction, and whether the move is valid.
// ============================================================
using System.Collections.Generic;
using UnityEngine;
using CitrusCity.Fruit;
using CitrusCity.Boosters;

namespace CitrusCity.Selection
{
    /// <summary>
    /// Describes which edge of the group's bounding box the touch
    /// originated from. Used to determine booster direction.
    ///
    /// Algorithm (see implementation plan for rationale):
    ///   - Normalise touch position within the group bounding box → [0,1]×[0,1]
    ///   - If within EdgeThreshold of left/right wall → Horizontal
    ///   - If within EdgeThreshold of top/bottom wall → Vertical
    ///   - If ambiguous (centre zone), use whichever dimension is wider
    /// </summary>
    public class SelectionState
    {
        public List<(int col, int row)> Group  { get; private set; } = new();
        public FruitType FruitType             { get; private set; } = FruitType.None;
        public BoosterType PendingBooster      { get; private set; } = BoosterType.None;
        public BoosterDirection Direction      { get; private set; } = BoosterDirection.None;
        public int TouchCol                    { get; private set; }
        public int TouchRow                    { get; private set; }
        public bool IsValid                    => Group.Count >= Core.Constants.MinGroupSize;
        public bool HasSelection               => Group.Count > 0;

        // ── Update from controller ────────────────────────────

        public void SetGroup(List<(int col, int row)> group, FruitType type)
        {
            Group     = group;
            FruitType = type;
            PendingBooster = BoosterFactory.GetBoosterForGroupSize(group.Count);
        }

        public void SetTouchCell(int col, int row)
        {
            TouchCol = col;
            TouchRow = row;
            Direction = ComputeDirection();
        }

        public void Clear()
        {
            Group.Clear();
            FruitType      = FruitType.None;
            PendingBooster = BoosterType.None;
            Direction      = BoosterDirection.None;
        }

        // ── Direction computation ─────────────────────────────

        private BoosterDirection ComputeDirection()
        {
            if (Group.Count == 0) return BoosterDirection.None;
            if (PendingBooster == BoosterType.None) return BoosterDirection.None;
            if (PendingBooster == BoosterType.Bomb || PendingBooster == BoosterType.Vortex)
                return BoosterDirection.None; // bombs/vortex are directionless

            var (minC, minR, maxC, maxR) = GroupFinder.GetBounds(Group);

            int spanC = maxC - minC;        // 0 if single column
            int spanR = maxR - minR;

            // Normalise touch within bounding box
            float normC = spanC > 0 ? (float)(TouchCol - minC) / spanC : 0.5f;
            float normR = spanR > 0 ? (float)(TouchRow - minR) / spanR : 0.5f;

            float edge = Core.Constants.EdgeThreshold;

            bool nearHEdge = normC <= edge || normC >= (1f - edge);
            bool nearVEdge = normR <= edge || normR >= (1f - edge);

            if (nearHEdge && nearVEdge)
            {
                // corner: prefer the orientation in which the group is longer
                return spanC >= spanR ? BoosterDirection.Horizontal : BoosterDirection.Vertical;
            }
            if (nearHEdge) return BoosterDirection.Horizontal;
            if (nearVEdge) return BoosterDirection.Vertical;

            // Centre zone: prefer longer axis
            return spanC >= spanR ? BoosterDirection.Horizontal : BoosterDirection.Vertical;
        }
    }
}
