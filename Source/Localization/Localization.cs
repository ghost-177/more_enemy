using LBoLEntitySideloader.Entities;
using LBoLEntitySideloader.Resource;

namespace EternalWinterMod.Localization
{
    public sealed class SampleCharacterLocalization
    {
        public static string EnemiesUnit = "EnemyUnit";
        public static string StatusEffects = "StatusEffects";
        public static string Cards = "Cards";
        public static string Exhibits = "Exhibits";
        public static string UnitModel = "UnitModel";

        public static BatchLocalization EnemiesUnitBatchLoc = new BatchLocalization(BepinexPlugin.directorySource, typeof(EnemyUnitTemplate), EnemiesUnit);
        public static BatchLocalization StatusEffectsBatchLoc = new BatchLocalization(BepinexPlugin.directorySource, typeof(StatusEffectTemplate), StatusEffects);
        public static BatchLocalization CardsBatchLoc = new BatchLocalization(BepinexPlugin.directorySource, typeof(CardTemplate), Cards);
        public static BatchLocalization ExhibitsBatchLoc = new BatchLocalization(BepinexPlugin.directorySource, typeof(ExhibitTemplate), Exhibits);
        public static BatchLocalization UnitModelBatchLoc = new BatchLocalization(BepinexPlugin.directorySource, typeof(UnitModelTemplate), UnitModel);

        public static void Init()
        {
            EnemiesUnitBatchLoc.DiscoverAndLoadLocFiles(EnemiesUnit);
            StatusEffectsBatchLoc.DiscoverAndLoadLocFiles(StatusEffects);
            CardsBatchLoc.DiscoverAndLoadLocFiles(Cards);
            ExhibitsBatchLoc.DiscoverAndLoadLocFiles(Exhibits);
            UnitModelBatchLoc.DiscoverAndLoadLocFiles(UnitModel);
        }
    }
}
