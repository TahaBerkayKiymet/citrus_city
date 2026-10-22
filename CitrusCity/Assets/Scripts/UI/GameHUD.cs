// ============================================================
//  GameHUD.cs — Citrus City
//  MonoBehaviour: move counter, header bar.
//  Subscribes to MoveConsumedEvent.
// ============================================================
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using CitrusCity.Core;

namespace CitrusCity.UI
{
    public class GameHUD : MonoBehaviour
    {
        [Header("Moves")]
        [SerializeField] private TMP_Text movesLabel;
        [SerializeField] private Image    movesIcon;

        [Header("Level")]
        [SerializeField] private TMP_Text levelLabel;

        private int _maxMoves;

        private void OnEnable()
        {
            EventBus<MoveConsumedEvent>.Subscribe(OnMovesChanged);
        }

        private void OnDisable()
        {
            EventBus<MoveConsumedEvent>.Unsubscribe(OnMovesChanged);
        }

        // ── Public API ────────────────────────────────────────

        public void SetMoves(int count)
        {
            _maxMoves = count;
            UpdateMovesDisplay(count);
        }

        public void SetLevelTitle(string title)
        {
            if (levelLabel != null) levelLabel.text = title;
        }

        // ── Event handlers ────────────────────────────────────

        private void OnMovesChanged(MoveConsumedEvent e)
        {
            UpdateMovesDisplay(e.MovesRemaining);
        }

        // ── Display ───────────────────────────────────────────

        private void UpdateMovesDisplay(int remaining)
        {
            if (movesLabel == null) return;
            movesLabel.text = remaining.ToString();

            // Turn red when low
            if (remaining <= 5)
                movesLabel.color = new Color(1f, 0.3f, 0.3f);
            else if (remaining <= 10)
                movesLabel.color = new Color(1f, 0.75f, 0.2f);
            else
                movesLabel.color = Color.white;
        }
    }
}
