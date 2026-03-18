using System.Collections.Generic;
using LBoL.Base;
using LBoL.ConfigData;
using LBoLEntitySideloader;
using EternalWinterMod.Enemies.Template;


namespace EternalWinterMod.Enemies.Act1
{
    public sealed class YukidoujiGroupDef : SampleCharacterEnemyGroupTemplate
    {
        public override EnemyGroupConfig MakeConfig()
        {
            EnemyGroupConfig config = GetEnemyGroupDefaultConfig();
            config.FormationName = VanillaFormations.Single;
            config.Enemies = new List<string>() { nameof(Yukidouji) };
            config.EnemyType = EnemyType.Normal;
            config.RollBossExhibit = false;
            return config;
        }
    }
}
