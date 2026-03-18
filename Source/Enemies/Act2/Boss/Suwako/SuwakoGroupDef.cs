using System.Collections.Generic;
using LBoL.Base;
using LBoL.ConfigData;
using LBoLEntitySideloader;
using EternalWinterMod.Enemies.Template;


namespace EternalWinterMod.Enemies.Act2
{
    public sealed class SuwakoGroupDef : SampleCharacterEnemyGroupTemplate
    {
        public override EnemyGroupConfig MakeConfig()
        {
            EnemyGroupConfig config = GetEnemyGroupDefaultConfig();
            config.FormationName = VanillaFormations.Single;
            config.Enemies = new List<string>() { nameof(Suwako) };
            config.EnemyType = EnemyType.Boss;
            config.RollBossExhibit = true;
            return config;
        }
    }
}
