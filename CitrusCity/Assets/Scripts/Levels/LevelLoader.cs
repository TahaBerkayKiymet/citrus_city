// ============================================================
//  LevelLoader.cs — Citrus City
//  MonoBehaviour: initialises board + tracker from LevelData.
//  Place on the GameScene root. Reads level index from
//  GameManager and loads the appropriate LevelData asset.
// ============================================================
using UnityEngine;
using CitrusCity.Board;
using CitrusCity.Objectives;
using CitrusCity.Selection;

namespace CitrusCity.Levels
{
    public class LevelLoader : MonoBehaviour
    {
        [Header("Level Assets")]
        [SerializeField] private LevelData[] levels;   // assign 3 assets in inspector

        [Header("Scene References")]
        [SerializeField] private BoardController boardController;
        [SerializeField] private SelectionController selectionController;
        [SerializeField] private UI.GameHUD hud;
        [SerializeField] private UI.ObjectiveView objectiveView;

        private ObjectiveTracker _tracker;

        private void Start()
        {
            int idx = Core.GameManager.Instance != null
                      ? Core.GameManager.Instance.CurrentLevelIndex
                      : 0;

            if (levels == null || levels.Length == 0)
            {
                levels = Resources.LoadAll<LevelData>("Levels");
            }

            if (levels == null || levels.Length == 0)
            {
                Debug.LogWarning("[LevelLoader] No levels assigned in Inspector or found in Resources/Levels. Using fallback.");
                LoadFallback();
                return;
            }

            idx = idx % levels.Length;
            var data = levels[idx];

            _tracker = new ObjectiveTracker();
            _tracker.Initialize(data.objectives, data.moveCount);

            boardController.Initialize(data, _tracker);
            selectionController.SetTracker(_tracker);

            hud?.SetMoves(data.moveCount);
            objectiveView?.Initialize(data.objectives);
        }

        // ── Fallback (no assets assigned) ────────────────────

        private void LoadFallback()
        {
            var data = ScriptableObject.CreateInstance<LevelData>();
            data.cols       = Core.Constants.DefaultCols;
            data.rows       = Core.Constants.DefaultRows;
            data.moveCount  = 30;
            data.seed       = 42;
            data.availableFruits = new[]
            {
                Fruit.FruitType.Orange, Fruit.FruitType.Lemon,
                Fruit.FruitType.Lime,   Fruit.FruitType.Cherry,
                Fruit.FruitType.Grape
            };
            data.objectives = new System.Collections.Generic.List<ObjectiveItem>
            {
                new ObjectiveItem { fruitType = Fruit.FruitType.Orange, count = 20 },
                new ObjectiveItem { fruitType = Fruit.FruitType.Lemon,  count = 15 },
            };

            _tracker = new ObjectiveTracker();
            _tracker.Initialize(data.objectives, data.moveCount);

            boardController.Initialize(data, _tracker);
            selectionController.SetTracker(_tracker);

            hud?.SetMoves(data.moveCount);
            objectiveView?.Initialize(data.objectives);
        }
    }
}
