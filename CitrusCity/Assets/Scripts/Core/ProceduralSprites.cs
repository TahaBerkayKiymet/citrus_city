// ============================================================
//  ProceduralSprites.cs — Citrus City
//  Generates circle/arrow/icon sprites at runtime so the
//  prototype looks good without external art assets.
//  Called once at startup by the BoardController / UI.
// ============================================================
using UnityEngine;

namespace CitrusCity.Core
{
    public static class ProceduralSprites
    {
        // ── Circle (fruit body) ───────────────────────────────

        /// <summary>
        /// Generates a smooth anti-aliased circle sprite.
        /// </summary>
        public static Sprite Circle(int diameter = 64, float feather = 3f)
        {
            var tex = new Texture2D(diameter, diameter, TextureFormat.RGBA32, false);
            tex.filterMode = FilterMode.Bilinear;
            float r = diameter * 0.5f;
            var pixels = new Color32[diameter * diameter];

            for (int y = 0; y < diameter; y++)
            {
                for (int x = 0; x < diameter; x++)
                {
                    float dx = x - r + 0.5f;
                    float dy = y - r + 0.5f;
                    float dist = Mathf.Sqrt(dx * dx + dy * dy);
                    float alpha = Mathf.Clamp01((r - dist) / feather);
                    pixels[y * diameter + x] = new Color32(255, 255, 255, (byte)(alpha * 255));
                }
            }
            tex.SetPixels32(pixels);
            tex.Apply();
            return Sprite.Create(tex, new Rect(0, 0, diameter, diameter),
                                 new Vector2(0.5f, 0.5f), diameter);
        }

        // ── Arrow (directional indicator) ────────────────────

        public static Sprite Arrow(bool horizontal, int width = 64, int height = 32)
        {
            int w = horizontal ? width  : height;
            int h = horizontal ? height : width;
            var tex = new Texture2D(w, h, TextureFormat.RGBA32, false);
            tex.filterMode = FilterMode.Bilinear;
            var pixels = new Color32[w * h];

            for (int y = 0; y < h; y++)
            {
                for (int x = 0; x < w; x++)
                {
                    float nx = (float)x / w;
                    float ny = (float)y / h;
                    // Simple chevron arrow shape
                    float mid = 0.5f;
                    float bodyHeight = 0.35f;
                    float inBody = Mathf.Abs(ny - mid) < bodyHeight ? 1f : 0f;
                    float tipX    = nx > 0.55f ? 1f : 0f;
                    float tip     = (nx > 0.55f && Mathf.Abs(ny - mid) < (1f - nx) * 1.2f) ? 1f : 0f;
                    float alpha   = Mathf.Max(
                        (inBody > 0 && nx < 0.6f) ? 1f : 0f,
                        tip);
                    pixels[y * w + x] = new Color32(255, 255, 255, (byte)(alpha * 220));
                }
            }
            tex.SetPixels32(pixels);
            tex.Apply();
            return Sprite.Create(tex, new Rect(0, 0, w, h),
                                 new Vector2(0.5f, 0.5f), horizontal ? width : height);
        }

        // ── Star (bomb icon) ─────────────────────────────────

        public static Sprite Star(int size = 64)
        {
            var tex = new Texture2D(size, size, TextureFormat.RGBA32, false);
            tex.filterMode = FilterMode.Bilinear;
            var pixels = new Color32[size * size];
            float r = size * 0.5f;

            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    float dx = x - r + 0.5f;
                    float dy = y - r + 0.5f;
                    float angle = Mathf.Atan2(dy, dx);
                    float dist  = Mathf.Sqrt(dx * dx + dy * dy);
                    int   points = 5;
                    float outer = r * 0.45f;
                    float inner = r * 0.2f;
                    float starR = Mathf.Lerp(inner, outer,
                        (Mathf.Cos(angle * points - Mathf.PI * 0.5f) + 1f) * 0.5f);
                    float alpha = Mathf.Clamp01((starR - dist + 2f) / 2f);
                    pixels[y * size + x] = new Color32(255, 255, 255, (byte)(alpha * 255));
                }
            }
            tex.SetPixels32(pixels);
            tex.Apply();
            return Sprite.Create(tex, new Rect(0, 0, size, size),
                                 new Vector2(0.5f, 0.5f), size);
        }

        // ── Swirl (vortex icon) ───────────────────────────────

        public static Sprite Swirl(int size = 64)
        {
            // Simple ring as placeholder for vortex icon
            var tex = new Texture2D(size, size, TextureFormat.RGBA32, false);
            tex.filterMode = FilterMode.Bilinear;
            var pixels = new Color32[size * size];
            float r = size * 0.5f;

            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    float dx = x - r + 0.5f;
                    float dy = y - r + 0.5f;
                    float dist = Mathf.Sqrt(dx * dx + dy * dy);
                    float ring = Mathf.Abs(dist - r * 0.6f);
                    float alpha = Mathf.Clamp01(1f - ring / (r * 0.15f));
                    pixels[y * size + x] = new Color32(255, 255, 255, (byte)(alpha * 220));
                }
            }
            tex.SetPixels32(pixels);
            tex.Apply();
            return Sprite.Create(tex, new Rect(0, 0, size, size),
                                 new Vector2(0.5f, 0.5f), size);
        }

        // ── Gradient Background ───────────────────────────────

        public static Texture2D GradientBackground(int w, int h,
            Color top, Color bottom)
        {
            var tex = new Texture2D(w, h, TextureFormat.RGB24, false);
            tex.filterMode = FilterMode.Bilinear;
            for (int y = 0; y < h; y++)
            {
                float t = (float)y / h;
                var c = Color.Lerp(bottom, top, t);
                for (int x = 0; x < w; x++)
                    tex.SetPixel(x, y, c);
            }
            tex.Apply();
            return tex;
        }
    }
}
