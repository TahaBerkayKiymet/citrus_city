// ============================================================
//  GameManager.cs — Citrus City
//  Top-level state machine: Menu → Playing → Victory → Failure
//  Persists current level index in PlayerPrefs.
// ============================================================
using UnityEngine;
using UnityEngine.SceneManagement;
using CitrusCity.Core;

namespace CitrusCity.Core
{
    public enum GameState { Menu, Playing, Victory, Failure }

    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }

        [Header("Settings")]
        [SerializeField] private int totalDemoLevels = 3;

        public GameState CurrentState { get; private set; } = GameState.Menu;
        public int CurrentLevelIndex  { get; private set; } = 0;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            DontDestroyOnLoad(gameObject);

            CurrentLevelIndex = PlayerPrefs.GetInt(Constants.PrefCurrentLevel, 0);
        }

        private void OnEnable()
        {
            EventBus<LevelVictoryEvent>.Subscribe(OnVictory);
            EventBus<LevelFailureEvent>.Subscribe(OnFailure);
            EventBus<LevelRestartEvent>.Subscribe(OnRestart);
        }

        private void OnDisable()
        {
            EventBus<LevelVictoryEvent>.Unsubscribe(OnVictory);
            EventBus<LevelFailureEvent>.Unsubscribe(OnFailure);
            EventBus<LevelRestartEvent>.Unsubscribe(OnRestart);
        }

        // ── Public API ────────────────────────────────────────

        public void StartGame()
        {
            CurrentState = GameState.Playing;
            SceneManager.LoadScene(Constants.GameScene);
        }

        public void GoToMainMenu()
        {
            CurrentState = GameState.Menu;
            SceneManager.LoadScene(Constants.MainMenuScene);
        }

        // ── Event handlers ────────────────────────────────────

        private void OnVictory(LevelVictoryEvent _)
        {
            CurrentState = GameState.Victory;
            // Advance to next level (cycle through demo levels)
            CurrentLevelIndex = (CurrentLevelIndex + 1) % totalDemoLevels;
            PlayerPrefs.SetInt(Constants.PrefCurrentLevel, CurrentLevelIndex);
            PlayerPrefs.Save();
        }

        private void OnFailure(LevelFailureEvent _)
        {
            CurrentState = GameState.Failure;
        }

        private void OnRestart(LevelRestartEvent _)
        {
            CurrentState = GameState.Playing;
            SceneManager.LoadScene(Constants.GameScene);
        }
    }
}
