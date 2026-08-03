using MonoMerge.Grid;

namespace MonoMerge.Core
{
    /// <summary>
    /// GDD 1: "Oyun Sonu (Fail State): Izgarada yeni bir tas koyacak yer kalmadiginda oyun
    /// biter." Placement has no rule beyond "cell must be empty" (GDD: "izgaradaki bos bir
    /// huecreye birakir") — there is no color/value matching restriction on where a tray tile
    /// can go — so a fully occupied grid is the sole, sufficient game-over condition.
    ///
    /// Extracted as its own static check (rather than inlined in GameManager) so the end
    /// condition is independently testable and reusable without touching MonoBehaviour state.
    /// </summary>
    public static class GameOverChecker
    {
        public static bool IsGameOver(GridManager grid) => grid != null && !grid.HasAnyEmptyCell();
    }
}
