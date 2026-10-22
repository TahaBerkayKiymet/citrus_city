// ============================================================
//  MainMenuUI.cs — Citrus City
//  MonoBehaviour: main menu Play button → starts the game.
// ============================================================
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace CitrusCity.UI
{
    public class MainMenuUI : MonoBehaviour
    {
        [SerializeField] private Button   playButton;
        [SerializeField] private TMP_Text titleLabel;
        [SerializeField] private TMP_Text taglineLabel;
        [SerializeField] private TMP_Text levelLabel;

        private void Start()
        {
            playButton?.onClick.AddListener(OnPlay);

            if (titleLabel   != null) titleLabel.text   = "CITRUS CITY";
            if (taglineLabel != null) taglineLabel.text = "The Puzzle with Direction";

            int lvl = PlayerPrefs.GetInt(Core.Constants.PrefCurrentLevel, 0) + 1;
            if (levelLabel != null) levelLabel.text = $"Level {lvl}";
        }

        private void OnPlay()
        {
            Core.GameManager.Instance?.StartGame();
        }
    }
}
