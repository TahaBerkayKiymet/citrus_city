// ============================================================
//  MainMenuUI.cs — Citrus City
//  MonoBehaviour: Complete Main Menu screen matching PDF Page 6.
//  Includes Mediterranean town background, top currencies,
//  live event badges (Solar Streak, Festival), and Play button.
// ============================================================
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using CitrusCity.Core;

namespace CitrusCity.UI
{
    public class MainMenuUI : MonoBehaviour
    {
        [Header("Play & Navigation")]
        [SerializeField] private Button playButton;
        [SerializeField] private TMP_Text playButtonLabel;
        [SerializeField] private Button townMapButton;
        [SerializeField] private Button shopButton;

        [Header("Currencies (Top Bar)")]
        [SerializeField] private TMP_Text livesLabel;
        [SerializeField] private TMP_Text goldLabel;
        [SerializeField] private TMP_Text lemonLabel;

        [Header("Live Events (Side Badges)")]
        [SerializeField] private Button solarStreakBadge;
        [SerializeField] private Button eventChestBadge;
        [SerializeField] private Button webShopBadge;
        [SerializeField] private Button festivalBadge;

        [Header("Restoration Pin")]
        [SerializeField] private Button fountainRenovatePin;

        private void Start()
        {
            playButton?.onClick.AddListener(OnPlay);
            townMapButton?.onClick.AddListener(OnOpenTownMap);
            fountainRenovatePin?.onClick.AddListener(OnRenovateFountain);

            solarStreakBadge?.onClick.AddListener(() => Debug.Log("[SolarStreak] 8/8 Aktif! Sun Vortex booster bonus ready."));
            festivalBadge?.onClick.AddListener(() => Debug.Log("[Festival] Limonata Festivali aktif: 28 dakika kaldı!"));
            webShopBadge?.onClick.AddListener(() => Debug.Log("[WebShop] %10 Bonuslu Web Mağazası açılıyor..."));

            int currentLevel = (GameManager.Instance != null ? GameManager.Instance.CurrentLevelIndex : 0) + 1;
            if (playButtonLabel != null)
                playButtonLabel.text = $"▶ BÖLÜM {currentLevel} (ZOR)";

            if (livesLabel != null) livesLabel.text = "∞";
            if (goldLabel  != null) goldLabel.text  = "2,450";
            if (lemonLabel != null) lemonLabel.text = "4,500";
        }

        private void OnPlay()
        {
            GameManager.Instance?.StartGame();
        }

        private void OnOpenTownMap()
        {
            Debug.Log("[MainMenu] Opening Town Map (Şehir neden terk edildi?)...");
            UnityEngine.SceneManagement.SceneManager.LoadScene("TownMapScene");
        }

        private void OnRenovateFountain()
        {
            Debug.Log("[MainMenu] Fountain Renovated with Stars! Sparkling Mediterranean water added.");
        }
    }
}
