// ============================================================
//  SceneBootstrap.cs — Citrus City
//  Editor utility: creates the complete GameScene hierarchy
//  programmatically so developers don't have to build it by hand.
//
//  Usage: CitrusCity → Setup → Create GameScene
//
//  This is NOT needed at runtime — it's Editor-only tooling.
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

        [MenuItem("CitrusCity/Setup/Create GameScene Objects")]
        public static void CreateGameScene()
        {
            // ── Camera ────────────────────────────────────────
            var camGo = new GameObject("Main Camera");
            var cam   = camGo.AddComponent<Camera>();
            cam.orthographic = true;
            cam.orthographicSize = 6f;
            cam.clearFlags = CameraClearFlags.SolidColor;
            cam.backgroundColor = new Color(0.13f, 0.08f, 0.22f);
            cam.transform.position = new Vector3(0, 0, -10f);
            camGo.tag = "MainCamera";

            // ── GameManager (DontDestroyOnLoad) ───────────────
            var gmGo = new GameObject("GameManager");
            gmGo.AddComponent<GameManager>();

            // ── Pool ──────────────────────────────────────────
            var poolGo = new GameObject("FruitViewPool");
            var pool   = poolGo.AddComponent<Fruit.FruitViewPool>();

            // ── Board ─────────────────────────────────────────
            var boardGo = new GameObject("Board");
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
            canvasGo.AddComponent<CanvasScaler>().uiScaleMode =
                CanvasScaler.ScaleMode.ScaleWithScreenSize;
            canvasGo.GetComponent<CanvasScaler>().referenceResolution = new Vector2(1080, 1920);
            canvasGo.AddComponent<GraphicRaycaster>();

            // ─── HUD panel ────────────────────────────────────
            var hudGo = CreatePanel(canvasGo.transform, "HUD", new Vector2(0, 1),
                new Vector2(0, 1), new Vector2(0, -120), new Vector2(1080, 120));
            var hud = hudGo.AddComponent<GameHUD>();

            var movesLabel = CreateLabel(hudGo.transform, "MovesLabel", "30",
                                         new Vector2(540, -60), 72, Color.white);

            // ─── Objective panel ──────────────────────────────
            var objGo = CreatePanel(canvasGo.transform, "Objectives",
                new Vector2(0, 1), new Vector2(0, 1),
                new Vector2(0, -230), new Vector2(1080, 100));
            var objView = objGo.AddComponent<ObjectiveView>();

            // ─── Victory Panel ────────────────────────────────
            var victoryGo = CreateFullPanel(canvasGo.transform, "VictoryPanel",
                new Color(0.1f, 0.7f, 0.3f, 0.95f));
            var victory = victoryGo.AddComponent<VictoryPanel>();
            var victoryTitle = CreateLabel(victoryGo.transform, "Title",
                "🎉 Level Complete!", new Vector2(540, 1100), 60, Color.white);
            var nextBtn = CreateButton(victoryGo.transform, "NextLevelButton",
                "Next Level", new Vector2(540, 860), new Vector2(400, 100));
            var menuBtn1 = CreateButton(victoryGo.transform, "MenuButton",
                "Main Menu", new Vector2(540, 740), new Vector2(400, 80));

            // ─── Failure Panel ────────────────────────────────
            var failureGo = CreateFullPanel(canvasGo.transform, "FailurePanel",
                new Color(0.7f, 0.1f, 0.1f, 0.95f));
            var failure = failureGo.AddComponent<FailurePanel>();
            var failureTitle = CreateLabel(failureGo.transform, "Title",
                "So Close! 🍋", new Vector2(540, 1100), 60, Color.white);
            var restartBtn = CreateButton(failureGo.transform, "RestartButton",
                "Try Again", new Vector2(540, 860), new Vector2(400, 100));
            var menuBtn2 = CreateButton(failureGo.transform, "MenuButton",
                "Main Menu", new Vector2(540, 740), new Vector2(400, 80));

            // ── Level Loader ──────────────────────────────────
            var loaderGo = new GameObject("LevelLoader");
            var loader = loaderGo.AddComponent<LevelLoader>();
            SetPrivateField(loader, "boardController",   bc);
            SetPrivateField(loader, "selectionController", sel);
            SetPrivateField(loader, "hud",              hud);
            SetPrivateField(loader, "objectiveView",    objView);

            var loadedLevels = Resources.LoadAll<LevelData>("Levels");
            if (loadedLevels != null && loadedLevels.Length > 0)
            {
                SetPrivateField(loader, "levels", loadedLevels);
            }

            Debug.Log("[SceneBootstrap] GameScene objects created and wired successfully!");
            Selection.activeGameObject = loaderGo;
        }

        // ── MainMenu ──────────────────────────────────────────

        [MenuItem("CitrusCity/Setup/Create MainMenu Objects")]
        public static void CreateMainMenu()
        {
            var camGo = new GameObject("Main Camera");
            var cam   = camGo.AddComponent<Camera>();
            cam.clearFlags = CameraClearFlags.SolidColor;
            cam.backgroundColor = new Color(0.13f, 0.08f, 0.22f);
            camGo.tag = "MainCamera";

            var gmGo = new GameObject("GameManager");
            gmGo.AddComponent<GameManager>();

            var canvasGo = new GameObject("Canvas");
            var canvas   = canvasGo.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvasGo.AddComponent<CanvasScaler>().uiScaleMode =
                CanvasScaler.ScaleMode.ScaleWithScreenSize;
            canvasGo.GetComponent<CanvasScaler>().referenceResolution = new Vector2(1080, 1920);
            canvasGo.AddComponent<GraphicRaycaster>();

            CreateLabel(canvasGo.transform, "Title", "CITRUS CITY",
                        new Vector2(540, 1200), 96, new Color(1f, 0.85f, 0.2f));
            CreateLabel(canvasGo.transform, "Tagline", "The Puzzle with Direction",
                        new Vector2(540, 1080), 36, Color.white);
            var playBtn = CreateButton(canvasGo.transform, "PlayButton",
                "PLAY", new Vector2(540, 820), new Vector2(500, 130));

            var menuGo = new GameObject("MainMenuUI");
            var menu   = menuGo.AddComponent<MainMenuUI>();
            SetPrivateField(menu, "playButton", playBtn);

            Debug.Log("[SceneBootstrap] MainMenu objects created.");
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

        private static TMP_Text CreateLabel(Transform parent, string name, string text,
            Vector2 pos, float fontSize, Color color)
        {
            var go = new GameObject(name);
            go.transform.SetParent(parent, false);
            var rt = go.AddComponent<RectTransform>();
            rt.anchoredPosition = pos - new Vector2(540, 960); // centre offset
            rt.sizeDelta = new Vector2(900, 120);
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
            rt.anchoredPosition = pos - new Vector2(540, 960);
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
            tmp.fontSize = 42;
            tmp.color = Color.white;
            tmp.alignment = TextAlignmentOptions.Center;
            tmp.fontStyle = FontStyles.Bold;

            return btn;
        }

        // ── Reflection helper for SerializeField assignment ───

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
