using LBoLEntitySideloader.Entities;
using LBoLEntitySideloader.Resource;

namespace EternalWinterMod.Localization
{
    public sealed class SampleCharacterLocalization
    {
        public static string EnemiesUnit = "EnemyUnit";
        public static string StatusEffects = "StatusEffects";
        public static string Cards = "Cards";

        public static BatchLocalization EnemiesUnitBatchLoc = new BatchLocalization(BepinexPlugin.directorySource, typeof(EnemyUnitTemplate), EnemiesUnit);
        public static BatchLocalization StatusEffectsBatchLoc = new BatchLocalization(BepinexPlugin.directorySource, typeof(StatusEffectTemplate), StatusEffects);
        public static BatchLocalization CardsBatchLoc = new BatchLocalization(BepinexPlugin.directorySource, typeof(CardTemplate), Cards);

        public static void Init()
        {
            EnemiesUnitBatchLoc.DiscoverAndLoadLocFiles(EnemiesUnit);
            StatusEffectsBatchLoc.DiscoverAndLoadLocFiles(StatusEffects);
            CardsBatchLoc.DiscoverAndLoadLocFiles(Cards);
        }
    }
}
