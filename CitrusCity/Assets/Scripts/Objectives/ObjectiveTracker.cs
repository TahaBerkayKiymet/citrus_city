// ============================================================
//  ObjectiveTracker.cs — Citrus City
//  Pure C#: tracks progress toward level objectives.
//  Fires events on progress and on completion.
// ============================================================
using System.Collections.Generic;
using CitrusCity.Fruit;
using CitrusCity.Core;

namespace CitrusCity.Objectives
{
    public class ObjectiveTracker
    {
        private readonly Dictionary<FruitType, int> _remaining = new();
        private int _moveCount;
        private int _movesUsed;

        public int MovesRemaining  => _moveCount - _movesUsed;
        public bool IsCompleted    => AllObjectivesMet;
        public bool MovesExhausted => MovesRemaining <= 0;

        public bool AllObjectivesMet
        {
            get
            {
                foreach (var v in _remaining.Values)
                    if (v > 0) return false;
                return true;
            }
        }

        // ── Setup ─────────────────────────────────────────────

        public void Initialize(List<Levels.ObjectiveItem> objectives, int moveCount)
        {
            _remaining.Clear();
            foreach (var obj in objectives)
                _remaining[obj.fruitType] = obj.count;
            _moveCount = moveCount;
            _movesUsed = 0;
        }

        public Dictionary<FruitType, int> GetRemaining() =>
            new Dictionary<FruitType, int>(_remaining);

        public int GetRemaining(FruitType t) =>
            _remaining.TryGetValue(t, out var v) ? v : 0;

        // ── Mutations ─────────────────────────────────────────

        public void RecordRemoval(FruitType type, int count = 1)
        {
            if (!_remaining.ContainsKey(type)) return;
            int prev = _remaining[type];
            _remaining[type] = System.Math.Max(0, prev - count);
            EventBus<ObjectiveProgressEvent>.Publish(
                new ObjectiveProgressEvent(type, _remaining[type]));
        }

        public void RecordRemovals(List<(int col, int row)> cells, Board.BoardState state)
        {
            // Note: state has already been mutated; we pass the original types separately
        }

        /// <summary>Called each time the player confirms a move.</summary>
        public void ConsumeMove()
        {
            _movesUsed++;
            EventBus<MoveConsumedEvent>.Publish(new MoveConsumedEvent(MovesRemaining));

            if (AllObjectivesMet)
                EventBus<LevelVictoryEvent>.Publish(new LevelVictoryEvent());
            else if (MovesExhausted)
                EventBus<LevelFailureEvent>.Publish(new LevelFailureEvent());
        }
    }
}
