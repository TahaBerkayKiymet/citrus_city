// ============================================================
//  FruitType.cs — Citrus City
//  Enum for all fruit and special-item types on the board.
// ============================================================
namespace CitrusCity.Fruit
{
    public enum FruitType
    {
        None    = 0,

        // ── Regular fruits ───
        Orange  = 1,
        Lemon   = 2,
        Lime    = 3,
        Cherry  = 4,
        Grape   = 5,

        // ── Special items ────
        Rocket  = 10,
        Bomb    = 11,
        Vortex  = 12,   // Rainbow / clears all of one color
    }

    public static class FruitTypeExtensions
    {
        public static bool IsRegular(this FruitType t) =>
            t >= FruitType.Orange && t <= FruitType.Grape;

        public static bool IsSpecial(this FruitType t) =>
            t == FruitType.Rocket || t == FruitType.Bomb || t == FruitType.Vortex;
    }
}
