// ============================================================
//  ProceduralSpriteInitializer.cs — Citrus City
//  MonoBehaviour: boots procedural sprites at scene start and
//  injects them into FruitView instances.
//  Attach to a persistent scene object (e.g., GameManager root).
// ============================================================
using UnityEngine;
using CitrusCity.Core;

namespace CitrusCity.Core
{
    public class ProceduralSpriteInitializer : MonoBehaviour
    {
        public static Sprite CircleSprite   { get; private set; }
        public static Sprite ArrowHSprite   { get; private set; }
        public static Sprite ArrowVSprite   { get; private set; }
        public static Sprite StarSprite     { get; private set; }
        public static Sprite SwirlSprite    { get; private set; }

        private void Awake()
        {
            CircleSprite = ProceduralSprites.Circle(128, 4f);
            ArrowHSprite = ProceduralSprites.Arrow(horizontal: true, width: 96, height: 40);
            ArrowVSprite = ProceduralSprites.Arrow(horizontal: false, width: 40, height: 96);
            StarSprite   = ProceduralSprites.Star(96);
            SwirlSprite  = ProceduralSprites.Swirl(96);

            Fruit.FruitDataRegistry.Initialize();
        }
    }
}
