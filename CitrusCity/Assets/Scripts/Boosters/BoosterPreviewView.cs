// ============================================================
//  BoosterPreviewView.cs — Citrus City
//  MonoBehaviour: renders the directional arrow and booster
//  icon ghost shown while the player holds their finger down.
//  This is the "Magnetic Tap" visual feedback core feature.
// ============================================================
using UnityEngine;
using CitrusCity.Boosters;
using CitrusCity.Fruit;

namespace CitrusCity.Boosters
{
    public class BoosterPreviewView : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private SpriteRenderer boosterIcon;
        [SerializeField] private SpriteRenderer arrowRenderer;
        [SerializeField] private LineRenderer sweepLine;

        [Header("Arrow Sprites")]
        [SerializeField] private Sprite arrowHorizontal;
        [SerializeField] private Sprite arrowVertical;
        [SerializeField] private Sprite rocketIcon;
        [SerializeField] private Sprite bombIcon;
        [SerializeField] private Sprite vortexIcon;

        [Header("Tuning")]
        [SerializeField] private float pulsePeriod = 0.6f;
        [SerializeField] private float arrowOffset  = 0.7f;   // world units from cell centre

        private bool _visible;
        private float _pulseT;

        // ── Lifecycle ─────────────────────────────────────────

        private void Awake()
        {
            // Create child renderers if not assigned in inspector
            if (boosterIcon == null)   boosterIcon   = CreateChildSR("BoosterIcon",  11);
            if (arrowRenderer == null) arrowRenderer = CreateChildSR("Arrow",        12);

            if (arrowHorizontal == null) arrowHorizontal = Resources.Load<Sprite>("Sprites/Boosters/Booster_Rocket_H");
            if (arrowVertical == null)   arrowVertical   = Resources.Load<Sprite>("Sprites/Boosters/Booster_Rocket_V");
            if (rocketIcon == null)      rocketIcon      = Resources.Load<Sprite>("Sprites/Boosters/Booster_Rocket");
            if (bombIcon == null)        bombIcon        = Resources.Load<Sprite>("Sprites/Boosters/Booster_Bomb");
            if (vortexIcon == null)      vortexIcon      = Resources.Load<Sprite>("Sprites/Boosters/Booster_Vortex");

            if (boosterIcon != null) boosterIcon.sortingLayerName = "Preview";
            if (arrowRenderer != null) arrowRenderer.sortingLayerName = "Preview";

            SetVisible(false);
        }

        private void Update()
        {
            if (!_visible) return;
            _pulseT += Time.deltaTime / pulsePeriod;
            float pulse = 0.7f + Mathf.Abs(Mathf.Sin(_pulseT * Mathf.PI)) * 0.3f;
            if (boosterIcon  != null) boosterIcon.transform.localScale  = Vector3.one * pulse;
            if (arrowRenderer != null) arrowRenderer.transform.localScale = Vector3.one * (0.9f + pulse * 0.1f);
        }

        // ── Public API ────────────────────────────────────────

        /// <summary>
        /// Show the preview at the given board cell, for the given booster + direction.
        /// </summary>
        public void Show(Vector3 worldPos, BoosterType booster, BoosterDirection direction,
                         FruitType fruitColor)
        {
            if (booster == BoosterType.None)
            {
                SetVisible(false);
                return;
            }

            transform.position = worldPos;
            SetVisible(true);

            // Booster icon color + sprite
            var color = FruitDataRegistry.GetColor(fruitColor);
            color.a = 0.85f;

            if (boosterIcon != null)
            {
                boosterIcon.color = color;
                boosterIcon.sprite = booster switch
                {
                    BoosterType.Rocket => rocketIcon,
                    BoosterType.Bomb   => bombIcon,
                    BoosterType.Vortex => vortexIcon,
                    _                  => null,
                };
            }

            // Direction arrow
            if (arrowRenderer != null)
            {
                arrowRenderer.gameObject.SetActive(
                    booster == BoosterType.Rocket && direction != BoosterDirection.None);

                if (direction == BoosterDirection.Horizontal)
                {
                    arrowRenderer.sprite = arrowHorizontal;
                    arrowRenderer.transform.localRotation = Quaternion.identity;
                }
                else if (direction == BoosterDirection.Vertical)
                {
                    arrowRenderer.sprite = arrowVertical;
                    arrowRenderer.transform.localRotation = Quaternion.identity;
                }
            }

            _pulseT = 0;
        }

        public void Hide() => SetVisible(false);

        // ── Private helpers ───────────────────────────────────

        private void SetVisible(bool v)
        {
            _visible = v;
            gameObject.SetActive(v);
        }

        private static SpriteRenderer CreateChildSR(string name, int sortOrder)
        {
            var go = new GameObject(name);
            var sr = go.AddComponent<SpriteRenderer>();
            sr.sortingOrder = sortOrder;
            return sr;
        }
    }
}
