// ============================================================
//  FailurePanel.cs — Citrus City
//  MonoBehaviour: "Almost there!" screen from the GDD.
//  Shows how close the player was; offers restart.
//  No ads shown here (per GDD: "Bu ekranda reklam yok").
// ============================================================
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using CitrusCity.Core;

namespace CitrusCity.UI
{
    public class FailurePanel : MonoBehaviour
    {
        [Header("UI Elements")]
        [SerializeField] private TMP_Text titleLabel;
        [SerializeField] private TMP_Text subtitleLabel;
        [SerializeField] private TMP_Text streakWarningLabel;
        [SerializeField] private Button   restartButton;
        [SerializeField] private Button   menuButton;
        [SerializeField] private CanvasGroup canvasGroup;

        private void Awake()
        {
            gameObject.SetActive(false);
        }

        private void OnEnable()
        {
            EventBus<LevelFailureEvent>.Subscribe(OnFailure);
        }

        private void OnDisable()
        {
            EventBus<LevelFailureEvent>.Unsubscribe(OnFailure);
        }

        private void Start()
        {
            restartButton?.onClick.AddListener(OnRestart);
            menuButton?.onClick.AddListener(OnMenu);
        }

        // ── Event ─────────────────────────────────────────────

        private void OnFailure(LevelFailureEvent _)
        {
            gameObject.SetActive(true);

            if (titleLabel    != null) titleLabel.text    = "So Close! 🍋";
            if (subtitleLabel != null) subtitleLabel.text = "You were almost there — try again!";

            // Streak warning (GDD: remind player losing streak on failure)
            if (streakWarningLabel != null)
                streakWarningLabel.text = "Your winning streak will reset if you leave now.";

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

        private void OnRestart()
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
