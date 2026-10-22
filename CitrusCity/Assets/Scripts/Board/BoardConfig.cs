// ============================================================
//  BoardConfig.cs — Citrus City
//  ScriptableObject: global board presentation settings.
// ============================================================
using UnityEngine;

namespace CitrusCity.Board
{
    [CreateAssetMenu(fileName = "BoardConfig", menuName = "CitrusCity/Board Config")]
    public class BoardConfig : ScriptableObject
    {
        [Header("Layout")]
        public float cellSize    = Core.Constants.CellSize;
        public float cellSpacing = Core.Constants.CellSpacing;

        [Header("Background")]
        public Color boardBackground = new Color(0.13f, 0.08f, 0.22f, 1f);
        public Color cellEvenColor   = new Color(1f, 1f, 1f, 0.05f);
        public Color cellOddColor    = new Color(1f, 1f, 1f, 0.03f);

        [Header("Physics Feel")]
        public float fallSpeed      = Core.Constants.FruitFallSpeed;
        public float fallStagger    = Core.Constants.FruitFallDelay;
    }
}
