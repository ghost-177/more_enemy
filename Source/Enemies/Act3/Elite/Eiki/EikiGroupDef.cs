using LBoL.Base;
using LBoL.ConfigData;
using LBoLEntitySideloader;
using EternalWinterMod.Enemies.Template;
using System.Collections.Generic;


namespace EternalWinterMod.Enemies.Act3
{
    public sealed class EikiEWGroupDef : SampleCharacterEnemyGroupTemplate
    {
        public override EnemyGroupConfig MakeConfig()
        {
            EnemyGroupConfig config = GetEnemyGroupDefaultConfig();
            config.FormationName = VanillaFormations.Single;
            config.Enemies = new List<string>() { "EikiEW" };
            config.EnemyType = EnemyType.Elite;
            config.RollBossExhibit = false;
            return config;
        }
    }
}
