// ============================================================
//  FruitView.cs — Citrus City
//  MonoBehaviour: visual representation of a single board cell.
//  Drives sprite/color, selection highlight, and fall animation.
//  Uses Unity coroutines (no external tween library required).
// ============================================================
using System.Collections;
using UnityEngine;
using CitrusCity.Fruit;
using CitrusCity.Boosters;

namespace CitrusCity.Fruit
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class FruitView : MonoBehaviour
    {
        // ── State ─────────────────────────────────────────────
        public int Col   { get; private set; }
        public int Row   { get; private set; }
        public FruitType FruitType { get; private set; }
        public BoosterType BoosterType { get; private set; }

        private SpriteRenderer _sr;
        private SpriteRenderer _iconSr;     // child renderer for booster icon
        private Coroutine _animCoroutine;
        private bool _selected;

        // ── Public sprites (set by BoardController / pool) ────
        [Header("Built-in Fallback Sprites")]
        [SerializeField] private Sprite circleSprite;   // procedural if null
        [SerializeField] private Sprite starSprite;
        [SerializeField] private Sprite diamondSprite;

        // ── Lifecycle ─────────────────────────────────────────

        private void Awake()
        {
            _sr = GetComponent<SpriteRenderer>();
            _sr.sortingLayerName = "Fruit";

            // Create child for booster icon
            var iconGo = new GameObject("Icon");
            iconGo.transform.SetParent(transform, false);
            _iconSr = iconGo.AddComponent<SpriteRenderer>();
            _iconSr.sortingLayerName = "Fruit";
            _iconSr.sortingOrder = _sr.sortingOrder + 1;
        }

        // ── Initialisation ────────────────────────────────────

        public void Init(int col, int row, FruitType type,
                         BoosterType booster = BoosterType.None,
                         BoosterDirection dir = BoosterDirection.None)
        {
            Col = col; Row = row;
            FruitType   = type;
            BoosterType = booster;

            ApplyVisuals(type, booster, dir);
        }

        // ── Visuals ───────────────────────────────────────────

        private void ApplyVisuals(FruitType type, BoosterType booster, BoosterDirection dir)
        {
            var data = FruitDataRegistry.Get(type);

            if (booster == BoosterType.Rocket)
            {
                var rSprite = data?.rocketSprite ?? Resources.Load<Sprite>("Sprites/Boosters/Booster_Rocket");
                _sr.sprite = rSprite;
                _sr.color = Color.white;

                _iconSr.enabled = true;
                _iconSr.sprite = dir == BoosterDirection.Horizontal
                    ? Resources.Load<Sprite>("Sprites/Boosters/Booster_Rocket_H")
                    : Resources.Load<Sprite>("Sprites/Boosters/Booster_Rocket_V");
                _iconSr.color = Color.white;
                _iconSr.transform.localScale = Vector3.one * 0.7f;
            }
            else if (booster == BoosterType.Bomb)
            {
                var bSprite = data?.bombSprite ?? Resources.Load<Sprite>("Sprites/Boosters/Booster_Bomb");
                _sr.sprite = bSprite;
                _sr.color = Color.white;
                _iconSr.enabled = false;
            }
            else if (booster == BoosterType.Vortex)
            {
                var vSprite = data?.vortexSprite ?? Resources.Load<Sprite>("Sprites/Boosters/Booster_Vortex");
                _sr.sprite = vSprite;
                _sr.color = Color.white;
                _iconSr.enabled = false;
            }
            else
            {
                // Regular fruit
                Sprite fruitSprite = data?.sprite;
                if (fruitSprite != null)
                {
                    _sr.sprite = fruitSprite;
                    _sr.color = Color.white;
                }
                else
                {
                    if (circleSprite != null) _sr.sprite = circleSprite;
                    else _sr.sprite = ProceduralSpriteInitializer.CircleSprite;
                    _sr.color = FruitDataRegistry.GetColor(type);
                }
                _iconSr.enabled = false;
            }
        }

        // ── Selection highlight ───────────────────────────────

        public void SetSelected(bool selected)
        {
            _selected = selected;
            StopBounce();
            if (selected)
                StartBounce();
            else
            {
                transform.localScale = Vector3.one;
                _sr.color = (FruitType.IsRegular() && FruitDataRegistry.Get(FruitType)?.sprite != null)
                    ? Color.white
                    : FruitDataRegistry.GetColor(FruitType);
            }
        }

        private void StartBounce()
        {
            _animCoroutine = StartCoroutine(BounceLoop());
        }

        private void StopBounce()
        {
            if (_animCoroutine != null) StopCoroutine(_animCoroutine);
            _animCoroutine = null;
        }

        private IEnumerator BounceLoop()
        {
            float t = 0;
            while (true)
            {
                t += Time.deltaTime * 5f;
                float s = 1.06f + Mathf.Sin(t) * 0.08f;
                transform.localScale = new Vector3(s, s, 1f);
                yield return null;
            }
        }

        // ── Fall animation ────────────────────────────────────

        public void AnimateFallTo(Vector3 targetWorld, float speed, System.Action onComplete = null)
        {
            StopBounce();
            _animCoroutine = StartCoroutine(FallTo(targetWorld, speed, onComplete));
        }

        private IEnumerator FallTo(Vector3 target, float speed, System.Action onComplete)
        {
            while (Vector3.Distance(transform.position, target) > 0.01f)
            {
                transform.position = Vector3.MoveTowards(transform.position, target,
                                                         speed * Time.deltaTime);
                yield return null;
            }
            transform.position = target;
            transform.localScale = Vector3.one;
            onComplete?.Invoke();
        }

        // ── Spawn-from-above animation ────────────────────────

        public void AnimateSpawnFrom(Vector3 startWorld, Vector3 targetWorld, float speed)
        {
            transform.position = startWorld;
            transform.localScale = Vector3.one * 0.1f;
            _animCoroutine = StartCoroutine(SpawnAnim(targetWorld, speed));
        }

        private IEnumerator SpawnAnim(Vector3 target, float speed)
        {
            // Scale up while falling
            while (Vector3.Distance(transform.position, target) > 0.01f)
            {
                transform.position = Vector3.MoveTowards(transform.position, target,
                                                         speed * Time.deltaTime);
                float prog = 1f - Vector3.Distance(transform.position, target) /
                                   Vector3.Distance(transform.position + (target - transform.position).normalized * 0.1f, target);
                transform.localScale = Vector3.Lerp(Vector3.one * 0.1f, Vector3.one, Mathf.Clamp01(prog * 3f));
                yield return null;
            }
            transform.position    = target;
            transform.localScale  = Vector3.one;
        }

        // ── Pop (removal) animation ───────────────────────────

        public void AnimatePop(System.Action onComplete = null)
        {
            StopBounce();
            _animCoroutine = StartCoroutine(PopAnim(onComplete));
        }

        private IEnumerator PopAnim(System.Action onComplete)
        {
            float t = 0;
            while (t < Core.Constants.PopDuration)
            {
                t += Time.deltaTime;
                float progress = t / Core.Constants.PopDuration;
                float s = 1f + Mathf.Sin(progress * Mathf.PI) * 0.3f;
                transform.localScale = new Vector3(s, s, 1f);
                _sr.color = new Color(_sr.color.r, _sr.color.g, _sr.color.b,
                                      1f - progress);
                yield return null;
            }
            onComplete?.Invoke();
            gameObject.SetActive(false);
        }

        // ── Booster activation flash ──────────────────────────

        public void AnimateBoosterActivate()
        {
            _animCoroutine = StartCoroutine(FlashWhite());
        }

        private IEnumerator FlashWhite()
        {
            float t = 0;
            Color orig = _sr.color;
            while (t < 0.2f)
            {
                t += Time.deltaTime;
                _sr.color = Color.Lerp(Color.white, orig, t / 0.2f);
                yield return null;
            }
        }

        // ── Pool reset ────────────────────────────────────────

        public void ResetForPool()
        {
            StopBounce();
            transform.localScale = Vector3.one;
            _sr.color            = Color.white;
            _iconSr.enabled      = false;
        }
    }
}
