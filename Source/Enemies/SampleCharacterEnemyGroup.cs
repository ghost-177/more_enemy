using System.Collections.Generic;
using LBoL.Base;
using LBoL.ConfigData;
using LBoLEntitySideloader;
using EternalWinterMod.Enemies.Template;


namespace EternalWinterMod.Enemies
{
    public sealed class SampleCharacterEnemyGroupDef : SampleCharacterEnemyGroupTemplate
    {
        public override IdContainer GetId() => nameof(EternalWinterMod);

        public override EnemyGroupConfig MakeConfig()
        {
            EnemyGroupConfig config = GetEnemyGroupDefaultConfig();
            config.Name = nameof(EternalWinterMod);
            config.FormationName = VanillaFormations.Single;
            config.Enemies = new List<string>() { nameof(EternalWinterMod) };
            config.EnemyType = EnemyType.Boss;
            config.RollBossExhibit = true;

            return config;
        }
    }
}