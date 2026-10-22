// ============================================================
//  FruitData.cs — Citrus City
//  ScriptableObject: visual properties for one fruit type.
//  Create one asset per FruitType in the editor.
// ============================================================
using UnityEngine;

namespace CitrusCity.Fruit
{
    [CreateAssetMenu(fileName = "FruitData_", menuName = "CitrusCity/Fruit Data")]
    public class FruitData : ScriptableObject
    {
        [Header("Identity")]
        public FruitType fruitType;

        [Header("Visuals")]
        public Color primaryColor   = Color.white;
        public Color glowColor      = Color.white;
        public Sprite sprite;               // optional; if null a colored circle is used
        public Sprite shadowSprite;         // optional drop shadow sprite

        [Header("Special Item")]
        public Sprite rocketSprite;         // shown when this fruit becomes a Rocket
        public Sprite bombSprite;
        public Sprite vortexSprite;
    }

    // ── Runtime lookup ────────────────────────────────────────

    /// <summary>
    /// Loaded once at startup; provides O(1) lookup by FruitType.
    /// Place all FruitData assets in Resources/FruitData/.
    /// Automatically populates with textured sprites from Resources as fallback.
    /// </summary>
    public static class FruitDataRegistry
    {
        private static System.Collections.Generic.Dictionary<FruitType, FruitData> _map;

        public static void Initialize()
        {
            _map = new();
            var assets = Resources.LoadAll<FruitData>("FruitData");
            foreach (var fd in assets)
            {
                if (fd != null) _map[fd.fruitType] = fd;
            }

            // Ensure all fruits and boosters have valid FruitData and sprites
            EnsureFruit(FruitType.Orange, "Fruit_Orange", new Color(1f, 0.55f, 0.1f), new Color(1f, 0.75f, 0.3f));
            EnsureFruit(FruitType.Lemon,  "Fruit_Lemon",  new Color(1f, 0.93f, 0.2f), new Color(1f, 1f, 0.5f));
            EnsureFruit(FruitType.Lime,   "Fruit_Lime",   new Color(0.3f, 0.85f, 0.2f), new Color(0.5f, 1f, 0.4f));
            EnsureFruit(FruitType.Cherry, "Fruit_Cherry", new Color(0.9f, 0.1f, 0.15f), new Color(1f, 0.4f, 0.4f));
            EnsureFruit(FruitType.Grape,  "Fruit_Grape",  new Color(0.55f, 0.1f, 0.85f), new Color(0.8f, 0.4f, 1f));
            EnsureFruit(FruitType.Rocket, "Booster_Rocket", new Color(0.2f, 0.6f, 1f), new Color(0.5f, 0.8f, 1f), isBooster: true);
            EnsureFruit(FruitType.Bomb,   "Booster_Bomb",   new Color(0.3f, 0.3f, 0.3f), new Color(0.8f, 0.5f, 0.2f), isBooster: true);
            EnsureFruit(FruitType.Vortex, "Booster_Vortex", new Color(1f, 0.4f, 0.9f), new Color(1f, 0.8f, 1f), isBooster: true);
        }

        private static void EnsureFruit(FruitType t, string spriteName, Color primary, Color glow, bool isBooster = false)
        {
            if (_map.TryGetValue(t, out var existing) && existing != null && existing.sprite != null)
                return;

            var fd = ScriptableObject.CreateInstance<FruitData>();
            fd.fruitType = t;
            fd.primaryColor = primary;
            fd.glowColor = glow;
            string path = isBooster ? $"Sprites/Boosters/{spriteName}" : $"Sprites/Fruits/{spriteName}";
            fd.sprite = Resources.Load<Sprite>(path);

            fd.rocketSprite = Resources.Load<Sprite>("Sprites/Boosters/Booster_Rocket");
            fd.bombSprite   = Resources.Load<Sprite>("Sprites/Boosters/Booster_Bomb");
            fd.vortexSprite = Resources.Load<Sprite>("Sprites/Boosters/Booster_Vortex");

            _map[t] = fd;
        }

        public static FruitData Get(FruitType t)
        {
            if (_map == null) Initialize();
            return _map.TryGetValue(t, out var d) ? d : null;
        }

        public static Color GetColor(FruitType t)
        {
            var d = Get(t);
            if (d != null) return d.primaryColor;
            return t switch
            {
                FruitType.Orange => new Color(1f,  0.55f, 0.1f),
                FruitType.Lemon  => new Color(1f,  0.93f, 0.2f),
                FruitType.Lime   => new Color(0.3f,0.85f, 0.2f),
                FruitType.Cherry => new Color(0.9f,0.1f,  0.15f),
                FruitType.Grape  => new Color(0.55f,0.1f, 0.85f),
                FruitType.Rocket => new Color(0.2f,0.6f,  1f),
                FruitType.Bomb   => new Color(0.3f,0.3f,  0.3f),
                FruitType.Vortex => new Color(1f,  0.4f,  0.9f),
                _                => Color.grey,
            };
        }
    }
}
