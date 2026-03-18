using System.Collections.Generic;
using LBoL.Base;
using LBoL.ConfigData;
using LBoLEntitySideloader;
using EternalWinterMod.Enemies.Template;


namespace EternalWinterMod.Enemies.Act1
{
    public sealed class DaiyouseiGroupDef : SampleCharacterEnemyGroupTemplate
    {
        public override EnemyGroupConfig MakeConfig()
        {
            EnemyGroupConfig config = GetEnemyGroupDefaultConfig();
            config.FormationName = VanillaFormations.Single;
            config.Enemies = new List<string>() { nameof(Daiyousei) };
            config.EnemyType = EnemyType.Elite;
            config.RollBossExhibit = false;
            return config;
        }
    }
}
