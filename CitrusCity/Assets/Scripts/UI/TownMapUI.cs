// ============================================================
//  TownMapUI.cs — Citrus City
//  MonoBehaviour: 6-District Town Map Screen (PDF Page 5).
//  "Şehir neden terk edildi? - 6 bölge, her bölge 150 seviye"
// ============================================================
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using CitrusCity.Core;

namespace CitrusCity.UI
{
    public class TownMapUI : MonoBehaviour
    {
        [Header("Navigation")]
        [SerializeField] private Button backButton;
        [SerializeField] private Button playNextButton;

        [Header("Dialogue Box")]
        [SerializeField] private TMP_Text dialogueText;

        [Header("Districts (1..6)")]
        [SerializeField] private Button district1Btn; // Limonata Kafe (Active)
        [SerializeField] private Button district2Btn; // Marina (Lansman)
        [SerializeField] private Button district3Btn; // Zeytinlik (+6 hafta)
        [SerializeField] private Button district4Btn; // Çarşı (+12 hafta)
        [SerializeField] private Button district5Btn; // Fener (+18 hafta)
        [SerializeField] private Button district6Btn; // Eski Konak (+24 hafta)

        private void Start()
        {
            backButton?.onClick.AddListener(OnBack);
            playNextButton?.onClick.AddListener(OnPlayNext);

            district1Btn?.onClick.AddListener(() => ShowDialogue("Limonata Kafe: Kafeyi yeniden açacağım. Ama bu kasabada herkes bir şey saklıyor..."));
            district2Btn?.onClick.AddListener(() => ShowDialogue("Marina: Eski balıkçı tekneleri terk edilmiş... Marina temizlenince yeni ipuçları çıkacak."));
            district3Btn?.onClick.AddListener(() => ShowDialogue("Zeytinlik: 6 hafta sonra açılacak. Asırlık ağaçlar gizemli fısıltılar taşıyor."));

            ShowDialogue("Kafeyi yeniden açacağım. Ama bu kasabada herkes bir şey saklıyor...");
        }

        private void ShowDialogue(string msg)
        {
            if (dialogueText != null)
                dialogueText.text = $"\"{msg}\"";
        }

        private void OnBack()
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
        }

        private void OnPlayNext()
        {
            GameManager.Instance?.StartGame();
        }
    }
}
