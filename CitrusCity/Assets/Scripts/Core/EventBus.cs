// ============================================================
//  EventBus.cs — Citrus City
//  Simple typed event system. Keeps systems decoupled.
//  Usage:
//    EventBus<FruitRemovedEvent>.Subscribe(OnFruitRemoved);
//    EventBus<FruitRemovedEvent>.Publish(new FruitRemovedEvent(...));
//    EventBus<FruitRemovedEvent>.Unsubscribe(OnFruitRemoved);
// ============================================================
using System;
using System.Collections.Generic;

namespace CitrusCity.Core
{
    public static class EventBus<T>
    {
        private static readonly List<Action<T>> _listeners = new();

        public static void Subscribe(Action<T> listener)
        {
            if (!_listeners.Contains(listener))
                _listeners.Add(listener);
        }

        public static void Unsubscribe(Action<T> listener)
        {
            _listeners.Remove(listener);
        }

        public static void Publish(T evt)
        {
            // iterate over a copy to allow un-subscription inside handlers
            var copy = new List<Action<T>>(_listeners);
            foreach (var l in copy)
                l?.Invoke(evt);
        }

        public static void Clear() => _listeners.Clear();
    }

    // ── Event payloads ───────────────────────────────────────

    public readonly struct FruitRemovedEvent
    {
        public readonly int Col;
        public readonly int Row;
        public readonly Fruit.FruitType Type;
        public FruitRemovedEvent(int col, int row, Fruit.FruitType type)
        { Col = col; Row = row; Type = type; }
    }

    public readonly struct GroupConfirmedEvent
    {
        public readonly System.Collections.Generic.List<(int col, int row)> Cells;
        public readonly Fruit.FruitType FruitType;
        public readonly Boosters.BoosterType GeneratedBooster;
        public readonly int TouchCol;
        public readonly int TouchRow;
        public readonly Boosters.BoosterDirection Direction;
        public GroupConfirmedEvent(
            System.Collections.Generic.List<(int, int)> cells,
            Fruit.FruitType fruitType,
            Boosters.BoosterType booster,
            int touchCol, int touchRow,
            Boosters.BoosterDirection dir)
        {
            Cells = cells; FruitType = fruitType;
            GeneratedBooster = booster;
            TouchCol = touchCol; TouchRow = touchRow;
            Direction = dir;
        }
    }

    public readonly struct MoveConsumedEvent
    {
        public readonly int MovesRemaining;
        public MoveConsumedEvent(int remaining) { MovesRemaining = remaining; }
    }

    public readonly struct ObjectiveProgressEvent
    {
        public readonly Fruit.FruitType Type;
        public readonly int CountRemaining;
        public ObjectiveProgressEvent(Fruit.FruitType type, int remaining)
        { Type = type; CountRemaining = remaining; }
    }

    public readonly struct LevelVictoryEvent { }
    public readonly struct LevelFailureEvent { }
    public readonly struct LevelRestartEvent { }

    public readonly struct BoosterActivatedEvent
    {
        public readonly int Col;
        public readonly int Row;
        public readonly Boosters.BoosterType Type;
        public readonly Boosters.BoosterDirection Direction;
        public BoosterActivatedEvent(int col, int row, Boosters.BoosterType t, Boosters.BoosterDirection d)
        { Col = col; Row = row; Type = t; Direction = d; }
    }

    public readonly struct BoardRefreshedEvent { }
}
