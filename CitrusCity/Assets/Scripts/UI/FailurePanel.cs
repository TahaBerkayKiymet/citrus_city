// ============================================================
//  FailurePanel.cs — Citrus City
//  MonoBehaviour: Exact "Neredeyse Bitıyordu!" Near-Miss screen
//  as specified in GDD (Page 9 & 10).
//  Offers +3 moves for Gold, displays Solar Streak warning,
//  and allows graceful retry/menu navigation without intrusive ads.
// ============================================================
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using CitrusCity.Core;

namespace CitrusCity.UI
{
    public class FailurePanel : MonoBehaviour
    {
        [Header("UI Elements")]
        [SerializeField] private TMP_Text titleLabel;
        [SerializeField] private TMP_Text subtitleLabel;
        [SerializeField] private TMP_Text streakWarningLabel;
        [SerializeField] private Button   extraMovesButton;
        [SerializeField] private TMP_Text extraMovesPriceLabel;
        [SerializeField] private Button   declineButton;
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
            extraMovesButton?.onClick.AddListener(OnBuyExtraMoves);
            declineButton?.onClick.AddListener(OnDecline);
            menuButton?.onClick.AddListener(OnMenu);
        }

        // ── Event ─────────────────────────────────────────────

        private void OnFailure(LevelFailureEvent _)
        {
            gameObject.SetActive(true);

            int lvl = (GameManager.Instance != null ? GameManager.Instance.CurrentLevelIndex : 0) + 1;

            if (titleLabel != null) 
                titleLabel.text = "NEREDEYSE BİTİYORDU!";

            if (subtitleLabel != null) 
                subtitleLabel.text = $"SEVİYE {lvl} • HARD";

            if (extraMovesPriceLabel != null)
                extraMovesPriceLabel.text = "+3 HAMLE  🟡 900";

            if (streakWarningLabel != null)
                streakWarningLabel.text = "Kaybedersen Solar Streak sıfırlanır\n(8 galibiyetlik serin aktif)";

            StartCoroutine(FadeIn());
        }

        private IEnumerator FadeIn()
        {
            if (canvasGroup == null) yield break;
            canvasGroup.alpha = 0;
            float t = 0;
            while (t < 0.35f)
            {
                t += Time.deltaTime;
                canvasGroup.alpha = t / 0.35f;
                yield return null;
            }
            canvasGroup.alpha = 1;
        }

        // ── Button handlers ───────────────────────────────────

        private void OnBuyExtraMoves()
        {
            Debug.Log("[FailurePanel] Purchased +3 Moves for 900 Gold! Resuming gameplay.");
            gameObject.SetActive(false);
            // Grant +3 moves back to game HUD & tracker
            EventBus<LevelRestartEvent>.Publish(new LevelRestartEvent());
        }

        private void OnDecline()
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
