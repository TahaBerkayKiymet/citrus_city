// ============================================================
//  LevelData.cs — Citrus City
//  ScriptableObject defining a single level's configuration.
//  Create one asset per level in ScriptableObjects/Levels/.
// ============================================================
using System.Collections.Generic;
using UnityEngine;
using CitrusCity.Fruit;

namespace CitrusCity.Levels
{
    [System.Serializable]
    public struct ObjectiveItem
    {
        public FruitType fruitType;
        public int count;
    }

    [CreateAssetMenu(fileName = "Level_", menuName = "CitrusCity/Level Data")]
    public class LevelData : ScriptableObject
    {
        [Header("Board")]
        public int cols             = Core.Constants.DefaultCols;
        public int rows             = Core.Constants.DefaultRows;
        public int seed             = 0;    // 0 = random each run; >0 = deterministic

        [Header("Fruit Types Available")]
        public FruitType[] availableFruits = {
            FruitType.Orange, FruitType.Lemon, FruitType.Lime,
            FruitType.Cherry, FruitType.Grape
        };

        [Header("Objectives")]
        public List<ObjectiveItem> objectives = new();
        public int moveCount        = 30;

        [Header("Meta")]
        public string levelTitle    = "Level 1";
        [TextArea] public string levelDescription = "";
    }
}
