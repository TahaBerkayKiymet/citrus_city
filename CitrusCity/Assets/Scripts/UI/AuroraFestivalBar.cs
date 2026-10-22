// ============================================================
//  AuroraFestivalBar.cs — Citrus City
//  UI Component: Displays the "Aurora Festivali" Sunlight Surge
//  charge meter described in the design document (Page 4 & 8).
//  Charges up as player matches fruit; at 8/8 fires Sunlight Surge!
// ============================================================
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using CitrusCity.Core;

namespace CitrusCity.UI
{
    public class AuroraFestivalBar : MonoBehaviour
    {
        [Header("UI Elements")]
        [SerializeField] private Image sunIcon;
        [SerializeField] private TMP_Text statusLabel;
        [SerializeField] private Slider chargeSlider;
        [SerializeField] private List<Image> segmentImages = new();

        [Header("Tuning")]
        [SerializeField] private int maxCharge = 8;
        private int _currentCharge = 0;

        private void Awake()
        {
            if (chargeSlider == null) chargeSlider = GetComponentInChildren<Slider>();
            if (statusLabel == null) statusLabel = GetComponentInChildren<TMP_Text>();
        }

        private void OnEnable()
        {
            EventBus<GroupConfirmedEvent>.Subscribe(OnGroupConfirmed);
            UpdateVisuals();
        }

        private void OnDisable()
        {
            EventBus<GroupConfirmedEvent>.Unsubscribe(OnGroupConfirmed);
        }

        private void OnGroupConfirmed(GroupConfirmedEvent e)
        {
            // Charge increments with each match group
            AddCharge(1);
        }

        public void AddCharge(int amount)
        {
            _currentCharge = Mathf.Clamp(_currentCharge + amount, 0, maxCharge);
            UpdateVisuals();

            if (_currentCharge >= maxCharge)
            {
                TriggerSunlightSurge();
            }
        }

        private void TriggerSunlightSurge()
        {
            Debug.Log("[AuroraFestival] SUNLIGHT SURGE READY! Firing at 4 critical targets!");
            if (statusLabel != null)
                statusLabel.text = "SUNLIGHT SURGE ACTIVE! ☀️";
        }

        private void UpdateVisuals()
        {
            if (chargeSlider != null)
            {
                chargeSlider.maxValue = maxCharge;
                chargeSlider.value = _currentCharge;
            }

            if (statusLabel != null)
            {
                statusLabel.text = _currentCharge >= maxCharge 
                    ? "SUNLIGHT SURGE READY: 8/8"
                    : $"AURORA FESTİVALİ: {_currentCharge}/{maxCharge}";
            }

            // Update segment lights if present
            for (int i = 0; i < segmentImages.Count; i++)
            {
                if (segmentImages[i] != null)
                {
                    segmentImages[i].color = i < _currentCharge 
                        ? new Color(0.9f, 0.4f, 1f, 1f)  // Glowing purple/magenta
                        : new Color(0.2f, 0.2f, 0.35f, 0.6f);
                }
            }
        }
    }
}
