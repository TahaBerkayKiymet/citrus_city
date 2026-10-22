// ============================================================
//  BoosterType.cs / BoosterDirection.cs — Citrus City
//  Enums and factory for booster generation.
// ============================================================
namespace CitrusCity.Boosters
{
    public enum BoosterType
    {
        None   = 0,
        Rocket = 1,
        Bomb   = 2,
        Vortex = 3,
    }

    public enum BoosterDirection
    {
        None       = 0,
        Horizontal = 1,
        Vertical   = 2,
    }

    // ── Factory ───────────────────────────────────────────────

    /// <summary>
    /// Pure C#: maps group size → booster type.
    /// Thresholds match the GDD: 5→Rocket, 7→Bomb, 9→Vortex.
    /// </summary>
    public static class BoosterFactory
    {
        public static BoosterType GetBoosterForGroupSize(int size)
        {
            if (size >= Core.Constants.VortexGroupSize) return BoosterType.Vortex;
            if (size >= Core.Constants.BombGroupSize)   return BoosterType.Bomb;
            if (size >= Core.Constants.RocketGroupSize) return BoosterType.Rocket;
            return BoosterType.None;
        }
    }
}
