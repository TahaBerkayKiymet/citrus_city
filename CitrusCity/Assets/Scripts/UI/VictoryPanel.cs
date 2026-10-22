// ============================================================
//  VictoryPanel.cs — Citrus City
//  MonoBehaviour: shown when all objectives met.
//  "Nearly there" messaging from the GDD is in FailurePanel.
// ============================================================
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using CitrusCity.Core;

namespace CitrusCity.UI
{
    public class VictoryPanel : MonoBehaviour
    {
        [Header("UI Elements")]
        [SerializeField] private TMP_Text titleLabel;
        [SerializeField] private TMP_Text subtitleLabel;
        [SerializeField] private Button   nextLevelButton;
        [SerializeField] private Button   menuButton;
        [SerializeField] private CanvasGroup canvasGroup;

        private void Awake()
        {
            gameObject.SetActive(false);
        }

        private void OnEnable()
        {
            EventBus<LevelVictoryEvent>.Subscribe(OnVictory);
        }

        private void OnDisable()
        {
            EventBus<LevelVictoryEvent>.Unsubscribe(OnVictory);
        }

        private void Start()
        {
            nextLevelButton?.onClick.AddListener(OnNextLevel);
            menuButton?.onClick.AddListener(OnMenu);
        }

        // ── Event ─────────────────────────────────────────────

        private void OnVictory(LevelVictoryEvent _)
        {
            gameObject.SetActive(true);
            if (titleLabel    != null) titleLabel.text    = "🎉 Level Complete!";
            if (subtitleLabel != null) subtitleLabel.text = "Citrus City grows stronger!";
            StartCoroutine(FadeIn());
        }

        private IEnumerator FadeIn()
        {
            if (canvasGroup == null) yield break;
            canvasGroup.alpha = 0;
            float t = 0;
            while (t < 0.4f)
            {
                t += Time.deltaTime;
                canvasGroup.alpha = t / 0.4f;
                yield return null;
            }
            canvasGroup.alpha = 1;
        }

        // ── Button handlers ───────────────────────────────────

        private void OnNextLevel()
        {
            EventBus<LevelRestartEvent>.Publish(new LevelRestartEvent());
            gameObject.SetActive(false);
        }

        private void OnMenu()
        {
            GameManager.Instance?.GoToMainMenu();
        }
    }
}
