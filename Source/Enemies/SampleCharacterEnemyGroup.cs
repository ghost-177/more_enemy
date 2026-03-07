using System.Collections.Generic;
using LBoL.Base;
using LBoL.ConfigData;
using LBoLEntitySideloader;
using SampleCharacterMod.Enemies.Template;


namespace SampleCharacterMod.Enemies
{
    public sealed class SampleCharacterEnemyGroupDef : SampleCharacterEnemyGroupTemplate
    {
        public override IdContainer GetId() => nameof(SampleCharacterMod);

        public override EnemyGroupConfig MakeConfig()
        {
            EnemyGroupConfig config = GetEnemyGroupDefaultConfig();
            config.Name = nameof(SampleCharacterMod);
            config.FormationName = VanillaFormations.Single;
            config.Enemies = new List<string>() { nameof(SampleCharacterMod) };
            config.EnemyType = EnemyType.Boss;
            config.RollBossExhibit = true;

            return config;
        }
    }
}