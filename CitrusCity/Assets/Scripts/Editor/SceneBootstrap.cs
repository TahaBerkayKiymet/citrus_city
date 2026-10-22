// ============================================================
//  SceneBootstrap.cs — Citrus City
//  Editor utility: creates the complete GameScene, MainMenu,
//  and TownMapScene hierarchies matching the Citrus City GDD.
//
//  Usage: CitrusCity → Setup → [Choose Scene]
// ============================================================
#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using TMPro;
using CitrusCity.Board;
using CitrusCity.Selection;
using CitrusCity.Boosters;
using CitrusCity.Effects;
using CitrusCity.UI;
using CitrusCity.Levels;
using CitrusCity.Core;

namespace CitrusCity.Editor
{
    public static class SceneBootstrap
    {
        // ── GameScene ─────────────────────────────────────────

        [MenuItem("CitrusCity/Setup/Create Complete GameScene (PDF Design)")]
        public static void CreateGameScene()
        {
            // ── Camera ────────────────────────────────────────
            var camGo = new GameObject("Main Camera");
            var cam   = camGo.AddComponent<Camera>();
            cam.orthographic = true;
            cam.orthographicSize = 6f;
            cam.clearFlags = CameraClearFlags.SolidColor;
            cam.backgroundColor = new Color(0.12f, 0.10f, 0.20f);
            cam.transform.position = new Vector3(0, 0, -10f);
            camGo.tag = "MainCamera";

            // ── Background Sprite (Mediterranean Terrace) ─────
            var bgGo = new GameObject("TerraceBackground");
            var bgSr = bgGo.AddComponent<SpriteRenderer>();
            bgSr.sprite = Resources.Load<Sprite>("Sprites/Backgrounds/Bg_Game_Terrace");
            bgSr.sortingLayerName = "Background";
            bgSr.sortingOrder = -10;
            bgGo.transform.position = new Vector3(0, 0, 1f);
            bgGo.transform.localScale = Vector3.one * 1.05f;

            // ── GameManager (DontDestroyOnLoad) ───────────────
            var gmGo = new GameObject("GameManager");
            gmGo.AddComponent<GameManager>();

            // ── Pool ──────────────────────────────────────────
            var poolGo = new GameObject("FruitViewPool");
            var pool   = poolGo.AddComponent<Fruit.FruitViewPool>();

            // ── Board ─────────────────────────────────────────
            var boardGo = new GameObject("Board");
            boardGo.transform.position = new Vector3(0, -0.3f, 0);
            var bc = boardGo.AddComponent<BoardController>();

            // ── Preview ───────────────────────────────────────
            var previewGo = new GameObject("BoosterPreview");
            var preview   = previewGo.AddComponent<BoosterPreviewView>();
            previewGo.SetActive(false);

            // ── Selection ─────────────────────────────────────
            var selGo = new GameObject("SelectionController");
            var sel   = selGo.AddComponent<SelectionController>();
            SetPrivateField(sel, "board", bc);
            SetPrivateField(sel, "preview", preview);

            // ── Effects ───────────────────────────────────────
            var fxGo  = new GameObject("FX");
            fxGo.AddComponent<FruitExplosionFX>();
            fxGo.AddComponent<BoosterActivateFX>();

            // ── Canvas ────────────────────────────────────────
            var canvasGo = new GameObject("Canvas");
            var canvas = canvasGo.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            var scaler = canvasGo.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1080, 1920);
            canvasGo.AddComponent<GraphicRaycaster>();

            // ── Top HUD ───────────────────────────────────────
            var hudGo = CreatePanel(canvasGo.transform, "TopHUD", new Vector2(0.05f, 0.88f),
                new Vector2(0.95f, 0.98f), Vector2.zero, Vector2.zero);
            var hudImg = hudGo.GetComponent<Image>();
            hudImg.color = new Color(0.12f, 0.16f, 0.28f, 0.95f);
            var hud = hudGo.AddComponent<GameHUD>();

            // Avatar in window (Top Left)
            var avatarGo = CreateImage(hudGo.transform, "AvatarWindow",
                Resources.Load<Sprite>("Sprites/UI/UI_Avatar_Window"),
                new Vector2(70, -60), new Vector2(100, 100));

            // Objective container
            var objGo = CreatePanel(hudGo.transform, "Objectives", new Vector2(0.2f, 0.2f),
                new Vector2(0.7f, 0.85f), Vector2.zero, Vector2.zero);
            objGo.GetComponent<Image>().color = Color.clear;
            var objView = objGo.AddComponent<ObjectiveView>();

            // Moves Counter (Top Right)
            var movesBox = CreatePanel(hudGo.transform, "MovesBox", new Vector2(0.75f, 0.1f),
                new Vector2(0.98f, 0.9f), Vector2.zero, Vector2.zero);
            movesBox.GetComponent<Image>().color = new Color(0.18f, 0.22f, 0.38f, 0.9f);
            CreateLabel(movesBox.transform, "MovesTitle", "HAMLE", new Vector2(0, 25), 24, new Color(0.7f, 0.8f, 1f));
            var movesLabel = CreateLabel(movesBox.transform, "MovesLabel", "30", new Vector2(0, -15), 48, Color.white);
            SetPrivateField(hud, "movesCountLabel", movesLabel);

            // ── Aurora Festival Bar (Just under Top HUD) ──────
            var auroraGo = CreatePanel(canvasGo.transform, "AuroraFestivalBar",
                new Vector2(0.15f, 0.82f), new Vector2(0.85f, 0.87f), Vector2.zero, Vector2.zero);
            auroraGo.GetComponent<Image>().color = new Color(0.12f, 0.16f, 0.3f, 0.95f);
            var auroraBar = auroraGo.AddComponent<AuroraFestivalBar>();

            var auroraSun = CreateImage(auroraGo.transform, "SunIcon",
                Resources.Load<Sprite>("Sprites/UI/Event_Solar_Streak"),
                new Vector2(-320, 0), new Vector2(70, 70));
            var auroraText = CreateLabel(auroraGo.transform, "StatusText",
                "AURORA FESTİVALİ • SUNLIGHT SURGE", new Vector2(40, 0), 26, new Color(1f, 0.9f, 0.4f));

            // ── Bottom Booster Bar ────────────────────────────
            var boosterBarGo = CreatePanel(canvasGo.transform, "BoosterBar",
                new Vector2(0.05f, 0.02f), new Vector2(0.95f, 0.11f), Vector2.zero, Vector2.zero);
            boosterBarGo.GetComponent<Image>().color = new Color(0.15f, 0.22f, 0.38f, 0.95f);

            CreateLabel(boosterBarGo.transform, "BoosterTitle", "BOOSTER BAR",
                new Vector2(0, 65), 22, new Color(0.8f, 0.9f, 1f));

            // 4 In-game boosters (Hammer, Bat, Basket, Mixer)
            CreateBoosterButton(boosterBarGo.transform, "Hammer", "Çekiç", "InGame_Booster_Hammer", new Vector2(-330, 0));
            CreateBoosterButton(boosterBarGo.transform, "Bat",    "Sopa",  "InGame_Booster_Bat",    new Vector2(-110, 0));
            CreateBoosterButton(boosterBarGo.transform, "Basket", "Sepet", "InGame_Booster_Basket", new Vector2(110, 0));
            CreateBoosterButton(boosterBarGo.transform, "Mixer",  "Mikser","InGame_Booster_Mixer",  new Vector2(330, 0));

            // ── Near-Miss / Failure Popup (PDF Page 10) ───────
            var failPopupGo = CreateFullPanel(canvasGo.transform, "FailurePanel", new Color(0.05f, 0.05f, 0.1f, 0.85f));
            var failPanel = failPopupGo.AddComponent<FailurePanel>();

            var failCard = CreatePanel(failPopupGo.transform, "Card", new Vector2(0.1f, 0.25f),
                new Vector2(0.9f, 0.75f), Vector2.zero, Vector2.zero);
            failCard.GetComponent<Image>().color = Color.white;

            CreateLabel(failCard.transform, "Title", "NEREDEYSE BİTİYORDU!", new Vector2(0, 360), 46, new Color(0.9f, 0.2f, 0.2f));
            CreateLabel(failCard.transform, "Subtitle", "SEVİYE 47 • HARD", new Vector2(0, 300), 28, new Color(0.3f, 0.3f, 0.4f));

            var extraBtn = CreateButton(failCard.transform, "ExtraMovesButton", "+3 HAMLE  🟡 900",
                new Vector2(0, 50), new Vector2(580, 110));
            extraBtn.GetComponent<Image>().color = new Color(1f, 0.75f, 0.1f);

            var streakWarningBox = CreatePanel(failCard.transform, "StreakWarning", new Vector2(0.08f, 0.2f),
                new Vector2(0.92f, 0.36f), Vector2.zero, Vector2.zero);
            streakWarningBox.GetComponent<Image>().color = new Color(1f, 0.92f, 0.92f, 1f);
            CreateLabel(streakWarningBox.transform, "WarningText",
                "Kaybedersen Solar Streak sıfırlanır\n(8 galibiyetlik serin aktif)",
                Vector2.zero, 24, new Color(0.85f, 0.2f, 0.2f));

            var declineBtn = CreateButton(failCard.transform, "DeclineButton", "Hayır, teşekkürler",
                new Vector2(0, -320), new Vector2(400, 70));
            declineBtn.GetComponent<Image>().color = Color.clear;
            declineBtn.GetComponentInChildren<TMP_Text>().color = new Color(0.4f, 0.5f, 0.65f);

            SetPrivateField(failPanel, "titleLabel", failCard.transform.Find("Title")?.GetComponent<TMP_Text>());
            SetPrivateField(failPanel, "subtitleLabel", failCard.transform.Find("Subtitle")?.GetComponent<TMP_Text>());
            SetPrivateField(failPanel, "extraMovesButton", extraBtn);
            SetPrivateField(failPanel, "declineButton", declineBtn);

            // ── Victory Panel ─────────────────────────────────
            var victoryGo = CreateFullPanel(canvasGo.transform, "VictoryPanel", new Color(0.05f, 0.05f, 0.1f, 0.85f));
            var victory = victoryGo.AddComponent<VictoryPanel>();
            var vCard = CreatePanel(victoryGo.transform, "Card", new Vector2(0.12f, 0.28f),
                new Vector2(0.88f, 0.72f), Vector2.zero, Vector2.zero);
            vCard.GetComponent<Image>().color = new Color(0.15f, 0.2f, 0.32f, 0.98f);
            CreateLabel(vCard.transform, "Title", "🎉 BÖLÜM TAMAMLANDI!", new Vector2(0, 260), 46, Color.white);
            CreateLabel(vCard.transform, "Stars", "⭐⭐⭐", new Vector2(0, 160), 72, Color.white);
            var nextBtn = CreateButton(vCard.transform, "NextLevelButton", "Sonraki Bölüm ▶", new Vector2(0, -50), new Vector2(480, 100));
            var menuBtn = CreateButton(vCard.transform, "MenuButton", "Kasabaya Dön 🏛️", new Vector2(0, -170), new Vector2(480, 85));

            // ── Level Loader ──────────────────────────────────
            var loaderGo = new GameObject("LevelLoader");
            var loader = loaderGo.AddComponent<LevelLoader>();
            SetPrivateField(loader, "boardController",   bc);
            SetPrivateField(loader, "selectionController", sel);
            SetPrivateField(loader, "hud",              hud);
            SetPrivateField(loader, "objectiveView",    objView);

            var loadedLevels = Resources.LoadAll<LevelData>("Levels");
            if (loadedLevels != null && loadedLevels.Length > 0)
                SetPrivateField(loader, "levels", loadedLevels);

            Debug.Log("[SceneBootstrap] Complete GameScene (PDF Design) created successfully!");
            Selection.activeGameObject = loaderGo;
        }

        // ── MainMenu ──────────────────────────────────────────

        [MenuItem("CitrusCity/Setup/Create Complete MainMenu (PDF Design)")]
        public static void CreateMainMenu()
        {
            var camGo = new GameObject("Main Camera");
            var cam   = camGo.AddComponent<Camera>();
            cam.clearFlags = CameraClearFlags.SolidColor;
            cam.backgroundColor = new Color(0.15f, 0.12f, 0.25f);
            camGo.tag = "MainCamera";

            var gmGo = new GameObject("GameManager");
            gmGo.AddComponent<GameManager>();

            var canvasGo = new GameObject("Canvas");
            var canvas   = canvasGo.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            var scaler = canvasGo.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1080, 1920);
            canvasGo.AddComponent<GraphicRaycaster>();

            // Full Background (Mediterranean Town & Sea)
            var bgGo = CreateImage(canvasGo.transform, "TownBackground",
                Resources.Load<Sprite>("Sprites/Backgrounds/Bg_Town_MainMenu"),
                Vector2.zero, new Vector2(1080, 1920));
            bgGo.transform.SetAsFirstSibling();

            // Top Bar with Currencies
            var topBar = CreatePanel(canvasGo.transform, "TopBar", new Vector2(0.04f, 0.89f),
                new Vector2(0.96f, 0.98f), Vector2.zero, Vector2.zero);
            topBar.GetComponent<Image>().color = new Color(0.1f, 0.15f, 0.28f, 0.9f);

            // Avatar in window
            CreateImage(topBar.transform, "AvatarWindow",
                Resources.Load<Sprite>("Sprites/UI/UI_Avatar_Window"),
                new Vector2(-420, 0), new Vector2(95, 95));

            CreateLabel(topBar.transform, "Lives", "❤️ ∞", new Vector2(-240, 0), 32, Color.white);
            CreateLabel(topBar.transform, "Gold",  "🟡 2,450", new Vector2(0, 0), 32, new Color(1f, 0.9f, 0.3f));
            CreateLabel(topBar.transform, "Lemon", "🍃 4,500", new Vector2(250, 0), 32, new Color(0.7f, 1f, 0.4f));

            // Left Event Badges
            CreateEventBadge(canvasGo.transform, "ChestBadge", "Event_Chest", new Vector2(-420, 320), "Sandık");
            CreateEventBadge(canvasGo.transform, "SolarStreak", "Event_Solar_Streak", new Vector2(-420, 150), "8/8 Aktif");
            CreateEventBadge(canvasGo.transform, "MagicWand", "Event_Magic_Wand", new Vector2(-420, -20), "Tılsım");

            // Right Event Badges
            CreateEventBadge(canvasGo.transform, "WebShop", "Event_Web_Shop", new Vector2(420, 320), "%10 Bonus");
            CreateEventBadge(canvasGo.transform, "Festival", "Event_Festival", new Vector2(420, 150), "Festival");

            // Center Renovation Pin
            var pinBtn = CreateButton(canvasGo.transform, "RenovatePin", "🔨 Onar",
                new Vector2(0, -80), new Vector2(160, 65));
            pinBtn.GetComponent<Image>().color = new Color(0.2f, 0.6f, 1f);

            // Big Green Play Button
            var playBtn = CreateButton(canvasGo.transform, "PlayButton", "▶ BÖLÜM 47 (ZOR)",
                new Vector2(0, -620), new Vector2(650, 130));
            playBtn.GetComponent<Image>().color = new Color(0.2f, 0.8f, 0.2f);
            playBtn.GetComponentInChildren<TMP_Text>().fontSize = 44;

            // Bottom Navigation Bar
            var navBar = CreatePanel(canvasGo.transform, "NavBar", new Vector2(0, 0),
                new Vector2(1, 0.08f), Vector2.zero, Vector2.zero);
            navBar.GetComponent<Image>().color = new Color(0.12f, 0.16f, 0.28f, 0.98f);

            var townMapBtn = CreateButton(navBar.transform, "TownMapBtn", "🏛️ Kasaba Haritası",
                new Vector2(0, 0), new Vector2(320, 75));
            townMapBtn.GetComponent<Image>().color = new Color(0.25f, 0.35f, 0.55f);

            var menuGo = new GameObject("MainMenuUI");
            var menu   = menuGo.AddComponent<MainMenuUI>();
            SetPrivateField(menu, "playButton", playBtn);
            SetPrivateField(menu, "townMapButton", townMapBtn);
            SetPrivateField(menu, "fountainRenovatePin", pinBtn);

            Debug.Log("[SceneBootstrap] Complete MainMenu (PDF Design) created successfully!");
        }

        // ── TownMapScene ──────────────────────────────────────

        [MenuItem("CitrusCity/Setup/Create TownMap Scene (PDF Design)")]
        public static void CreateTownMapScene()
        {
            var camGo = new GameObject("Main Camera");
            var cam   = camGo.AddComponent<Camera>();
            cam.clearFlags = CameraClearFlags.SolidColor;
            cam.backgroundColor = new Color(0.12f, 0.15f, 0.26f);
            camGo.tag = "MainCamera";

            var canvasGo = new GameObject("Canvas");
            var canvas   = canvasGo.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            var scaler = canvasGo.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1080, 1920);
            canvasGo.AddComponent<GraphicRaycaster>();

            // Background
            var bgGo = CreateImage(canvasGo.transform, "MapBackground",
                Resources.Load<Sprite>("Sprites/Backgrounds/Bg_Town_Map"),
                Vector2.zero, new Vector2(1080, 1920));
            bgGo.transform.SetAsFirstSibling();

            CreateLabel(canvasGo.transform, "Title", "Şehir neden terk edildi?",
                new Vector2(0, 750), 52, Color.white);
            CreateLabel(canvasGo.transform, "Subtitle", "6 bölge • her bölge 150 seviye",
                new Vector2(0, 680), 30, new Color(0.8f, 0.9f, 1f));

            // Back button
            var backBtn = CreateButton(canvasGo.transform, "BackButton", "◀ Geri",
                new Vector2(-420, 840), new Vector2(160, 65));
            backBtn.GetComponent<Image>().color = new Color(0.3f, 0.4f, 0.6f);

            // Dialogue Box at bottom
            var dialogueBox = CreatePanel(canvasGo.transform, "DialogueBox", new Vector2(0.08f, 0.05f),
                new Vector2(0.92f, 0.18f), Vector2.zero, Vector2.zero);
            dialogueBox.GetComponent<Image>().color = new Color(0.12f, 0.18f, 0.32f, 0.95f);

            var avatar = CreateImage(dialogueBox.transform, "Avatar",
                Resources.Load<Sprite>("Sprites/UI/UI_Avatar_Window"),
                new Vector2(-350, 0), new Vector2(100, 100));

            var dialogueLabel = CreateLabel(dialogueBox.transform, "DialogueText",
                "\"Kafeyi yeniden açacağım. Ama bu kasabada herkes bir şey saklıyor...\"",
                new Vector2(80, 0), 28, Color.white);

            var mapUiGo = new GameObject("TownMapUI");
            var mapUi = mapUiGo.AddComponent<TownMapUI>();
            SetPrivateField(mapUi, "backButton", backBtn);
            SetPrivateField(mapUi, "dialogueText", dialogueLabel);

            Debug.Log("[SceneBootstrap] Complete TownMap Scene (PDF Design) created successfully!");
        }

        // ── UI Helpers ────────────────────────────────────────

        private static GameObject CreatePanel(Transform parent, string name,
            Vector2 anchorMin, Vector2 anchorMax, Vector2 anchoredPos, Vector2 size)
        {
            var go = new GameObject(name);
            go.transform.SetParent(parent, false);
            var rt = go.AddComponent<RectTransform>();
            rt.anchorMin = anchorMin;
            rt.anchorMax = anchorMax;
            rt.anchoredPosition = anchoredPos;
            rt.sizeDelta = size;
            var img = go.AddComponent<Image>();
            img.color = new Color(0, 0, 0, 0.5f);
            return go;
        }

        private static GameObject CreateFullPanel(Transform parent, string name, Color color)
        {
            var go = new GameObject(name);
            go.transform.SetParent(parent, false);
            var rt = go.AddComponent<RectTransform>();
            rt.anchorMin = Vector2.zero;
            rt.anchorMax = Vector2.one;
            rt.offsetMin = Vector2.zero;
            rt.offsetMax = Vector2.zero;
            var img = go.AddComponent<Image>();
            img.color = color;
            go.SetActive(false);
            return go;
        }

        private static GameObject CreateImage(Transform parent, string name, Sprite sprite,
            Vector2 pos, Vector2 size)
        {
            var go = new GameObject(name);
            go.transform.SetParent(parent, false);
            var rt = go.AddComponent<RectTransform>();
            rt.anchoredPosition = pos;
            rt.sizeDelta = size;
            var img = go.AddComponent<Image>();
            img.sprite = sprite;
            img.color = Color.white;
            return go;
        }

        private static TMP_Text CreateLabel(Transform parent, string name, string text,
            Vector2 pos, float fontSize, Color color)
        {
            var go = new GameObject(name);
            go.transform.SetParent(parent, false);
            var rt = go.AddComponent<RectTransform>();
            rt.anchoredPosition = pos;
            rt.sizeDelta = new Vector2(750, 100);
            var tmp = go.AddComponent<TextMeshProUGUI>();
            tmp.text = text;
            tmp.fontSize = fontSize;
            tmp.color = color;
            tmp.alignment = TextAlignmentOptions.Center;
            return tmp;
        }

        private static Button CreateButton(Transform parent, string name, string label,
            Vector2 pos, Vector2 size)
        {
            var go = new GameObject(name);
            go.transform.SetParent(parent, false);
            var rt = go.AddComponent<RectTransform>();
            rt.anchoredPosition = pos;
            rt.sizeDelta = size;
            var img = go.AddComponent<Image>();
            img.color = new Color(1f, 0.6f, 0.1f);
            var btn = go.AddComponent<Button>();

            var labelGo = new GameObject("Label");
            labelGo.transform.SetParent(go.transform, false);
            var lrt = labelGo.AddComponent<RectTransform>();
            lrt.anchorMin = Vector2.zero;
            lrt.anchorMax = Vector2.one;
            lrt.offsetMin = Vector2.zero;
            lrt.offsetMax = Vector2.zero;
            var tmp = labelGo.AddComponent<TextMeshProUGUI>();
            tmp.text = label;
            tmp.fontSize = 36;
            tmp.color = Color.white;
            tmp.alignment = TextAlignmentOptions.Center;
            tmp.fontStyle = FontStyles.Bold;

            return btn;
        }

        private static void CreateEventBadge(Transform parent, string name, string spriteName, Vector2 pos, string label)
        {
            var go = new GameObject(name);
            go.transform.SetParent(parent, false);
            var rt = go.AddComponent<RectTransform>();
            rt.anchoredPosition = pos;
            rt.sizeDelta = new Vector2(130, 130);
            var img = go.AddComponent<Image>();
            img.sprite = Resources.Load<Sprite>($"Sprites/UI/{spriteName}");
            go.AddComponent<Button>();

            var lGo = new GameObject("Text");
            lGo.transform.SetParent(go.transform, false);
            var lrt = lGo.AddComponent<RectTransform>();
            lrt.anchoredPosition = new Vector2(0, -65);
            lrt.sizeDelta = new Vector2(150, 40);
            var tmp = lGo.AddComponent<TextMeshProUGUI>();
            tmp.text = label;
            tmp.fontSize = 20;
            tmp.color = Color.white;
            tmp.alignment = TextAlignmentOptions.Center;
        }

        private static void CreateBoosterButton(Transform parent, string name, string label, string spriteName, Vector2 pos)
        {
            var btnGo = new GameObject(name);
            btnGo.transform.SetParent(parent, false);
            var rt = btnGo.AddComponent<RectTransform>();
            rt.anchoredPosition = pos;
            rt.sizeDelta = new Vector2(100, 100);
            var img = btnGo.AddComponent<Image>();
            img.sprite = Resources.Load<Sprite>($"Sprites/Boosters/{spriteName}");
            btnGo.AddComponent<Button>();

            var lGo = new GameObject("Label");
            lGo.transform.SetParent(btnGo.transform, false);
            var lrt = lGo.AddComponent<RectTransform>();
            lrt.anchoredPosition = new Vector2(0, -60);
            lrt.sizeDelta = new Vector2(110, 30);
            var tmp = lGo.AddComponent<TextMeshProUGUI>();
            tmp.text = label;
            tmp.fontSize = 20;
            tmp.color = Color.white;
            tmp.alignment = TextAlignmentOptions.Center;
        }

        private static void SetPrivateField(object obj, string fieldName, object value)
        {
            var field = obj.GetType().GetField(fieldName,
                System.Reflection.BindingFlags.NonPublic |
                System.Reflection.BindingFlags.Instance);
            field?.SetValue(obj, value);
        }
    }
}
#endif
