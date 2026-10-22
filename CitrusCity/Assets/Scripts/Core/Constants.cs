// ============================================================
//  Constants.cs — Citrus City
//  Global compile-time and runtime constants.
// ============================================================
namespace CitrusCity.Core
{
    public static class Constants
    {
        // ----- Board -----
        public const int DefaultCols        = 8;
        public const int DefaultRows        = 9;
        public const int MinGroupSize       = 2;   // minimum fruits to confirm a move
        public const int RocketGroupSize    = 5;
        public const int BombGroupSize      = 7;
        public const int VortexGroupSize    = 9;

        // ----- Cell sizing (world units) -----
        public const float CellSize         = 1.0f;
        public const float CellSpacing      = 0.05f;   // gap between cells

        // ----- Animation -----
        public const float FruitFallSpeed   = 10f;     // units per second
        public const float FruitFallDelay   = 0.05f;   // stagger between columns
        public const float PopDuration      = 0.15f;
        public const float SpawnBounce      = 0.2f;

        // ----- Selection -----
        public const float EdgeThreshold    = 0.33f;   // 33% from each edge = directional zone

        // ----- Layers -----
        public const string FruitLayer      = "Fruit";
        public const string UILayer         = "UI";

        // ----- Sorting -----
        public const int FruitSortBase      = 10;
        public const int BoosterSortBase    = 20;
        public const int PreviewSortBase    = 30;

        // ----- Tags -----
        public const string FruitTag        = "Fruit";

        // ----- Scene Names -----
        public const string MainMenuScene   = "MainMenu";
        public const string GameScene       = "GameScene";

        // ----- Player prefs -----
        public const string PrefCurrentLevel = "CurrentLevel";
    }
}
