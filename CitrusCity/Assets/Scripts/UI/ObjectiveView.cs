// ============================================================
//  ObjectiveView.cs — Citrus City
//  MonoBehaviour: displays objective progress icons.
//  Subscribes to ObjectiveProgressEvent and updates counters.
// ============================================================
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using CitrusCity.Core;
using CitrusCity.Fruit;

namespace CitrusCity.UI
{
    [System.Serializable]
    public struct ObjectiveUIItem
    {
        public Image  fruitIcon;
        public TMP_Text countLabel;
        public Image  checkmark;
    }

    public class ObjectiveView : MonoBehaviour
    {
        [SerializeField] private List<ObjectiveUIItem> slots;
        [SerializeField] private GameObject slotPrefab;
        [SerializeField] private Transform  slotContainer;

        private readonly Dictionary<FruitType, (int idx, int total)> _objectives = new();

        private void OnEnable()
        {
            EventBus<ObjectiveProgressEvent>.Subscribe(OnProgress);
        }

        private void OnDisable()
        {
            EventBus<ObjectiveProgressEvent>.Unsubscribe(OnProgress);
        }

        // ── Public API ────────────────────────────────────────

        public void Initialize(List<Levels.ObjectiveItem> objectives)
        {
            _objectives.Clear();

            // Clear existing dynamic slots
            if (slotContainer != null)
                foreach (Transform child in slotContainer)
                    Destroy(child.gameObject);

            for (int i = 0; i < objectives.Count; i++)
            {
                var obj = objectives[i];
                _objectives[obj.fruitType] = (i, obj.count);

                if (i < slots.Count)
                    UpdateSlot(i, obj.count, obj.fruitType, false);
            }
        }

        // ── Event handler ─────────────────────────────────────

        private void OnProgress(ObjectiveProgressEvent e)
        {
            if (!_objectives.TryGetValue(e.Type, out var info)) return;
            UpdateSlot(info.idx, e.CountRemaining, e.Type, e.CountRemaining <= 0);
        }

        private void UpdateSlot(int idx, int remaining, FruitType type, bool completed)
        {
            if (idx >= slots.Count) return;
            var slot = slots[idx];

            if (slot.countLabel != null)
                slot.countLabel.text = remaining > 0 ? remaining.ToString() : "✓";

            if (slot.fruitIcon != null)
            {
                var data = FruitDataRegistry.Get(type);
                if (data != null && data.sprite != null)
                {
                    slot.fruitIcon.sprite = data.sprite;
                    slot.fruitIcon.color = Color.white;
                }
                else
                {
                    slot.fruitIcon.color = FruitDataRegistry.GetColor(type);
                }
            }

            if (slot.checkmark != null)
                slot.checkmark.gameObject.SetActive(completed);
        }
    }
}
